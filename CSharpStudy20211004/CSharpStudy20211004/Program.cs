using System;

namespace CSharpStudy20211004
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main1();
            //Main2();
            //Main3();
            Main4();
        }

        static void Main1()
        {
            //default로 디폴트값 대입
            int i = default;
            double d = default;
            char c = default;
            string s = default;
            //Console.WriteLine($"{i},{d},{c},{s}");

            //서식지정자를 이용해서 문자열 값에 변수값 대입
            string s2 = "Hello C#";
            Console.WriteLine("##{0}##", s2);    // 자리 표시자(Placeholder), 서식지정자(format specifier)
        }

        static void Main2()
        {
            //Primitive type 과 Reference type 확인
            Console.WriteLine("int is Primitive : {0}", typeof(int).IsPrimitive);
            Console.WriteLine("string is Primitive : {0}", typeof(string).IsPrimitive);

            //문자열 리터럴
            Console.WriteLine("리터럴 사용 : {0}", "HELLO");

            //변수 선언시 동일한 변수 타입은 한번에 선언하는 것이 가능
            int a, b, c = 1;
            //위와 같이 초기화 한 경우 c만 초기화됨.
            //Console.WriteLine("a : {0}, b : {1}, c : {2}",a,b,c);

            //같은 변수형 선언시 각각 값을 초기화하는 것은 가능
            char d = 'd', e = 'e';
            Console.WriteLine("d : {0}, e : {1}", d, e);

            //같은 값을 넣을 경우
            int f, g;
            f = g = 3;
            Console.WriteLine($"f : {f}, g : {g}");

            //정수형 데이터형식에는 byte, short, int, long이 있고
            //byte는 부호가 없는 형식이다. 부호가 있는 형식은 sbyte
            //short, int, long은 부호가 있는 형식이고, 부호가 없는 형식은 ushort, uint, ulong이다.

            //숫자 구분자 사용 가능
            //숫자 구분자는 무시되므로 숫자를 표현하는데 문제가 되지 않는다.
            Console.WriteLine("숫자구분자 사용 : {0}",1_000_000);

            //Nullable 타입
            int? nullableInt = null;
            Console.WriteLine("null : {0}",nullableInt == null);

            //여러줄 문자열 저장하기 : @기호를 사용
            string multilineS = @"1st Line
2nd Line
    3rd Line";
            Console.WriteLine(multilineS);

            //문자열에 변수값 담기
            string v1 = "123";
            string v2 = "456";
            string strVar1 = string.Format("{0}{1}", v1, v2);
            Console.WriteLine(strVar1);
            Console.WriteLine($"{v1}{v2}");

            //날짜 형식
            Console.WriteLine("오늘은 {0}월 {1}일",DateTime.Now.Month, DateTime.Now.Day);
        }

        static void Main3()
        {
            while (true)
            {
                Console.Write("구구단 몇단을 하시겠습니까??");
                string inputValue = Console.ReadLine();
                if (int.TryParse(inputValue, out int dan))
                {
                    for (int i = 2; i < 10; i++)
                    {
                        Console.WriteLine($"{dan} x {i} = {dan*i}");
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("입력값이 잘못되었습니다.");
                }
            }
            
        }

        static void Main4()
        {
            //다른 진법의 숫자 다루기
            byte b = 0b1100;
            Console.WriteLine(Convert.ToString(b,16));

            //var 키워드를 사용한 변수 선언
            var v1 = 123;
            Console.WriteLine(v1.GetType());

            //튜플 리터럴 사용하기
            var tupleV = (100, 200);
            Console.WriteLine("v1 : {0}, v2 : {1}", tupleV.Item1, tupleV.Item2);
        }
    }
}
