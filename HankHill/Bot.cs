using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeedsMoreJpeg {
    public class Bot {
        #region Privates

        private CommandService Service { get; }

        #endregion

        #region Publics

        public bool StopRequested { get; set; }
        public static DiscordSocketClient Client { get; set; }

        #endregion

        public Bot() {
            DiscordSocketConfig config = new DiscordSocketConfig {
                LogLevel = LogSeverity.Info,
                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance
            };
            //Client = new WS4NetClient();
            Client = new DiscordSocketClient(config);
            Client.Log += Log;
            Client.MessageReceived += MessageReceived;
            Client.Ready += Ready;
            Client.JoinedGuild += JoinedGuild;

            try {
                Login().GetAwaiter().GetResult();
            } catch (Exception ex) {
                Console.WriteLine($"Could not login as Discord Bot! {ex.Message}", LogSeverity.Critical);
            }
        }

        private async Task JoinedGuild(SocketGuild arg) {
            await Client.SetGameAsync($"github.com/mrousavy/HankHill | {Client.Guilds.Count} Guilds");
        }

        private async Task Ready() {
            await Client.SetGameAsync($"github.com/mrousavy/HankHill | {Client.Guilds.Count} Guilds");
        }

        public async Task Login() {
            string path = Path.Combine(AppContext.BaseDirectory, "token");
            if (!File.Exists(path)) {
                File.Create(path).Dispose();
                throw new Exception($"Token file is empty! ({path})");
            }
            string token = File.ReadAllText(path);

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
        }

        private async Task MessageReceived(SocketMessage messageArg) {
            // Don't process the command if it was a System Message
            SocketUserMessage usermessage = messageArg as SocketUserMessage;
            if (usermessage == null)
                return;

            if (usermessage.Author.ToString() != Client.CurrentUser.ToString()) {
                await Log(new LogMessage(
                    LogSeverity.Info,
                    usermessage.Author.ToString(),
                    usermessage.Content));

                try {
                    string text = usermessage.Content.ToLower();

                    Regex moreJpeg = new Regex("more *(jpeg|jpg)$", RegexOptions.IgnoreCase);
                    Regex pixelate = new Regex("(pixel|pixelate)$", RegexOptions.IgnoreCase);
                    Regex help = new Regex($"({Client.CurrentUser.Mention}|{Client.CurrentUser.Mention.Replace("!", "")}) *help", RegexOptions.IgnoreCase);

                    //JPEG
                    if (moreJpeg.IsMatch(text)) {
                        EventHandler.Jpegify(usermessage.Channel);
                    }
                    //PIXELATE
                    else if (pixelate.IsMatch(text)) {
                        EventHandler.Pixelate(usermessage.Channel);
                    }
                    //HELP
                    else if (help.IsMatch(text)) {
                        await usermessage.Channel.SendMessageAsync(
                            "I'm Hank Hill, I don't know what a JPEG is " +
                            "and I'm made by <@266162606161526784> (http://github.com/mrousavy/HankHill)." +
                            Environment.NewLine + "Write \"needs more jpeg\" or \"pixelate\" after sending an image.");
                    } else {
                        return;
                    }

                    //"loading" emoji
                    await usermessage.AddReactionAsync(new Emoji("🤔"));
                } catch (Exception ex) {
                    Console.WriteLine($"Error Handling message! ({ex.Message})");
                    try {
                        await usermessage.Channel.SendMessageAsync("https://www.youtube.com/watch?v=ZXVhOPiM4mk    _(this is an error message, sorry)_");
                    } catch {
                        // no messages allowed
                    }
                }
            }
        }

        private static Task Log(LogMessage message) {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] [{message.Severity}] [{message.Source}] {message.Message}");
            return Task.CompletedTask;
        }
    }
}
