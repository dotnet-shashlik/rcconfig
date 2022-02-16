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
    public class EventQueue
    {
        public static readonly BlockingCollection<string> Queues = new(1);

        public static string Wait(CancellationToken cancellationToken)
        {
            try
            {
                if (Queues.TryTake(out var file, 3 * 1000, cancellationToken))
                    return file;
                return string.Empty;
            }
            catch (OperationCanceledException)
            {
                return string.Empty;
            }
        }
    }

    class Program
    {
        private static string CreatePassword(int len)
        {
            string[] valid = { "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "1234567890", "!@#$%^&*()_+" };
            byte[] random = RandomNumberGenerator.GetBytes(len);
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                var a = random[i];
                var b = a % 4;
                res.Append(valid[b][a % valid[b].Length]);
            }

            return res.ToString();
        }

        private static string CreatePassword1(int len)
        {
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=";
            byte[] random = RandomNumberGenerator.GetBytes(len);
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                var a = random[i];
                res.Append(valid[a % valid.Length]);
            }

            return res.ToString();
        }

        private static string CreatePassword2(int len)
        {
            //note: added + and / chars. could be any of them
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=";
            using var crypt = RandomNumberGenerator.Create();
            var sb = new StringBuilder();
            var buf = new byte[len]; //length: should be larger
            crypt.GetBytes(buf); //get the bytes

            foreach (byte b in buf)
            {
                sb.Append(valid[b % valid.Length]);
            }

            ;


            return sb.ToString();
        }

        public static String random(int count, int start, int end, bool letters, bool numbers, char[] chars)
        {
            if (count == 0)
            {
                return "";
            }
            else if (count < 0)
            {
                throw new Exception("Requested random string length " + count + " is less than 0.");
            }
            else if (chars != null && chars.Length == 0)
            {
                throw new Exception("The chars array must not be empty");
            }
            else
            {
                if (start == 0 && end == 0)
                {
                    if (chars != null)
                    {
                        end = chars.Length;
                    }
                    else if (!letters && !numbers)
                    {
                        end = 1114111;
                    }
                    else
                    {
                        end = 123;
                        start = 32;
                    }
                }
                else if (end <= start)
                {
                    throw new Exception("Parameter end (" + end + ") must be greater than start (" + start + ")");
                }

                //int zero_digit_ascii = true;
                //int first_letter_ascii = true;
                if (chars == null && (numbers && end <= 48 || letters && end <= 65))
                {
                    throw new Exception("Parameter end (" + end + ") must be greater then (" + 48 +
                                        ") for generating digits or greater then (" + 65 + ") for generating letters.");
                }
                else
                {
                    StringBuilder builder = new StringBuilder(count);
                    int gap = end - start;

                    while (true)
                    {
                        while (count-- != 0)
                        {
                            int codePoint;
                            if (chars == null)
                            {
                                codePoint = RandomNumberGenerator.GetInt32(gap) + start;
                                switch (char.GetUnicodeCategory((char)codePoint))
                                {
                                    case System.Globalization.UnicodeCategory.OtherNotAssigned:
                                    case System.Globalization.UnicodeCategory.PrivateUse:
                                    case System.Globalization.UnicodeCategory.Surrogate:
                                        ++count;
                                        continue;
                                }
                            }
                            else
                            {
                                codePoint = chars[RandomNumberGenerator.GetInt32(gap) + start];
                            }


                            int numberOfChars = codePoint >= 0x010000 ? 2 : 1;
                            if (count == 0 && numberOfChars > 1)
                            {
                                ++count;
                            }
                            else if ((!letters || !char.IsLetter((char)codePoint)) && (!numbers || !char.IsDigit((char)codePoint)) &&
                                     (letters || numbers))
                            {
                                ++count;
                            }
                            else
                            {
                                builder.Append((char)codePoint);
                                if (numberOfChars == 2)
                                {
                                    --count;
                                }
                            }
                        }

                        return builder.ToString();
                    }
                }
            }
        }

        public static String random(int count, string chars)
        {
            return random(count, 0, chars.Length, false, false, chars.ToCharArray());
        }

        public static String random(int count)
        {
            return random(count, 0, 0, true, true, null);
        }

        static void Main(string[] args)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100 * 1000);
            Console.WriteLine("start: " + DateTime.Now);
            var res = EventQueue.Wait(cancellationTokenSource.Token);
            Console.WriteLine("end: " + DateTime.Now);
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