using cherryBridge.Models.Discord;
using Discord;
using System.Text.Json.Serialization;

namespace cherryBridge.Models
{
  public class API_Start
  {
    public required string BotToken { get; set; }
  }
}