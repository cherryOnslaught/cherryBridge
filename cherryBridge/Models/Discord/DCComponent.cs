using Discord.WebSocket;

namespace cherryBridge.Models.Discord
{
  public class DCComponent(SocketMessageComponent component)
  {
    public string CustomID { get; set; } = component.Data.CustomId;
    public DCSocketUser User { get; set; } = new DCSocketUser(component.User);
    public List<string>? Values { get; set; } = component?.Data?.Values?.ToList();

    public string? ChannelID { get; set; } = component?.Channel?.Id.ToString();
    public string? MessageID { get; set; } = component?.Message?.Id.ToString();

    }
}
