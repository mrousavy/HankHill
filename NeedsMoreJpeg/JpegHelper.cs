using System.IO;
using System.Net.Http;

namespace NeedsMoreJpeg {
    public static class JpegHelper {
        public static async void Jpegify(string url, Discord.IMessageChannel channel) {
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

            using (FileStream stream = File.OpenRead("foo.jpg"))
            using (FileStream output = File.OpenWrite("bar.jpg")) {
                Image image = new Image(stream);
                image.Resize(image.Width / 2, image.Height / 2)
                     .Grayscale()
                     .Save(output);
            }

            await channel.SendFileAsync(jpegified);
        }
    }
}
