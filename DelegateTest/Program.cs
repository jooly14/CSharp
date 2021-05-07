using System;

namespace DelegateTest
{
    delegate void Del();
    class Program
    {
        public static void Print()
        {
            Console.WriteLine("대리자를 통한 메서드 호출");
        }   

        public static void Print2()
        {
            Console.WriteLine("하나 더 참조");
        }
        public static void Main()
        {
            Del myDel = new Del(Print);
            myDel += Print2;
            myDel();
        }
    }
}
