using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;
using ImageSharp;
using ImageSharp.Formats;

namespace HankHill
{
    public static class ImageHelper
    {
        public static async void Jpegify(string url, ISocketMessageChannel channel)
        {
            await ProcessImage(url, channel, (image, newpath) =>
            {
                var options = new JpegEncoderOptions
                {
                    Quality = 1,
                    Subsample = JpegSubsample.Ratio420
                };
                using (var output = File.OpenWrite(newpath))
                {
                    image.SaveAsJpeg(output, options);
                }
            });
        }
        public static async void Pixelate(string url, ISocketMessageChannel channel)
        {
            await ProcessImage(url, channel, (image, newpath) =>
            {
                var options = new JpegEncoderOptions
                {
                    Quality = 60,
                    Subsample = JpegSubsample.Ratio420
                };
                using (var output = File.OpenWrite(newpath))
                {
                    Image<Rgba32> pixelatedImage = image.Pixelate(8);
                    pixelatedImage.SaveAsJpeg(output, options);
                }
            });
        }
        public static async void Nuke(string url, ISocketMessageChannel channel)
        {
            await ProcessImage(url, channel, (image, newpath) =>
            {
                var options = new JpegEncoderOptions
                {
                    Quality = 60,
                    Subsample = JpegSubsample.Ratio420
                };
                using (var output = File.OpenWrite(newpath))
                {
                    // TODO: nuke image
                    image.SaveAsJpeg(output, options);
                }
            });
        }

        public static async Task ProcessImage(string url, ISocketMessageChannel channel, Action<Image<Rgba32>, string> func)
        {
            string file = Path.GetTempFileName();
            string newpath = Path.GetTempFileName().Replace(".tmp", ".jpg");

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        using (
                            Stream contentStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync(),
                            stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 3145728,
                                true))
                        {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Couldn't download Image! ({ex.Message})");
            }

            try
            {
                using (Image<Rgba32> image = Image.Load(file))
                {
                    func(image, newpath);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Couldn't process Image! ({ex.Message})");
            }

            try
            {
                await channel.SendFileAsync(newpath);
            } catch
            {
                try
                {
                    await channel.SendMessageAsync("I can't send Images here..");
                } catch
                {
                    // can't even send messages
                }
            }

            try
            {
                File.Delete(file);
                File.Delete(newpath);
            } catch (Exception ex)
            {
                Console.WriteLine($"Couldn't cleanup! ({ex.Message})");
            }
        }
    }
}