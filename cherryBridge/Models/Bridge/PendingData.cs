using cherryBridge.Models.Discord;

namespace cherryBridge.Models.Bridge
{
  public class PendingData
  {
    public required List<DCSocketMessage> Messages { get; set; } = [];
    public required List<DCComponent> Options { get; set; } = [];    
  }
}
