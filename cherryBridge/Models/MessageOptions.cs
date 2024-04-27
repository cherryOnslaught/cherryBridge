using cherryBridge.Models.Bridge;
using cherryBridge.Models.Discord;
using Discord;
using System.Text.Json.Serialization;

namespace cherryBridge.Models
{

  public class MessageOptions
  {
    public MessageOptions()
    {

    }
    public required string SendId { get; set; }

    public required string Content { get; set; }
    public required string IdType { get; set; }

    public required List<DCButton> Buttons { get; set; }
 
    public ulong GetSendID()
    {
      if (ulong.TryParse(SendId, out ulong sendID))
      {
        return sendID;
      }
      else
      {
        return 0;
      }
    }
  }
}
