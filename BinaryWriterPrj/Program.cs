using System;
using System.IO;

namespace BinaryWriterPrj
{
    class Program
    {
        static void Main(string[] args)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream("test.dat", FileMode.Create)))
            {
                bw.Write(12);
                bw.Write(3.14f);
                bw.Write("Hello World!!");
            }
        }
    }
}
