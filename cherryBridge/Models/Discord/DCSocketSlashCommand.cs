using cherryBridge.Models.Bridge;
using cherryBridge.Models.Discord;
using Discord.WebSocket;
using System.Text;

namespace cherryBridge.Models.Discord
{
  public class DCSocketSlashCommand
  {
    public string? Id { get; set; }
    public string? ApplicationId { get; set; }
    public ulong? GuildId { get; set; }
    public DCSocketChannel Channel { get; set; }
    public string CommandName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DCSocketUser User { get; set; }
    public List<DCSocketSlashCommandDataOption> Options { get; set; }


    public DCSocketSlashCommand(SocketSlashCommand command)
    {
      Id = command.Id.ToString();
      ApplicationId = command.ApplicationId.ToString();
      GuildId = command.GuildId;
      Channel = new DCSocketChannel(command.Channel);
      CommandName = command.CommandName;
      CreatedAt = command.CreatedAt;
      User = new DCSocketUser(command.User);
      Options = [];
      foreach (var option in command.Data.Options)
      {
        Options.Add(new DCSocketSlashCommandDataOption(option));
      }
    }

    public string BuildMessage()
    {
      var contentBuilder = new StringBuilder($"/{CommandName}");

      foreach (var option in Options)
        contentBuilder.Append($" {option.Value}");

      return contentBuilder.ToString();
    }


  }
}

//public class DCSocketMessage(SocketMessage message)
//{
//  public string Id { get; set; } = message.Id.ToString();
//  public string? Content { get; set; } = message.Content;
//  public DateTimeOffset Timestamp { get; set; } = message.Timestamp;
//  public DateTimeOffset? EditedTimestamp { get; set; } = message.EditedTimestamp;
//  public bool IsTTS { get; set; } = message.IsTTS;
//  public bool IsPinned { get; set; } = message.IsPinned;
//  public string? Source { get; set; } = message.Source.ToString();
//  public DCSocketUser? Author { get; set; } = message.Author != null ? new DCSocketUser(message.Author) : null;
//  public DCSocketChannel? Channel { get; set; } = message.Channel != null ? new DCSocketChannel(message.Channel) : null;
//}
