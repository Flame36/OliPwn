using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CommandLine;

namespace OliPwn
{
    internal class Program
    {
        public static string payload = "switch (({$VAR$} / {$DEFINEVAR$}) % 4) { case 0: return -2; case 1: *(char *)0 = 0; case 2: while (1) {} case 3: long long* buf; while (1) { buf = new long long; *buf = 0xdeadbeef; }}";
        public const double MEMORY_TRESHOLD = 1000;
        public const double TIME_TRESHOLD = 0.001;

        // TODO
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }


        static void Main(string[] args)
            => new Program().MainAsync().Wait();

        public async Task MainAsync()
        {
            Console.Write("Username:");
            string username = Console.ReadLine();
            Console.Write("Password:");
            string password = Console.ReadLine();
            Console.Write("Task name:");
            string taskName = Console.ReadLine();
            Console.Write("Variable name:");
            string variableName = Console.ReadLine();
            Console.Write("Variable max value:");
            long upperBound = long.Parse(Console.ReadLine());
            Console.Write("Invalid output example:");
            string invalidOutput = Console.ReadLine();
            Console.Write("Template file name:");
            string file = File.ReadAllText(Console.ReadLine());

            OlinfoApi session = new();

            await session.Login(username, password);
            OlinfoTask task = await session.GetTaskDataAsync(taskName);

            List<int> ids = new();

            int magnitude = (int)Math.Ceiling(Math.Log(upperBound)/Math.Log(4));
            for (int i = 0; i < magnitude; i++)
            {
                string newFile = file.Replace("{$CHECK$}", payload).Replace("{$VAR$}", variableName).Replace("{$DEFINEVAR$}", $"{ 1L << (i * 2) }ll");
                Console.Write($"{(i+1)*100.0/magnitude:0.00}% ({(i+1)}/{magnitude})");
                Console.CursorLeft = 0;
                ids.Add(await session.SubmitFileAsync(task, newFile));
            }

            await Task.Delay(20000);

            Console.Write("Checking ids: [");
            for (int i = 0; i < ids.Count; i++)
            {
                if (i != 0)
                    Console.Write(", ");
                Console.Write(ids[i]);
            }
            Console.WriteLine("]");


            List<long> vars = new();
            for (int i = 0; i < ids.Count; i++)
            {
                List<SubmissionResult> sub = await session.GetSubmissionResultAsync(ids[i]);

                if (i == 0)
                    for (int j = 0; j < sub.Count; j++)
                        vars.Add(0);

                Console.Write($"{variableName}/{1L << (i * 2)} mod 4: ");
                for (int j = 0; j < sub.Count; j++)
                {
                    long delta = 0;

                    if (task.MemoryLimit - sub[j].Memory < MEMORY_TRESHOLD)
                        delta = 3;
                    else if (task.TimeLimit - sub[j].Time < TIME_TRESHOLD)
                        delta = 2;
                    else if (sub[j].Text.Contains("killed"))
                        delta = 1;

                    vars[j] += delta * (1L << (i * 2));
                    Console.Write($"{delta} ");
                }
                Console.WriteLine();
            }

            foreach (long var in vars)
            {
                Console.Write($"{var} ");
            }

        }
    }
}