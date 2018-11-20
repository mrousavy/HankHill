using System;
using System.Threading;

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