using Discord.WebSocket;
using System.Threading.Channels;

namespace cherryBridge.Models.Discord
{
  public class DCSocketChannel
  {
    public DCSocketChannel(ISocketMessageChannel channel)
    {
      Id = channel.Id.ToString();
      Name = channel.Name;
      if (channel is SocketGuildChannel guildChannel)
      {
        Construct(guildChannel);
      }
    }
    public DCSocketChannel(SocketGuildChannel channel)
    {
      Id = channel.Id.ToString();
      Name = channel.Name;
      Construct(channel);
    }
    public DCSocketChannel(ulong channelId,string userName)
    {
      Id = channelId.ToString();
      Name = $"DM {userName}";
    }

    private void Construct(SocketGuildChannel channel)
    {
      Position = channel.Position;
      Guild = new DCSocketGuild(channel.Guild);
      CurrentUser = new DCSocketGuildUser(channel.Guild.CurrentUser);
    }

    public string? Id { get; set; }
    public string? Name { get; set; }
    public int Position { get; set; }
    public DCSocketGuild? Guild { get; set; }
    public DCSocketGuildUser? CurrentUser { get; set; }
  }
}
