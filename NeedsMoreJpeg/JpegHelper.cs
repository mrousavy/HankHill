using ImageSharp;
using ImageSharp.Formats;
using System.IO;
using System.Net.Http;

namespace NeedsMoreJpeg {
    public static class JpegHelper {
        public static async void Jpegify(string url, Discord.WebSocket.ISocketMessageChannel channel) {
            string file = Path.GetTempFileName();
            string jpegified = Path.GetTempFileName();

            using (HttpClient client = new HttpClient()) {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url)) {
                    using (
                        Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true)) {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }

            using (Image<Rgba32> image = Image.Load(file)) {
                JpegEncoderOptions options = new JpegEncoderOptions {
                    Quality = 50
                };
                using (FileStream output = File.OpenWrite(jpegified)) {
                    image.SaveAsJpeg(output, options);
                }
            }

            await channel.SendFileAsync(jpegified);
        }
    }
}
