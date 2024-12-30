namespace MSFEP.Models;

public class GitHubAppConfig
{
    public required string AppId { get; set; } = string.Empty;
    public required string PrivateKey { get; set; } = string.Empty;
    public required string InstallationId { get; set; } = string.Empty;
}
