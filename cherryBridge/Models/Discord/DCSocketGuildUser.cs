using Discord.WebSocket;
using System.Text.Json.Serialization;

namespace cherryBridge.Models.Discord
{
  public class DCSocketGuildUser(SocketGuildUser guildUser)
  {
    public string Id { get; set; } = guildUser.Id.ToString();
    public string? Username { get; set; } = guildUser.Username;
    public ulong GuildId { get; set; } = guildUser.Guild.Id;
    public string? Nickname { get; set; } = guildUser.Nickname;
    //public List<DCSocketRole>? Roles { get; set; }
    public DateTimeOffset JoinedAt { get; set; } = guildUser.JoinedAt ?? DateTimeOffset.MinValue;
    public bool IsMuted { get; set; } = guildUser.IsMuted;
    public bool IsDeafened { get; set; } = guildUser.IsDeafened;
  }
}
