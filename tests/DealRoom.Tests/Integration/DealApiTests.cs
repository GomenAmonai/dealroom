using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DealRoom.Core.DTOs;

namespace DealRoom.Tests.Integration;

public class DealApiTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _factory;

    public DealApiTests(ApiFactory factory) => _factory = factory;

    private sealed record TestOrg(HttpClient Client, int Id);

    private async Task<TestOrg> RegisterOrgAsync()
    {
        var client = _factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");

        var response = await client.PostAsJsonAsync("/auth/register", new
        {
            name = "User",
            email = $"user-{suffix}@example.com",
            password = "Password123!",
            organizationName = $"Org-{suffix[..8]}",
        });
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(Json);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var org = await client.GetFromJsonAsync<OrganizationResponse>("/organizations/me", Json);
        return new TestOrg(client, org!.Id);
    }

    private async Task<DealResponse> CreateDealAsync(TestOrg initiator, TestOrg counterparty)
    {
        var response = await initiator.Client.PostAsJsonAsync("/deals", new
        {
            title = "Supply contract",
            counterpartyOrganizationId = counterparty.Id,
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DealResponse>(Json))!;
    }

    private static Task<HttpResponseMessage> PatchStatusAsync(HttpClient client, int dealId, string status)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/deals/{dealId}/status")
        {
            Content = JsonContent.Create(new { status }),
        };
        return client.SendAsync(request);
    }

    [DockerFact]
    public async Task Register_ReturnsAccessToken()
    {
        var org = await RegisterOrgAsync();

        Assert.True(org.Id > 0);
    }

    [DockerFact]
    public async Task CreateDeal_BetweenTwoOrgs_StartsInDraft()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();

        var deal = await CreateDealAsync(initiator, counterparty);

        Assert.Equal("Draft", deal.Status);
    }

    [DockerFact]
    public async Task CreateDeal_WithSelf_ReturnsBadRequest()
    {
        var org = await RegisterOrgAsync();

        var response = await org.Client.PostAsJsonAsync("/deals", new
        {
            title = "X",
            counterpartyOrganizationId = org.Id,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [DockerFact]
    public async Task GetDeals_ReturnsCreatedDeal()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();
        var deal = await CreateDealAsync(initiator, counterparty);

        var deals = await initiator.Client.GetFromJsonAsync<List<DealResponse>>("/deals", Json);

        Assert.Contains(deals!, d => d.Id == deal.Id);
    }

    [DockerFact]
    public async Task UpdateStatus_DraftToActive_ReturnsActive()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();
        var deal = await CreateDealAsync(initiator, counterparty);

        var response = await PatchStatusAsync(counterparty.Client, deal.Id, "Active");
        var updated = await response.Content.ReadFromJsonAsync<DealResponse>(Json);

        Assert.Equal("Active", updated!.Status);
    }

    [DockerFact]
    public async Task UpdateStatus_DraftToClosed_ReturnsUnprocessableEntity()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();
        var deal = await CreateDealAsync(initiator, counterparty);

        var response = await PatchStatusAsync(initiator.Client, deal.Id, "Closed");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [DockerFact]
    public async Task GetDeal_AsNonParticipant_ReturnsForbidden()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();
        var outsider = await RegisterOrgAsync();
        var deal = await CreateDealAsync(initiator, counterparty);

        var response = await outsider.Client.GetAsync($"/deals/{deal.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [DockerFact]
    public async Task GetDeals_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/deals");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [DockerFact]
    public async Task UploadDocument_ThenList_ReturnsDocument()
    {
        var initiator = await RegisterOrgAsync();
        var counterparty = await RegisterOrgAsync();
        var deal = await CreateDealAsync(initiator, counterparty);

        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
        file.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        form.Add(file, "file", "contract.pdf");
        var upload = await initiator.Client.PostAsync($"/deals/{deal.Id}/documents", form);
        upload.EnsureSuccessStatusCode();

        var documents = await initiator.Client.GetFromJsonAsync<List<DocumentResponse>>(
            $"/deals/{deal.Id}/documents", Json);

        Assert.Single(documents!);
    }

    [DockerFact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
