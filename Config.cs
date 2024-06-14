using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ApacheLogs
{
    internal class Config
    {
        public string FilesDir { get; set; }
        public string Ext { get; set; }
        public string Format { get; set; }
        public int MinuteOfUpdate { get; set; }
        public bool ShowCron { get; set; }

        private Config() { }

        public static Config LoadFromFile(string configPath)
        {
            if (!File.Exists(configPath))
            {
                ConsoleHelper.WriteError("Файл конфигурации не найден, поэтому он будет создан со стандартными параметрами!");
                File.WriteAllText(configPath, "files_dir = C:\\Users\\..\r\next = log\r\nformat = %h %l %u %t \\\"%r\\\" %>s %b\r\ntime = 60\r\nshowcron = false");
                Process.Start(configPath);
            }

            var config = new Config();
            var lines = File.ReadAllLines(configPath);

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "files_dir":
                        config.FilesDir = value;
                        break;
                    case "ext":
                        config.Ext = value;
                        break;
                    case "format":
                        config.Format = value;
                        break;
                    case "time":
                        if (!int.TryParse(value, out var time))
                        {
                            ConsoleHelper.WriteError($"Ошибка: неверный формат времени обновления '{value}', установлено значение по умолчанию 60 минут.");
                            time = 60;
                        }
                        config.MinuteOfUpdate = time;
                        break;
                    case "showcron":
                        if(!bool.TryParse(value, out var showcron))
                        {
                            ConsoleHelper.WriteError($"Ошибка: неверный формат настройки отображения работы cron '{value}', установлено значение по умолчанию true.");
                            config.ShowCron = true;
                        }
                        config.ShowCron = showcron;
                        break;
                }
            }

            if (string.IsNullOrEmpty(config.FilesDir) || string.IsNullOrEmpty(config.Ext) || string.IsNullOrEmpty(config.Format))
            {
                ConsoleHelper.WriteError("Ошибка: не все необходимые параметры заданы в конфигурационном файле.");
                return null;
            }

            return config;
        }
    }
}
