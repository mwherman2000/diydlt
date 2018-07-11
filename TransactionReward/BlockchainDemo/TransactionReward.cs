using Newtonsoft.Json;
using System;

namespace BlockchainDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;

            Blockchain phillyCoin = new Blockchain();
            phillyCoin.CreateTransaction(new Transaction("Henry", "MaHesh", 10));
            phillyCoin.ProcessPendingTransactions("Bill");

            phillyCoin.CreateTransaction(new Transaction("MaHesh", "Henry", 5));
            phillyCoin.CreateTransaction(new Transaction("MaHesh", "Henry", 5));
            phillyCoin.ProcessPendingTransactions("Bill");

            var endTime = DateTime.Now;

            Console.WriteLine($"Duration: {endTime - startTime}");

            Console.WriteLine("=========================");
            Console.WriteLine($"Henry' balance: {phillyCoin.GetBalance("Henry")}");
            Console.WriteLine($"MaHesh' balance: {phillyCoin.GetBalance("MaHesh")}");
            Console.WriteLine($"Bill' balance: {phillyCoin.GetBalance("Bill")}");

            Console.WriteLine("=========================");
            Console.WriteLine($"phillyCoin");
            Console.WriteLine(JsonConvert.SerializeObject(phillyCoin, Formatting.Indented));

            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }
    }
}
