using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
           var r= Per.R | Per.R | Per.W|Per.W|Per.RW;
           Console.WriteLine(r);
        }
    }

    [Flags]
    public enum Per
    {
        R = 1,
        W = 2,
        RW = 4
    }
}