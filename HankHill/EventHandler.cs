using System;
using Discord;
using Discord.WebSocket;

namespace HankHill
{
    public static class EventHandler
    {
        public static async void Pixelate(ISocketMessageChannel channel)
        {
            try
            {
                var messages = await channel.GetMessagesAsync().Flatten();

                foreach (var message in messages)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment.Width > 0)
                        {
                            ImageHelper.Pixelate(attachment.Url, channel);
                            return;
                        }
                    }
                }

                await channel.SendMessageAsync(
                    "Couldn't find an Image that needs to be pixelated!");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unknown Error while pixelating! {ex.Message}");
            }
        }

        public static async void Jpegify(ISocketMessageChannel channel)
        {
            try
            {
                var messages = await channel.GetMessagesAsync().Flatten();

                foreach (var message in messages)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment.Width > 0)
                        {
                            ImageHelper.Jpegify(attachment.Url, channel);
                            return;
                        }
                    }
                }

                await channel.SendMessageAsync(
                    "Couldn't find an Image that needs more jpeg");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unknown Error while jpegifying! {ex.Message}");
            }
        }

        public static async void Nuke(ISocketMessageChannel channel)
        {
            try
            {
                var messages = await channel.GetMessagesAsync().Flatten();

                foreach (var message in messages)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment.Width > 0)
                        {
                            ImageHelper.Nuke(attachment.Url, channel);
                            return;
                        }
                    }
                }

                await channel.SendMessageAsync(
                    "Couldn't find an Image that needs to be nuked");
            } catch (Exception ex)
            {
                Console.WriteLine($"Unknown Error while nuking! {ex.Message}");
            }
        }
    }
}