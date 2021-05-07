using System;
using System.IO;

namespace BinaryReaderPrj
{
    class Program
    {
        static void Main(string[] args)
        {
            int var1;
            float var2;
            string str1;
            using (BinaryReader br = new BinaryReader(File.Open("test.dat", FileMode.Open)))
            {
                var1 = br.ReadInt32();
                var2 = br.ReadSingle();
                str1 = br.ReadString();
            }
            Console.WriteLine("{0} {1} {2}",var1,var2,str1);
        }
    }
}
