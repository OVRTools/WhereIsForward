using System;

namespace WhereIsForward
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new WhereIsForwardApp();

            Console.WriteLine("Hello gamer. You now have an arrow. Congratulation! :) Press enter to exit.");
            Console.ReadLine();

            app.Shutdown();
        }
    }
}
