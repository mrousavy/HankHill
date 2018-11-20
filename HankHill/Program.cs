using System;
using System.IO;
using System.Threading;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.PixelFormats;
using SixLabors.Primitives;

namespace HankHill
{
    public class Program
    {
        private static void Main()
        {
            Console.Title = "Hank Hill Discord Bot";
            Console.WriteLine("Starting Hank Hill Bot..");

            var _ = new Bot();

            Console.WriteLine("Bot started, sleeping forever..");
            Thread.Sleep(-1);
        }
    }
}