using System;
using System.IO;
using System.Linq;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace DelMesVk
{
    static class ids
    {
        public static long userid { get; set; }
        public static long peerid { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("txt");
            Console.Write("Введите id usera: ");
            ids.userid = Convert.ToInt64(Console.ReadLine());
            Console.Write("Введите id беседы: ");
            ids.peerid = 2000000000;
            ids.peerid += Convert.ToInt64(Console.ReadLine());
            while (true)
            {
                Console.Write($"1. Вход через Config.txt\n" +
                    $"2. Вход черз логин + пароль\n\t" +
                    $"Команда: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        authwithconfig();
                        break;
                    case "2":
                        authwithlogin();
                        break;
                }
            }
        }


        static void authwithconfig()
        {
            try
            {
                string config = null;
                config = File.ReadLines("txt/config.txt").First();
                VkApi api = new VkApi();
                api.AuthorizeAsync(new ApiAuthParams { AccessToken = config });
                delmes(api);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        static void authwithlogin()
        {
            try
            {
                Console.Write("Введите логин: ");
                string login = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string pass = "";
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Backspace)
                    {
                        pass += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write("\b");
                    }
                }
                while (key.Key != ConsoleKey.Enter);
                Console.WriteLine();
                VkApi api = new VkApi();

                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 6121396,
                    Login = login,
                    Password = pass,
                    Settings = Settings.All,
                    TwoFactorAuthorization = () =>
                    {
                        Console.WriteLine("Enter Code:");
                        return Console.ReadLine();
                    }
                });
                File.WriteAllText("txt/config.txt", api.Token);
                delmes(api);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        static void delmes(VkApi api)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                    var mes = api.Messages.GetHistory(new VkNet.Model.RequestParams.MessagesGetHistoryParams { Count = 4, PeerId = ids.peerid });
                    foreach (var m in mes.Messages)
                    {
                        if (m.FromId == ids.userid)
                        {
                            logs(m);
                            api.Messages.Delete(messageIds: new ulong[] { Convert.ToUInt64(m.Id) }, deleteForAll: true);
                        }
                    }
                }
                catch(Exception ex) 
                { 
                    Console.WriteLine(ex.Message+ "\n");
                    break;
                }
            }
        }
        static void logs(Message m)
        {
            using (var sw = File.AppendText("txt/log.txt"))
                sw.WriteLine($"{m.FromId}:   {m.Text}");
        }
    }
}
