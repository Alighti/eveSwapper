using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowManager;

namespace eveSwapper
{
    class Program
    {
        static public APIs GetAPIs = new APIs();
        static public string username, target, bio, Session;
        static public EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        static public EventWaitHandle waitBlocked = new EventWaitHandle(false, EventResetMode.ManualReset);
        static public EventWaitHandle nothing = new EventWaitHandle(false, EventResetMode.ManualReset);
        static bool bools;
        private static HttpStatusCode httpStatus = new HttpStatusCode();
        public static volatile int BlockCounter;
        public static volatile int Counter;
        static public int threads = 5;
        static public int sleep = 5;
        static public int loops = 5;
        static void Main(string[] args)
        {
            Console.Title = "eveSwapper";
            while (true)
            {
                Console.Clear();
                Console.Write("[e] Username : ");
                try
                {
                    Session = File.ReadAllLines($"@{Console.ReadLine()}.txt")[2].Split(':')[1];
                }
                catch { }
                var con = GetAPIs.Consent(Session);
                if (con == HttpStatusCode.OK)
                {
                    username = GetAPIs.CURRENT_USER(Session);
                   
                        if (username == null || username.Length < 2)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("[e] Someting wrong!, try again? [Y/N]: ");
                            Console.ResetColor();
                            if (Console.ReadLine().Equals("n", StringComparison.CurrentCultureIgnoreCase))
                            {
                                return;
                            }
                            else
                            {
                                continue;
                            }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[e] Logged in {username} Successfully  !");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[e] Someting wrong!, try again? [Y/N]: ");
                    Console.ResetColor();
                    if (Console.ReadLine().Equals("n", StringComparison.CurrentCultureIgnoreCase))
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                break;
            }
            Console.ResetColor();
            Console.Write("[e] Target : ");
            target = Console.ReadLine();

            if (GetAPIs.CHECK_14(target))
            {
                Console.WriteLine($"[e] @{target} Can't Swap it ..");
            }
            else
            {
                Console.WriteLine($"[e] @{target} is swappable");
            }
            Console.Write("[e] Bio : ");
            bio = Console.ReadLine();
            threads = new Random().Next(25,50);
            Console.Write("[e] Enter Threads : ");
            var thre = Console.ReadLine();
            if (thre != "")
                threads = int.Parse(thre);
            loops = new Random().Next(500);
            Console.Write("[e] Enter Loops : ");
            var lop = Console.ReadLine();
            if (lop != "")
                 loops = int.Parse(lop);
            Console.Clear();
            Parallel.For(0, threads, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },(integer, LoopState) =>
            {
                Thread thread = new Thread(delegate () { RequestSET(); })
                 {
                   Priority = ThreadPriority.Highest,
                   IsBackground = true
                 };
                 thread.Start();
            });
            Console.Clear();
            Console.WriteLine($"[e] Target : {target}\n[e] Threads : {threads}\n[e] Loops : {loops}");
            MsgBoxResult result = Interaction.MsgBox("[e] Press ok when u ready", Console.Title, MsgBoxStyle.Information);
            waitHandle.Set();
            Task.Run(() =>
            {
                SuperVisior();
            });
            nothing.WaitOne();
        }
        public static void RequestSET()
        {
                waitHandle.WaitOne();
                while (true)
                {
                    if (bools || BlockCounter >= 60) 
                        return;
                    for (int i = 0; i < loops; i++)
                    {
                        if (bools || BlockCounter >= 60)
                            return;
                            httpStatus = GetAPIs.Set_username(target, Session);

                            if (httpStatus == HttpStatusCode.BadRequest)
                            {
                                Interlocked.Increment(ref Counter);
                            }
                            if (httpStatus == HttpStatusCode.OK)
                            {
                                GetAPIs.Set_biography(bio, Session);
                                Claimed();
                                Interlocked.Increment(ref Counter);
                            }
                            if (httpStatus.ToString() == "429")
                            {
                                Interlocked.Increment(ref BlockCounter);
                            }
                    }
                   sleep = new Random().Next(150);
                   if (sleep == 5)
                      sleep = 1;
                   Thread.Sleep(TimeSpan.FromMilliseconds(sleep));
                } 
        }
        public static void Claimed()
        {
            bools = true;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[e] Successfully Swapped @{target}");
            Console.ResetColor();
            Console.WriteLine($"[e] Attempts: {Counter} , Spam: {BlockCounter}");
            Environment.Exit(0);
        }
        public static void SuperVisior()
        {
            while (true)
                if (!bools)
                    Console.Write($"[e] Attempts: {Counter} , Spam: {BlockCounter}\r");
            
        }
    }
}
