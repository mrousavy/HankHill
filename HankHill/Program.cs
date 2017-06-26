using System;
using System.Threading;

namespace NeedsMoreJpeg {
    public class Program {
        static void Main(string[] args) {
            Console.Title = "Hank Hill Discord Bot";
            Console.WriteLine("Starting Hank Hill Bot..");

            new Bot();


            Console.WriteLine("Bot started, sleeping forever..");
            Thread.Sleep(-1);
        }
    }
}