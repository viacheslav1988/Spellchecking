using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Spellchecking
{
    class Program
    {
        private static int Levenshtein(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return !string.IsNullOrEmpty(b) ? b.Length : 0;
            if (string.IsNullOrEmpty(b)) return !string.IsNullOrEmpty(a) ? a.Length : 0;
            var с = new int[a.Length + 1, b.Length + 1];
            for (var i = 0; i <= с.GetUpperBound(0); ++i) с[i, 0] = i;
            for (var i = 0; i <= с.GetUpperBound(1); ++i) с[0, i] = i;
            for (var i = 1; i <= с.GetUpperBound(0); ++i)
            {
                for (var j = 1; j <= с.GetUpperBound(1); ++j)
                {
                    var cost = Convert.ToInt32(a[i - 1] != b[j - 1]);
                    var min1 = с[i - 1, j] + 1;
                    var min2 = с[i, j - 1] + 1;
                    var min3 = с[i - 1, j - 1] + cost;
                    с[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return с[с.GetUpperBound(0), с.GetUpperBound(1)];
        }

        private static async void Verification(string word, string fileName, int coefficient)
        {
            try
            {
                var dic = new Dictionary<string, int>();
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"Dictionary\", fileName), System.Text.Encoding.UTF8))
                {
                    Console.WriteLine(@"...Анализирую...");
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        var result = Levenshtein(word.ToLower(), line);
                        if (0 <= result && result <= coefficient) dic[line] = result;
                    }
                }
                Console.Clear();
                if (dic.Any(x => x.Value == 0))
                {
                    Console.WriteLine(@"Слово """ + word + @""" не содержит ошибки");
                    Console.WriteLine(@"Для выхода нажмите любую клавишу...");
                    return;
                }
                Console.WriteLine(@"Слово """ + word + @""" можно исправить на:");
                foreach (var x in dic) Console.WriteLine(x.Key);
                Console.WriteLine(@"Для выхода нажмите любую клавишу...");
            }
            catch
            {
                Console.WriteLine("Ошибка!!!");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(@"Введите слово для проверки:");
            string word = Console.ReadLine();
            Console.WriteLine(@"Введите коэффициент проверки:");
            Verification(word, @"dic", int.TryParse(Console.ReadLine(), NumberStyles.Integer, new CultureInfo("ru-RU"), out int coefficient ) ? coefficient : 1);
            Console.WriteLine(@"Для выхода в любое время нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}