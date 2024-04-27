namespace cherryBridge.Models.Bridge
{
  public class ServerStatus
  {
    public bool DiscordConnected { get; set; }
    public required string Version { get; set; } = "v0.1";
  }
}
