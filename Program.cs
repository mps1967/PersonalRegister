using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;

namespace PersonalRegister
{
    internal class RawEmployee
    {
        // Assumptions:
        // - the name contains no digits.
        // - the first digit starts the number. Hence numbers will always be positive.
        // - the name ends with the last letter before the number.
        // - all chars before the first letter are ignored.
        // - the whole sequence from the first digit on constitutes the number.
        public RawEmployee(string raw)
        {
            ra_ = raw.EnumerateRunes().ToArray();
            // find pay_begin_
            for (crt_ = 0; crt_ < ra_.Length; crt_++)
            {
                if (is_digit())
                {
                    pay_begin_ = crt_;
                    break;
                }
            }
            // find name_begin_
            for (crt_ = 0; crt_ < pay_begin_; ++crt_)
            {
                if (is_letter())
                {
                    name_begin_ = crt_;
                    break;
                }
            }
            // find name_end_
            for (crt_ = pay_begin_ - 1; crt_ >= name_begin_; --crt_)
            {
                if (is_letter())
                {
                    name_end_ = crt_ + 1;
                    break;
                }
            }
            crt_ = name_end_ - 1;
            Debug.Assert(is_letter());
            // make name_
            for (crt_ = name_begin_; crt_ < name_end_; ++crt_) name_.Append(r().ToString());
            pay_end_ = raw.Length;
            // find pay_end
            for (crt_ = pay_begin_; crt_ < ra_.Length; ++crt_)
            {
                int num = -1;
                pay_.Append(r().ToString());
                try
                {
                    num = int.Parse(pay_.ToString());
                }
                catch (Exception)
                {
                    num = -1;
                }
                if (num >= 0)
                {
                    pay_end_ = crt_ + 1;
                }
            }
            pay_.Length = pay_end_ - pay_begin_;
        }
        private Rune r() { return ra_[crt_]; }
        private bool is_letter() { return Rune.IsLetter(r()); }
        private bool is_digit() { return Rune.IsDigit(r()); }

        private Rune[] ra_;
        private StringBuilder name_ = new StringBuilder();
        private StringBuilder pay_ = new StringBuilder();
        private int crt_ = -1;
        private int name_begin_= -1;
        private int name_end_ = -1;
        private int pay_begin_ = -1;
        private int pay_end_ = -1;

        public static RawEmployee? New(string raw)
        {
            var re = new RawEmployee(raw);
            if (re.Name.Length == 0) return null;
            if (re.Pay.Length == 0) return null;
            return re;
        }
        public string Name { get { return name_.ToString(); } }
        public string Pay { get { return pay_.ToString(); } }
    }
    internal record Employee(string Name, int Pay)
    {
        public static Employee? New(string raw)
        {
            var re = RawEmployee.New(raw);
            if (re == null) return null;
            int pay = -1;
            try
            {
                pay = int.Parse(re.Pay);
            }
            catch (Exception)
            {
                return null;
            }
            return new Employee(re.Name, pay);
        }
    }

    internal class Program
    {
        static SortedDictionary<string, int> register = new SortedDictionary<string, int>();

        public static string FirstChar(string s)
        {
            var re = s.EnumerateRunes();
            foreach (var r in re)
            {
                if (!r.IsAscii) return string.Empty;
                return r.ToString().ToUpper();
            }
            return string.Empty;
        }

        public static string Menu()
        {
            while(true)
            {
                Console.WriteLine("+ Add Employee as: name pay");
                Console.WriteLine("L List");
                Console.WriteLine("Q Quit");
                var line = Console.ReadLine() ?? string.Empty;
                var choice = FirstChar(line);
                switch (choice)
                {
                    case "+":
                    case "L":
                    case "Q":
                        return choice;
                    default:
                        Console.WriteLine("+LQ accepted only!");
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var choice = string.Empty;
            while (choice != "Q")
            {
                choice = Menu();
                switch (choice)
                {
                    case "Q":
                        break;
                    case "+":
                        var line = Console.ReadLine() ?? string.Empty;
                        var e = Employee.New(line);
                        if (e == null)
                        {
                            Console.WriteLine("invalid employee input!");
                            continue;
                        }
                        if (register.ContainsKey(e.Name))
                        {
                            Console.WriteLine("duplicate employee"!);
                            continue;
                        }
                        register.Add(e.Name, e.Pay);
                        break;
                    case "L":
                        int size = register.Count();
                        int total = 0;
                        Console.WriteLine($"{size} employees");
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
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
