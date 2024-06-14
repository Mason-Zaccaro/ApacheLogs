using System;
using System.Collections.Generic;
using System.IO;

namespace ApacheLogs
{
    internal class Apache
    {
        public static List<LogEntry> Parse(string configFile)
        {
            try
            {
                var config = Config.LoadFromFile(configFile) ?? throw new Exception("Неправильно задан файл конфигурации!");

                var logFiles = Directory.GetFiles(config.FilesDir, $"*.{config.Ext}");
                if (logFiles.Length == 0)
                {
                    throw new Exception("Файлов с данным расширением нет в выбранной вами папке!");
                }

                var result = new List<LogEntry>();

                foreach (var logFile in logFiles)
                {
                    var logLines = File.ReadAllLines(logFile);
                    foreach (var logLine in logLines)
                    {
                        try
                        {
                            var logEntry = LogEntry.Parse(logLine, config.Format);
                            result.Add(logEntry);
                        }
                        catch (Exception e)
                        {
                            ConsoleHelper.WriteError($"Ошибка при обработке строки: {e.Message}");
                        }
                    }
                }

                return result.Count > 0 ? result : null;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Ошибка: {ex.Message}");
                return null;
            }
        }
    }
}
