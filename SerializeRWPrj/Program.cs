using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SerializeRWPrj
{
    [Serializable]
    struct Data
    {
        public int var1;
        public float var2;
        public string str1;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Data[] Datas = new Data[2];
            Datas[0].var1 = 13;
            Datas[0].var2 = 3.14f;
            Datas[0].str1 = "hello";

            Datas[1].var1 = 20;
            Datas[1].var2 = 1.5f;
            Datas[1].str1 = "world";

            using(FileStream fs = new FileStream("test.dat", FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs,Datas);
            }

            Data[] ResultData;

            using(FileStream fs2 = new FileStream("test.dat", FileMode.Open))
            {
                BinaryFormatter formatter2 = new BinaryFormatter();
                ResultData = (Data[])formatter2.Deserialize(fs2);
            }

            for(int i = 0; i < ResultData.Length; i++)
            {
                Console.WriteLine("{0} {1} {2}", ResultData[i].var1, ResultData[i].var2, ResultData[i].str1);
            }
        }
    }
}
