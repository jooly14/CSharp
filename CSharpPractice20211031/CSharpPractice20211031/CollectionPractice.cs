using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpPractice20211031
{
    class CollectionPractice
    {
        static void Main(string[] args)
        {
            //Main1();
            //Main2();
            //Main3();
            Main4();
        }

        //LIst Class
        static void Main1()
        {
            List<String> colors = new List<string>();
            colors.Add("Red");
            colors.Add("Green");
            colors.Add("Blue");
            for (int i = 0; i < colors.Count; i++)
            {
                Console.WriteLine(colors[i]);
            }
        }
        //Enumerable Class
        static void Main2()
        {
            var numbers = Enumerable.Range(1,5);
            foreach (var item in numbers)
            {
                Console.Write($"{item}\t");
            }
            Console.WriteLine();
        }

        //Dictionary Class
        static void Main3()
        {
            var data = new Dictionary<string, string>();
            data.Add("Eng","Hello");
            data.Add("Kor", "안녕");
            foreach (var item in data)
            {
                Console.WriteLine($"Key : {item.Key} / Value : {item.Value}");
            }
        }

        // Nullable Type
        static void Main4()
        {
            int? nullableInt = null;
            int intValue = nullableInt ?? default; // ?? null값을 가지고 있는 경우 오른쪽 값을 반환
            Console.WriteLine($"{intValue}");

            var retval = nullableInt?.CompareTo(1) ?? -1; // ? 해당 변수의 값이 null인 경우 오른쪽 연산을 실행하지 않고
                                                          // null 값을 반환한다. 이 경우 ?? 연산자를 사용하여 반환값이
                                                          // null인 경우 정해진 값을 반환하도록 할 수 있다.
                                                          // 그런 점에서 ?연산자와 ??연산자를 함께 사용하면 좋다.
            Console.WriteLine(retval);
        }
    }
}
