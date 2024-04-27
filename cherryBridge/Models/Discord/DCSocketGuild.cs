using Discord.WebSocket;
using System.Text.Json.Serialization;

namespace cherryBridge.Models.Discord
{
  public class DCSocketGuild(SocketGuild guild)
  {
    public string Id { get; set; } = guild.Id.ToString();
    public string? Name { get; set; } = guild.Name;
  }
}
