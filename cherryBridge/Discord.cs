using Discord.WebSocket;
using Discord;
using System.Collections.Concurrent;
using cherryBridge.Models.Discord;
using cherryBridge.Models;
using cherryBridge.Models.Bridge;
using Discord.Net;
using System.Net;
using System.Threading.Channels;
using System;
using Discord.Rest;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace cherryBridge
{
  public class Discord
  {
    private readonly DiscordSocketClient client;
    private readonly ILogger<Discord> logger;
    private DCUser? authorizedUser;
    private bool findAuthUser = false;
    public record MessageSentResult(ulong MessageID, string Result);
    public bool DiscordConnected { get; set; } = false;

    readonly ConcurrentQueue<DCSocketMessage> messages = new();
    readonly ConcurrentQueue<DCComponent> selectedOptions = new();
    readonly ConcurrentDictionary<string, SocketMessageComponent> respondTo = [];

    public Discord(ILogger<Discord> logger)
    {
      var config = new DiscordSocketConfig
      {
        GatewayIntents = GatewayIntents.Guilds
        | GatewayIntents.MessageContent
        | GatewayIntents.GuildMessages
        | GatewayIntents.GuildIntegrations
        | GatewayIntents.GuildMembers
        | GatewayIntents.DirectMessages,
        MessageCacheSize = 100,
        AlwaysDownloadUsers = true,
      };

      client = new DiscordSocketClient(config);
      this.logger = logger;

      client.Log += LogAsync;
      client.ButtonExecuted += ComponentSelected;
      client.SelectMenuExecuted += ComponentSelected;
      client.Ready += ClientReady;
      client.MessageReceived += NewMessage;
      client.Disconnected += Disconnected;
      client.SlashCommandExecuted += SlashCommandExecuted;

    }

    private async Task SlashCommandExecuted(SocketSlashCommand command)
    {
      var dcCommand = new DCSocketSlashCommand(command);

      var message = DCSocketMessage.FromSlashCommand(dcCommand);
      await NewMessage(message);
      await command.RespondAsync($"> {dcCommand.BuildMessage()}", ephemeral: true);
    }

    private async Task Disconnected(Exception exception)
    {
      logger.LogError(exception, "Disconnected from Discord.");
      DiscordConnected = false;
    }

    public PendingData GetData()
    {
      var pending = new PendingData
      {
        Messages = GetData<DCSocketMessage>(),
        Options = GetData<DCComponent>(),
      };

      return pending;
    }

    public List<T> GetData<T>()
    {
      var newData = new List<T>();

      ConcurrentQueue<T>? que;

      if (typeof(T) == typeof(DCSocketMessage))
      {
        que = messages as ConcurrentQueue<T>;
      }
      else if (typeof(T) == typeof(DCComponent))
      {
        que = selectedOptions as ConcurrentQueue<T>;
      }
      else
      {
        return newData;
      }

      if (que == null)
        return newData;

      while (!que.IsEmpty)
      {
        if (que.TryDequeue(out var item))
        {
          newData.Add(item);
        }
      }

      return newData;
    }

    private async Task NewMessage(SocketMessage message)
    {
      if (message.Author.IsBot)
        return;

      await NewMessage(new DCSocketMessage(message));
      await message.AddReactionAsync(new Emoji("👍"));
    }
    private async Task NewMessage(DCSocketMessage message)
    {
      messages.Enqueue(message);

      if (message.Channel != null && message.Channel.Name != null)
        Console.WriteLine($"Channel: {message.Channel.Name}");


      if (message.Author != null)
      {
        Console.WriteLine("DM:");
        Console.WriteLine($"[{message.Timestamp}: Author: {message.Author.Username} - {message.Content}]");
      }

    }
    public async Task<Microsoft.AspNetCore.Http.IResult> Authorizeuser(string userID)
    {
      if (ulong.TryParse(userID, out ulong longUserID))
      {
        authorizedUser = new DCUser(await client.GetUserAsync(longUserID));
        findAuthUser = authorizedUser != null;

        return findAuthUser ? Results.Ok(authorizedUser) : Results.NotFound("User not found");
      }
      return Results.BadRequest("Invalid User ID");
    }

    private async Task ClientReady()
    {
      logger.LogInformation("Connected to Discord.");
      DiscordConnected = true;
    }

    private async Task ComponentSelected(SocketMessageComponent component)
    {
      Console.WriteLine("Component Stored");
      selectedOptions.Enqueue(new DCComponent(component));
      respondTo.AddOrUpdate(component.Data.CustomId, component, (key, oldValue) => component);
      await component.DeferAsync();
    }

    public async Task<ResultError?> ConnectAsync(API_Start data)
    {
      if (!DiscordConnected)
      {

        try
        {
          await client.LoginAsync(TokenType.Bot, data.BotToken);
          await client.StartAsync();
        }
        catch (HttpException ex)
        {
          return new ResultError { Error = ex.Message, ErrorCode = (int)ex.HttpCode, ErrorTag = ex.HttpCode.ToString() };
        }


      }

      return null;
    }

    private Task LogAsync(LogMessage log)
    {

      logger.LogInformation(log.ToString());
      return Task.CompletedTask;

    }

    internal async Task<Microsoft.AspNetCore.Http.IResult> SendOptionsMessage(MessageOptions option_message)
    {

      if (ulong.TryParse(option_message.SendId, out ulong sendID))
      {
        switch (option_message.IdType)
        {
          case "channel":
            SocketChannel baseChannel;
            baseChannel = client.GetChannel(sendID);
            if (baseChannel is not IMessageChannel channel)
            {
              return Results.NotFound($"Channel {option_message.SendId} not found");
            }

            Console.WriteLine($"Sending options to messageChannel: {channel.Name}");

            var components = DCButton.Build(option_message.Buttons);
            for (int i = 0; i < components.Count; i++)
            {
              if (i == 0)
                await channel.SendMessageAsync(option_message.Content, components: components[i]);
              else
                await channel.SendMessageAsync("Continued...", components: components[i]);

            }

            return Results.Ok("Content Sent");

          case "user":
            var user = client.GetUser(sendID);
            if (user != null)
            {
              var dmChannel = await user.CreateDMChannelAsync();

              var dmComponents = DCButton.Build(option_message.Buttons);
              foreach (var component in dmComponents)
                await dmChannel.SendMessageAsync(option_message.Content, components: component);

              return Results.Ok("Content Sent");
            }
            else
            {
              return Results.NotFound("User not found");
            }

          default:
            Results.NotFound("Unknown Message Type");
            break;
        }

      }
      return Results.BadRequest("Channel ID is not valid.");
    }

    internal ServerStatus GetStatus()
    {
      return new ServerStatus
      {
        DiscordConnected = DiscordConnected,
        Version = "v0.1"
      };
    }
    internal async Task<Microsoft.AspNetCore.Http.IResult> GetChannelAccess()
    {
      var channels = new List<DCSocketChannel>();


      foreach (var guild in client.Guilds)
      {
        await guild.DownloadUsersAsync();

        var botUser = guild.CurrentUser;

        foreach (var channel in guild.Channels)
        {
          var permissions = botUser.GetPermissions(channel);

          if (permissions.ViewChannel && permissions.SendMessages)
          {
            channels.Add(new DCSocketChannel(channel));

            Console.WriteLine($"Bot has access to messageChannel {channel.Name} in guild {guild.Name}");
          }
        }
      }

      return channels.Count != 0 ? Results.Ok(channels) : Results.NotFound("The bot doesn't seem to have access to any channels.");
    }
    internal async Task<Microsoft.AspNetCore.Http.IResult> PostMessage(Message message)
    {
      SocketChannel baseChannel;
      if (ulong.TryParse(message.ChannelID, out ulong channelID))
      {
        baseChannel = client.GetChannel(channelID);
      }
      else
      {
        return Results.BadRequest("Channel ID is not valid.");
      }


      if (baseChannel is IMessageChannel messageChannel)
      {
        Console.WriteLine($"Sending to dmChannel: {messageChannel.Name}");
        Console.WriteLine($"Content: {message.Content}");
        var result = await messageChannel.SendMessageAsync(text: message.Content);

        return result != null ? Results.Ok(new MessageSentResult(result.Id, "Content sent successfully.")) : Results.NotFound("Issue sending message");

      }
      else if (baseChannel is IDMChannel dmChannel)
      {
        Console.WriteLine($"Sending DM to: {dmChannel.Recipient.Username}");
        Console.WriteLine($"Content: {message.Content}");
        var result = await dmChannel.SendMessageAsync(text: message.Content);

        return result != null ? Results.Ok(new MessageSentResult(result.Id, "Content sent successfully.")) : Results.NotFound("Issue sending message");
      }

      return Results.NotFound($"Channel {message.ChannelID} not found");
    }

    internal List<DCSocketGuild> GetServerAccess()
    {
      var servers = new List<DCSocketGuild>();
      foreach (var guild in client.Guilds)
      {
        servers.Add(new DCSocketGuild(guild));
      }

      return servers;
    }

    internal async Task<IResult> SendDM(Dm message)
    {
      if (ulong.TryParse(message.UserID, out ulong longUserID))
      {
        var user = client.GetUser(longUserID);
        if (user != null)
        {
          var dmChannel = await user.CreateDMChannelAsync();
          await dmChannel.SendMessageAsync(message.Content);
          return Results.Ok("Content Sent");
        }
        else
        {
          Results.NotFound("User not found");
        }
      }

      return Results.BadRequest("Invalid User ID");
    }

    internal async Task<IResult> PostReply(ReplyOptions options)
    {
      if (respondTo.Remove(options.CustomID, out var component))
      {
        if (options.Buttons.Count != 0)
        {
          var components = DCButton.Build(options.Buttons);
          foreach (var buttonComponent in components)
            await component.FollowupAsync(options.Content, components: buttonComponent);

          return Results.Ok();
        }
        else
        {
          await component.FollowupAsync(options.Content);
          return Results.Ok();
        }
      }
      return Results.NotFound("Component not found");
    }

    public async Task<IResult> RegisterCommandsAsync(string guildId, List<Command> commands)
    {
      Console.WriteLine($"Registering commands for guild: {guildId}");

      if (ulong.TryParse(guildId, out ulong guildid))
      {
        var guild = client.GetGuild(guildid);

        if (guild == null)
          return Results.NotFound("Guild not found");

        var guidCmds = await guild.GetApplicationCommandsAsync();
        foreach (var cmd in guidCmds)
        {
          var found = commands.Find(c => c.Name == cmd.Name);
          if (found == null)
          {
            Console.WriteLine($"Deleting command: {cmd.Name}");
            await cmd.DeleteAsync();
          }
          else
          {
            if (found.Description != cmd.Description)
            {
              await cmd.DeleteAsync();
              await guild.CreateApplicationCommandAsync(found.Build());
              Console.WriteLine($"Updating command: {cmd.Name}");
            }
          }
        }

        var gcmds = guidCmds.ToList();
        //var newCmdMessage = new StringBuilder("New Commands Registered:");


        //bool newCOmmands = false;
        foreach (var cmd in commands)
        {
          var found = gcmds.Find(c => c.Name == cmd.Name);
          if (found == null)
          {
            //newCOmmands = true;
            await guild.CreateApplicationCommandAsync(cmd.Build());
            Console.WriteLine($"Creating command: {cmd.Name}");
            //newCmdMessage.Append($"/{cmd.Name} - {cmd.Description}");
          }
        }
      }

      //if (newCOmmands)
      //{

      //}
      return Results.Ok();
    }
  }
}