using Discord.WebSocket;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace cherryBridge.Models.Discord
{
  public class DCSocketMessage
  {
    public DCSocketMessage(SocketMessage message)
    {
      Id = message.Id.ToString();
      Content = message.Content;
      Timestamp = message.Timestamp;
      EditedTimestamp = message.EditedTimestamp;
      IsTTS = message.IsTTS;
      IsPinned = message.IsPinned;
      Source = message.Source.ToString();
      Author = message.Author != null ? new DCSocketUser(message.Author) : null;
      Channel = message.Channel != null ? new DCSocketChannel(message.Channel) : null;

    }
    public DCSocketMessage()
    {

    }

    public string? Id { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? EditedTimestamp { get; set; }
    public bool IsTTS { get; set; }
    public bool IsPinned { get; set; }
    public string? Source { get; set; }
    public DCSocketUser? Author { get; set; }
    public DCSocketChannel? Channel { get; set; }
    public bool IsSlashCommand { get; set; }

    //public string Id { get; set; } = message.Id.ToString();
    //public string? Content { get; set; } = message.Content;
    //public DateTimeOffset Timestamp { get; set; } = message.Timestamp;
    //public DateTimeOffset? EditedTimestamp { get; set; } = message.EditedTimestamp;
    //public bool IsTTS { get; set; } = message.IsTTS;
    //public bool IsPinned { get; set; } = message.IsPinned;
    //public string? Source { get; set; } = message.Source.ToString();
    //public DCSocketUser? Author { get; set; } = message.Author != null ? new DCSocketUser(message.Author) : null;
    //public DCSocketChannel? Channel { get; set; } = message.Channel != null ? new DCSocketChannel(message.Channel) : null;


    public static DCSocketMessage FromSlashCommand(DCSocketSlashCommand command)
    {
      return new DCSocketMessage
      {
        IsSlashCommand = true,
        Author = command.User,
        Channel = command.Channel,
        Content = command.BuildMessage(),
        Timestamp = command.CreatedAt,
        Id = command.Id
      };
    }
  }
}
