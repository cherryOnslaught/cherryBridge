using cherryBridge.Models.Bridge;
using Discord;

namespace cherryBridge.Models.Discord
{

  public class ReplyOptions
  {
    public required string CustomID { get; set; }

    public required string Content { get; set; }

    public required List<DCButton> Buttons { get; set; }
    //public MessageComponent Build()
    //{


    //  var builder = new ComponentBuilder();
    //  foreach (var button in Buttons)
    //  {
    //    builder.WithButton(button.Title, button.Id.ToString());
    //  }
    //  return builder.Build();
    //}
    public ulong GetSendID()
    {
      if (ulong.TryParse(CustomID, out ulong customID))
      {
        return customID;
      }
      else
      {
        return 0;
      }
    }
  }
}


