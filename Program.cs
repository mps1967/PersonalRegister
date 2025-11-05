using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace PersonalRegister
{
    internal record Employee(string Name, int Pay);
    internal class Program
    {
        static SortedDictionary<string, int> register = new SortedDictionary<string, int>();

        static string ParseName(string raw)
        {
            var re = raw.EnumerateRunes();
            Rune[] ra = re.ToArray();
            var name = new StringBuilder();
            int len = 0;
            foreach (var r in ra)
            {
                if (len == 0)
                {
                    if (!Rune.IsLetter(r)) continue;
                }
                name.Append(r.ToString());
                if (Rune.IsLetter(r))
                {
                    len = name.Length;
                }
            }
            name.Length = len;  // cut non-letters.
            return name.ToString();
        }

        static int ParsePay(string raw)
        {
            // TODO: use CulturInfo.
            try
            {
                return int.Parse(raw);
            }
            catch (Exception _)
            {
                return -1;
            }
        }

        static Employee ParseEmployee(string line)
        {
            var re = line.EnumerateRunes();
            Rune[] ra = re.ToArray();
            // to the left of the first digit is a name.
            // after the first digit is a number.
            // the name is processed to start and end with a letter.
            // the number is processed to start and end with a digit.
            // only ascii digits are admissible.
            var name = new StringBuilder();
            var number = new StringBuilder();
            bool in_name = true;
            for(int i = 0; i < ra.Length; ++i)
            {
                if (Rune.IsDigit(ra[i]))
                {
                    in_name = false;
                }
                if (in_name)
                {
                    name.Append(ra[i]);
                }
                else
                {
                   number.Append(ra[i]);
                }
            }
            return new Employee(ParseName(name.ToString()), ParsePay(number.ToString()));

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null || line.Length == 0)
                {
                    break;
                }
                var e = ParseEmployee(line);
                if (e.Name.Length == 0)
                {
                    Console.WriteLine("name error!");
                    continue;
                }
                if (e.Pay < 0)
                {
                    Console.WriteLine("pay error!");
                    continue;
                }
                if (register.ContainsKey(e.Name))
                {
                    Console.WriteLine("duplicate name!");
                    continue;
                }
                register.Add(e.Name, e.Pay);
            }
            int total = 0;
            foreach (var n in register.Keys)
            {
                Console.Write(n);
                Console.Write(" ");
                var p = register[n];
                Console.WriteLine(p);
                total += p;
            }
            Console.Write("\n\n+++Total Pay: ");
            Console.WriteLine(total);
        }
    }
}
