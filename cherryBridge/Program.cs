using cherryBridge.Models;
using cherryBridge.Models.Bridge;
using cherryBridge.Models.Discord;
using Discord.WebSocket;
using System.Text.Json.Serialization;

namespace cherryBridge
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateSlimBuilder(args);
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
      builder.Logging.SetMinimumLevel(LogLevel.Debug);

      builder.Services.ConfigureHttpJsonOptions(options =>
      {
        options.SerializerOptions.TypeInfoResolverChain.Add(AppJsonSerializerContext.Default);
      });

      // Configure services here
      builder.Services.AddLogging(configure => configure.AddConsole());
      builder.Services.AddSingleton<DiscordSocketClient>();
      builder.Services.AddSingleton<Discord>();
      var app = builder.Build();
      var discord = app.Services.GetRequiredService<Discord>();

      var messagesApi = app.MapGroup("/messages");
      messagesApi.MapGet("/", () => discord.GetData());
      messagesApi.MapPost("/", (Message message) => discord.PostMessage(message));
      messagesApi.MapPost("/reply", (ReplyOptions reply) => discord.PostReply(reply));

      var optionsApi = app.MapGroup("/options");
      optionsApi.MapPost("/", (MessageOptions message) => discord.SendOptionsMessage(message));
      //optionsApi.MapGet("/", () => discord.GetData<DCComponent>());

      var connectApi = app.MapGroup("/connect");
      connectApi.MapPost("/", async (API_Start data) => await discord.ConnectAsync(data));

      var authUserApi = app.MapGroup("/authuser");
      authUserApi.MapGet("/{userid}", async (string userid) => await discord.Authorizeuser(userid));

      var sendDMApi = app.MapGroup("/dm");
      sendDMApi.MapPost("/", async (Dm message) => await discord.SendDM(message));

      var statusApi = app.MapGroup("/status");
      statusApi.MapGet("/", () => discord.GetStatus());

      var channelsApi = app.MapGroup("/channels");
      channelsApi.MapGet("/", () => discord.GetChannelAccess());

      var serversApi = app.MapGroup("/servers");
      serversApi.MapGet("/", () => discord.GetServerAccess());

      var cmdsApi = app.MapGroup("/commands");
      cmdsApi.MapPost("/{guildId}", (string guildId,List<Command> commands) => discord.RegisterCommandsAsync(guildId,commands));

      var logger = app.Services.GetRequiredService<ILogger<Program>>();
      app.Run();
    }
  }



  [JsonSerializable(typeof(List<Command>))]
  [JsonSerializable(typeof(ReplyOptions))]
  [JsonSerializable(typeof(PendingData))]
  [JsonSerializable(typeof(List<DCSocketChannel>))]
  [JsonSerializable(typeof(List<DCSocketMessage>))]
  [JsonSerializable(typeof(MessageOptions))]
  [JsonSerializable(typeof(ServerStatus))]
  [JsonSerializable(typeof(Message))]
  [JsonSerializable(typeof(DCButton))]
  [JsonSerializable(typeof(Dm))]
  [JsonSerializable(typeof(API_Start))]
  [JsonSerializable(typeof(DCUser))]
  [JsonSerializable(typeof(ResultError))]
  [JsonSerializable(typeof(List<DCComponent>))]
  [JsonSerializable(typeof(List<DCSocketGuild>))]
  [JsonSerializable(typeof(List<Discord.MessageSentResult>))]
  internal partial class AppJsonSerializerContext : JsonSerializerContext
  {
  }

}