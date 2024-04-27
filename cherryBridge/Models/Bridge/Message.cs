using cherryBridge.Models.Discord;
using Discord.WebSocket;

namespace cherryBridge.Models.Bridge
{

  public class Message()
  {
    public string? Content { get; set; }
    public string ChannelID { get; set; }
  }
}
