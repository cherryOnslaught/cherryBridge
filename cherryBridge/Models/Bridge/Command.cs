using Discord;

namespace cherryBridge.Models.Bridge
{
  public class Command
  {
    public string? Name { get; set; }
    public string? Description { get; set; }

    public SlashCommandProperties Build()
    {
      return new SlashCommandBuilder()
         .WithName(Name)
         .WithDescription(Description)
         .AddOption(new SlashCommandOptionBuilder()
         .WithName("message")
         .WithDescription("The message to send back with the command")
         .WithType(ApplicationCommandOptionType.String)
         .WithRequired(false))
         .Build();
    }
  }
}
