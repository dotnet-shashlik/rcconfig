using System;
using Shashlik.Utils.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "3";

           var action =  s.ParseTo<PermissionAction>();
           Console.WriteLine(action);
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