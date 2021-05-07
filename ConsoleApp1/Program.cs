using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int> {0, 1, 2, 4, 8, 16};
            Console.WriteLine(list.Aggregate((a, b) => a | b));
        }
    }

    [Flags]
    public enum PermissionAction
    {
        Read = 1,
        Write = 2,
        Delete = 4
    }
}