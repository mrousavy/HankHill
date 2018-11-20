using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;
using ImageSharp;
using ImageSharp.Formats;
using SixLabors.Primitives;

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
                    var pixelatedImage = image.Pixelate(8);
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
                    Quality = 20,
                    Subsample = JpegSubsample.Ratio420
                };
                using (var output = File.OpenWrite(newpath))
                {
                    var bounds = new Rectangle((int) (image.Width * 0.2), (int) (image.Height * 0.2), (int) (image.Width * 0.6), (int) (image.Height * 0.6));
                    image.Glow(Rgba32.Violet, (int) (image.Width * 0.5), bounds);
                    image.BoxBlur(2);
                    image.Vignette((int) (image.Width * 0.5), (int) (image.Height * 0.5));
                    image.Saturation(100);
                    image.Quantize(Quantization.Octree, 64);
                    image.Contrast(10);
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
                using (var image = Image.Load(file))
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