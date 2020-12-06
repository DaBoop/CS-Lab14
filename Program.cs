using System;
using System.Collections.Generic;

namespace Lab14
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<int>( new int[]{ 1, 2, 3 });
            var cake = new Cake("Tiramisu", 5, 5, 100, 20, 30, 20, 20);
            CustomSerializer.BinSerialize(cake, "BinCake.txt");
            //CustomSerializer.BinSerialize(list, "Array.txt");
            var serializedCake = CustomSerializer.BinDeserialize(typeof(Cake), "BinCake.txt");
            Console.WriteLine(serializedCake);
            CustomSerializer.JSONSerialize(cake, "JsonCake.json");
            serializedCake = CustomSerializer.JSONDeserialize(typeof(Cake), "JsonCake.json");
            Console.WriteLine(serializedCake);

        }
    }
}
