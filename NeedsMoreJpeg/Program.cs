using System;
using System.Threading;

namespace NeedsMoreJpeg {
    public class Program {
        static void Main(string[] args) {
            Console.Title = "Needs More JPEG Bot";
            Console.WriteLine("Starting Needs More JPEG Bot..");

            new Bot();


            Console.WriteLine("Bot started, sleeping forever..");
            Thread.Sleep(-1);
        }
    }
}