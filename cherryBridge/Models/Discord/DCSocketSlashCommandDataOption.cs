using Discord.WebSocket;
using Discord;

namespace cherryBridge.Models.Discord
{
  public class DCSocketSlashCommandDataOption(SocketSlashCommandDataOption option)
  {
    public string Name { get; set; } = option.Name;
    public object Value { get; set; } = option.Value;
    public ApplicationCommandOptionType Type { get; set; } = option.Type;
  }
}
