using Discord.WebSocket;

namespace cherryBridge.Models.Discord
{
  public class DCSocketUser
  {
    public DCSocketUser(SocketUser user)
    {
      Id = user.Id.ToString();
      Username = user.Username;
      Discriminator = user.Discriminator;
      AvatarId = user.GetAvatarUrl() ?? string.Empty;
      IsBot = user.IsBot;
      IsWebhook = user.IsWebhook;
      if (user is SocketGuildUser guildUser)
      {
        GuildUser = new DCSocketGuildUser(guildUser);
      }
    }

    public string Id { get; set; }
    public string? Username { get; set; }
    public string? Discriminator { get; set; }
    public string? AvatarId { get; set; }
    public bool IsBot { get; set; }
    public bool IsWebhook { get; set; }
    public DCSocketGuildUser? GuildUser { get; set; }
  }
}
