using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace ApacheLogs
{
    internal class Program
    {
        static string configpath = "config.txt";
        static Timer timer; 

        static void Main(string[] args)
        {
            ShowWelcomeMessage();

            StartTimer();

            while (true)
            {
                Console.Write("> ");

                string command = Console.ReadLine();

                switch (command.ToLower().Trim())
                {
                    case "config":
                        OpenFileInDefaultProgram(configpath);
                        break;
                    case var s when s.StartsWith("getlog"):
                        GetData(s);
                        break;
                    case "clear":
                        ClearConsole();
                        break;
                    case "parse":
                        Parse(configpath);
                        break;
                    case "close":
                        timer?.Dispose();
                        return;
                    default:
                        ConsoleHelper.WriteError("Неизвестная команда!");
                        break;
                }
            }
        }

        static void StartTimer()
        {
            Config config = Config.LoadFromFile(configpath);

            if (config != null)
            {
                timer = new Timer(state =>
                {
                    config = Config.LoadFromFile(configpath);

                    if (config.ShowCron)
                    {
                        Console.WriteLine("Парсинг логов...");
                    }
                   
                    Parse(configpath, config.ShowCron);

                    if (config.ShowCron)
                    {
                        Console.Write("> ");
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromMinutes(config.MinuteOfUpdate));
            }
            else
            {
                ConsoleHelper.WriteError("Не удалось запустить приложение из-за отсутствующей или некорректной конфигурации.");
            }
        }

        static void ShowWelcomeMessage()
        {
            ConsoleHelper.WriteInfo("Добро пожаловать в программу по просмотру логов. =^.^=\n" +
                              "Cписок доступных команд:\n" +
                              "config - Открывает файл конфигурации\n" +
                              "parse - Извлекает данные из конфига, сопоставляет их с логами и записывает полученные данные из логов в базу данных\n" +
                              "getlog (ip) (date|datefrom) (dateto) (status) - Получает данные логов из уже выгруженной базы данных.\n" +
                              "clear - Очищает консоль и выводит доступные команды.\n" +
                              "close - Завершает выполнение программы\n");
        }

        static void OpenFileInDefaultProgram(string filePath)
        {
            try
            {
                Process.Start(filePath);
                ConsoleHelper.WriteInfo("Не забудь сохранить файл и прописать команду parse, если это требуется!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Ошибка при открытии файла: " + ex.Message);
            }
        }

        static void Parse(string configpath, bool isShow = true)
        {
            var logs = Apache.Parse(configpath);
            if (logs == null)
            {
                ConsoleHelper.WriteError("Не удалось считать данные");
            }
            else
            {
                DataBase.Create();
                bool isSuccess = DataBase.SetDatas(logs);
                if (isSuccess)
                {
                    if(isShow)
                    {
                        ConsoleHelper.WriteInfo("Данные успешно получены и записаны в базу данных!");
                    }                  
                }
                else
                {
                    if (isShow)
                    {
                        ConsoleHelper.WriteError("Произошли ошибки при записи данных в базу.");
                    }
                }
            }
        }

        static void GetData(string commandline)
        {
            string[] commands = commandline.Trim().Split(' ');

            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            string ip = null;
            int? status = null;

            for (int i = 1; i < commands.Length; i++)
            {
                string datetmp = commands[i];

                if (int.TryParse(datetmp, out int sta))
                {
                    status = sta;
                }
                else if (datetmp.Count(c => c == '.') == 3)
                {
                    ip = datetmp;
                }
                else if (DateTime.TryParseExact(datetmp, new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "dd/MMM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    if (dateFrom == null)
                    {
                        dateFrom = date;
                    }
                    else
                    {
                        dateTo = date;
                    }
                }
                else
                {
                    ConsoleHelper.WriteError("Неверный формат данных!");
                    return;
                }
            }

            DataBase.GetLogsByFilter(dateFrom, dateTo, ip, status);
        }

        static void ClearConsole()
        {
            Console.Clear();
            ShowWelcomeMessage();
        }
    }
}
