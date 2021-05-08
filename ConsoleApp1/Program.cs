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

           var type= typeof(Program).Assembly.GetType("ConsoleApp1.A");
        }
    }

    public interface IA
    {
    }

    public class A : IA
    {
    }
}