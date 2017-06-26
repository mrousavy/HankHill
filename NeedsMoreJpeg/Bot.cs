using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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


                string text = usermessage.Content.ToLower();
                // Determine if the message is a command by checking for all prefixes
                if (!(
                    text.StartsWith("needs more jpeg") ||
                    text.StartsWith(Client.CurrentUser.Mention.ToLower()) ||
                    text.StartsWith("needsmorejpeg") ||
                    text.StartsWith("more jpeg") ||
                    text.StartsWith("morejpeg")
                    ))
                    return;

                ISocketMessageChannel channel = usermessage.Channel;
                IEnumerable<IMessage> messages = await channel.GetMessagesAsync().Flatten();

                try {
                    foreach (IMessage message in messages) {
                        foreach (IAttachment attachment in message.Attachments) {
                            if (attachment.Width != default(int?)) {
                                JpegHelper.Jpegify(attachment.Url, channel);
                                return;
                            }
                        }
                    }
                } catch {
                    await channel.SendMessageAsync("https://www.youtube.com/watch?v=ZXVhOPiM4mk    (this is an error message, sorry)");
                }
            }
        }

        private static Task Log(LogMessage message) {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] [{message.Severity}] [{message.Source}] {message.Message}");
            return Task.CompletedTask;
        }
    }
}
