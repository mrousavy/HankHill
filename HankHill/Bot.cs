using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HankHill.WS4Net;

namespace HankHill
{
    public class Bot
    {
        public Bot()
        {
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                WebSocketProvider = WS4NetProvider.Instance
            };
            //Client = new WS4NetClient();
            Client = new DiscordSocketClient(config);
            Client.Log += Log;
            Client.MessageReceived += MessageReceived;
            Client.Ready += Ready;
            Client.JoinedGuild += GuildUpdated;
            Client.LeftGuild += GuildUpdated;

            try
            {
                Login().GetAwaiter().GetResult();
            } catch (Exception ex)
            {
                Console.WriteLine($"Could not login as Discord Bot! {ex.Message}", LogSeverity.Critical);
            }
        }

        private static async Task GuildUpdated(SocketGuild arg)
        {
            await Client.SetGameAsync($"#{Client.Guilds.Count} github.com/mrousavy/HankHill");

            Console.WriteLine($"Guild updated: \"{arg.Name}\"");
        }

        private static async Task Ready()
        {
            await Client.SetGameAsync($"#{Client.Guilds.Count} github.com/mrousavy/HankHill");

            foreach (IGuild guild in Client.Guilds)
            {
                Console.WriteLine($"Guild ready: \"{guild.Name}\"");
            }
        }

        public async Task Login()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "token");
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                throw new Exception($"Token file is empty! ({path})");
            }

            string token = File.ReadAllText(path);

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
        }

        private static async Task MessageReceived(SocketMessage messageArg)
        {
            // Don't process the command if it was a System Message
            if (!(messageArg is SocketUserMessage usermessage))
            {
                return;
            }

            if (usermessage.Author.ToString() != Client.CurrentUser.ToString())
            {
                await Log(new LogMessage(
                    LogSeverity.Info,
                    usermessage.Author.ToString(),
                    usermessage.Content));

                try
                {
                    string text = usermessage.Content.ToLower();

                    var moreJpeg = new Regex("more *(jpeg|jpg)$", RegexOptions.IgnoreCase);
                    var pixelate = new Regex("(pixel|pixelate)$", RegexOptions.IgnoreCase);
                    var nuke = new Regex("(nuke|nuking)$", RegexOptions.IgnoreCase);
                    var help = new Regex(
                        $"({Client.CurrentUser.Mention}|{Client.CurrentUser.Mention.Replace("!", "")}) *help",
                        RegexOptions.IgnoreCase);

                    //JPEG
                    if (moreJpeg.IsMatch(text))
                    {
                        EventHandler.Jpegify(usermessage.Channel);
                    }
                    //PIXELATE
                    else if (pixelate.IsMatch(text))
                    {
                        EventHandler.Pixelate(usermessage.Channel);
                    }
                    //PIXELATE
                    else if (nuke.IsMatch(text))
                    {
                        EventHandler.Nuke(usermessage.Channel);
                    }
                    //HELP
                    else if (help.IsMatch(text))
                    {
                        await usermessage.Channel.SendMessageAsync(
                            "I'm Hank Hill, I don't know what a JPEG is " +
                            "and I'm made by <@266162606161526784> (http://github.com/mrousavy/HankHill)." +
                            Environment.NewLine + "Write \"needs more jpeg\" or \"pixelate\" after sending an image.");
                    } else
                    {
                        return;
                    }

                    //"loading" emoji
                    await usermessage.AddReactionAsync(new Emoji("🤔"));
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error Handling message! ({ex.Message})");
                    try
                    {
                        await usermessage.Channel.SendMessageAsync(
                            "https://www.youtube.com/watch?v=ZXVhOPiM4mk    _(this is an error message, sorry)_");
                    } catch
                    {
                        // no messages allowed
                    }
                }
            }
        }

        private static Task Log(LogMessage message)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] [{message.Severity}] [{message.Source}] {message.Message}");
            return Task.CompletedTask;
        }

        #region Publics

        public bool StopRequested { get; set; }
        public static DiscordSocketClient Client { get; set; }

        #endregion
    }
}