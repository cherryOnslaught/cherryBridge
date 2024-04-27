using Discord.WebSocket;

namespace cherryBridge.Models.Discord
{
  public class DCSocketRole(SocketRole role)
  {
    public ulong Id { get; set; } = role.Id;
    public string? Name { get; set; } = role.Name;
    public bool IsHoisted { get; set; } = role.IsHoisted;
    public int Position { get; set; } = role.Position;
    public bool IsManaged { get; set; } = role.IsManaged;
    public bool IsMentionable { get; set; } = role.IsMentionable;
  }
}
