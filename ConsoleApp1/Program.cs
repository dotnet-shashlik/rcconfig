using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shashlik.Utils.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<int> ids = new HashSet<int>();
            for (int i = 0; i < 100000; i++)
            {
                T1(ids).GetAwaiter().GetResult();
            }

            Console.WriteLine(ids.Count);
        }

        static async Task T1(HashSet<int> ids)
        {
            ids.Add(Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(1);
        }
    }
}