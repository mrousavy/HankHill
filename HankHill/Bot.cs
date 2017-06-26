using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
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
                LogLevel = LogSeverity.Info
            };
            //Client = new WS4NetClient();
            Client = new DiscordSocketClient(config);
            Client.Log += Log;
            Client.MessageReceived += MessageReceived;

            try {
                Login().GetAwaiter().GetResult();
            } catch (Exception ex) {
                Console.WriteLine($"Could not login as Discord Bot! {ex.Message}", LogSeverity.Critical);
            }
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
                    await usermessage.AddReactionAsync(new Emoji(""));

                    string text = usermessage.Content.ToLower();

                    //JPEG
                    if (text.Contains("needs more jpeg") ||
                        text.Contains(Client.CurrentUser.Mention.ToLower()) ||
                        text.Contains(Client.CurrentUser.Mention.Replace("!", "")) ||
                        text.Contains("needsmorejpeg") ||
                        text.Contains("more jpeg") ||
                        text.Contains("morejpeg")) {
                        EventHandler.Jpegify(usermessage.Channel);
                    }
                    //PIXELATE
                    else if (text.Contains("pixelate") ||
                             text.Contains("pixel")) {
                        EventHandler.Pixelate(usermessage.Channel);
                    }
                } catch {
                    await usermessage.Channel.SendMessageAsync("https://www.youtube.com/watch?v=ZXVhOPiM4mk    _(this is an error message, sorry)_");
                }
            }
        }

        private static Task Log(LogMessage message) {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] [{message.Severity}] [{message.Source}] {message.Message}");
            return Task.CompletedTask;
        }
    }
}
