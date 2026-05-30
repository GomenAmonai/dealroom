namespace DealRoom.Tests.Integration;

public static class DockerEnvironment
{
    public static bool IsAvailable { get; } = Detect();

    private static bool Detect()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOCKER_HOST")))
            return true;

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] sockets =
        {
            "/var/run/docker.sock",
            Path.Combine(home, ".docker/run/docker.sock"),
            Path.Combine(home, ".colima/default/docker.sock"),
        };
        return sockets.Any(File.Exists);
    }
}

// Skips the test when no Docker daemon is reachable, so the suite still
// passes on machines (or CI) without Docker.
public sealed class DockerFactAttribute : FactAttribute
{
    public DockerFactAttribute()
    {
        if (!DockerEnvironment.IsAvailable)
            Skip = "Docker is not available; integration test skipped.";
    }
}
