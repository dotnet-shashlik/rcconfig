using System;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            using var rsa = RSA.Create(2048);
            var key = rsa.ExportRSAPrivateKey();
            var k = Convert.ToBase64String(key);
            Console.WriteLine(k);
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