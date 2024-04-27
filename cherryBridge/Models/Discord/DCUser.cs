using Discord;

namespace cherryBridge.Models.Discord
{
  public class DCUser(IUser user)
  {
    public string Id { get; set; } = user.Id.ToString();
    public string Username { get; set; } = user.Username;
    public string Discriminator { get; set; } = user.Discriminator;
    public bool IsBot { get; set; } = user.IsBot;
  }
}
