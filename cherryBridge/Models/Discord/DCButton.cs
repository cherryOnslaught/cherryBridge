using Discord;
using System.Net.NetworkInformation;

namespace cherryBridge.Models.Discord
{ 
  public class DCButton
  {
    public required string Title { get; set; }
    public required int Id { get; set; }

    public static List<MessageComponent> Build(List<DCButton> buttons)
    {
      if (buttons.Count == 0)
        return [];

      var components = new List<MessageComponent>();

      var componentCount = (int)Math.Ceiling((buttons.Count / 25m));

      for (int i = 0; i < componentCount; i++)
      {
        var builder = new ComponentBuilder();

        for (int j = 0; j < 25; j++)
        {
          var index = i * 25 + j;
          if (index >= buttons.Count)
            break;

          var button = buttons[index];
          builder.WithButton(button.Title, button.Id.ToString());
        }
        //foreach (var button in buttons)
        //{
        //  builder.WithButton(button.Title, button.Id.ToString());
        //}
        components.Add(builder.Build());
      }

      return components;
    }
  }
}