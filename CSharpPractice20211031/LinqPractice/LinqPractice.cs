using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqPractice
{
    class LinqPractice
    {
        static void Main(string[] args)
        {
            //Main1();
            Main2();
        }
        //List Array Class 활용
        static void Main1()
        {
            List<int> list1 = new List<int>();
            list1.Add(1);
            list1.Add(2);
            list1.Add(3);
            list1.Add(4);
            int[] arr1 = list1.ToArray();
            Array.Reverse(arr1);
            list1 = new List<int>(arr1);
            foreach (var item in list1)
            {
                Console.WriteLine(item.ToString());

            }
        }
        static void Main2()
        {
            List<int> list1 = new List<int>(new int[]{ 1, 2, 3 });
            int sum = list1.Sum();
            Console.WriteLine($"sum : {sum}");
            int count = list1.Count();
            Console.WriteLine($"count : {count}");
            double avg = list1.Average();
            Console.WriteLine($"average : {avg}");
            int max = list1.Max();
            Console.WriteLine($"max : {max}");
            int min = list1.Min();
            Console.WriteLine($"min : {min}");
            IEnumerable<int> ie = list1.Where(num => num > 2);
            foreach (var item in ie)
            {
                Console.Write(item+"\t");
            }
            Console.WriteLine();
            list1 = ie.ToList<int>();
            Console.WriteLine(list1.Count);

            IEnumerable<int> ie2 = Enumerable.Range(1,10);
            int sum2 = ie2.Where(x => x % 2 == 0).Sum();
            Console.WriteLine($"sum2 : {sum2}");
            int sum3 = ie2.Sum(x => x % 2 == 0? x : 0);
            Console.WriteLine($"sum3 : {sum3}");

            bool bln1 = ie2.Any(x => x == 2);
            Console.WriteLine(bln1? $"2 is exist": $"2 is not exist");
        }
    }
}
