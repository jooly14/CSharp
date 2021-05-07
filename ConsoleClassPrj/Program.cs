using System;
using System.Threading;

namespace ConsoleClassPrj
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 40);
            Random ran = new Random();
            ConsoleColor[] colors = {ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Gray };
            
            while (true)
            {
                Console.Clear();
                for(int i = 0; i < 30; i++)
                {
                    Console.ForegroundColor = colors[ran.Next(7)];
                    Console.SetCursorPosition(ran.Next(100), ran.Next(40));
                    Console.Write("Hello World");
                    
                }
                Thread.Sleep(300);
            }
        }
    }
}
