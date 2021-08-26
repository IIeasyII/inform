using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace inform
{
    class Program
    {
        static void Main(string[] args)
        {
            //Не стал заморачиваться на данном моменте, с выводом диалогового окна, проверкой [] args и т.п., на данном этапе не так важно.
            Console.WriteLine("Укажите файл:");
            //Защиты от дурака нету, да, можно ввести кривое название не существующего файла,
            //выдаст exception, обработки собственно нет и я это понимаю.
            //Просто вводим полное название файла, лежаее рядом с исполняемым файлом
            var file = Console.ReadLine();

            var content = File.ReadAllText(file);
            Console.WriteLine("Подождите...");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Analysis(content);
            sw.Stop();
            Console.WriteLine("Время выполнения: " + sw.ElapsedMilliseconds + "мс");
        }

        /// <summary>
        /// Частотный анализ 10 часто встречаемых треплетов слов.
        /// Формат вывода на консоль: xxx, xxx, ..., xxx
        /// Где xxx -> треплет
        /// </summary>
        /// <param name="text">Текст</param>
        public static void Analysis(string text)
        {
            //Все треплеты текста
            var triplets = new List<string>();

            //Разбиваем текст на слова
            var split = Regex.Split(text, @"\W+")
                //.AsParallel() //-> Снижает на большом объеме
                .ToArray();

            object sync = new object();
            //Из набора слов формируем треплеты
            Parallel.For(0, split.Length - 1, (i) =>
            {
                lock (sync)
                {
                    string word = split[i];

                    for (int j = 0; j < word.Length - 2; j++)
                    {
                        triplets.Add(word.Substring(j, 3));
                    }
                }
            });

            //Группируем одинаковые треплеты
            var groups = triplets.AsParallel().GroupBy(str => str);

            //Выводим на экран первые 10 треплетов с наибольшим количеством повторений
            Console.WriteLine(string.Join
                (
                    ',',
                    groups.OrderByDescending(gr => gr.Count()).Take(10).Select(gr => $"{gr.Key}")
                ));
        }
    }
}
