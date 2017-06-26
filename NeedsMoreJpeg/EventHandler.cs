using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace NeedsMoreJpeg {
    public static class EventHandler {
        public static async void Pixelate(ISocketMessageChannel channel) {
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync().Flatten();

            foreach (IMessage message in messages) {
                foreach (IAttachment attachment in message.Attachments) {
                    if (attachment.Width != default(int?)) {
                        JpegHelper.Pixelate(attachment.Url, channel);
                        return;
                    }
                }
            }
        }

        public static async void Jpegify(ISocketMessageChannel channel) {
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync().Flatten();

            foreach (IMessage message in messages) {
                foreach (IAttachment attachment in message.Attachments) {
                    if (attachment.Width != default(int?)) {
                        JpegHelper.Jpegify(attachment.Url, channel);
                        return;
                    }
                }
            }
        }
    }
}
