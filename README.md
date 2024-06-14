# Агрегатор логов Apache

## Агрегатор логов Apache — это приложение агрегатор данных из access логов веб-сервера Apache с сохранением в БД.

# Установка и запуск
Склонируйте репозиторий:

>git clone https://github.com/Mason-Zaccaro/ApacheLogs.git

Перейдите в каталог проекта:

>cd ApacheLogs/bin/Debug

## Запустите приложение:
> start ApacheLogs.exe

# Настройте конфигурационный файл config.txt

# Конфигурация
Ниже приведен пример конфигурации:

bin/Debug/config.txt:
>files_dir - путь к директории логов

>ext - расширение логов

>format - формат логов

>time - автоматическое обновление логов в минутах

>showcron - true или false. Определяет, выводить ли отчёт об использованиие cron

При первом зауске программы, она самостоятельно создаст файл конфигурации и откроет его в редакторе по умолчанию

# Виды запросов
>config - Открывает файл конфигурации.

>parse - Извлекает данные из конфига, сопоставляет их с логами и записывает полученные данные из логов в бд.

>getlog (ip) (date|datefrom) (dateto) (status) - Получает данные логов из уже выгруженной базы данных.

>clear - Очищает консоль и выводит доступные команды.

>close - Завершает выполнение программы.

## Параметры
### getlog:
> без параметров: выводит все доступные логи

> date|datefrom - если нет параметра dateto, то поиск идёт только по этой дате, иначе будет считатся как стартовая дата для диапозона

>dateto - по какую дату делать выборку

>ip - выводит только те логи, у которых есть данный ip

>status - выводит только те логи, у которых есть данный status

# Примеры использования:

>getlog

>getlog 200

>getlog 127.0.0.1

>getlog 172.16.0.1 13.11.2020 17.03.2023

>getlog 172.16.0.1 13.11.2020 17.03.2023 200

## Формат данных
Дата в запросе указывается в формате "dd.mm.yyyy", "dd-mm-yyyy", "dd-/mm/yyyy" или "dd-/mmm/yyyy". 

Примеры:
>04.01.2020

>04-01-2020

>04/01/2020

>04/jan/2020

IP-адрес указывается в стандартном формате "ddd.ddd.ddd.ddd". Примеры:

>192.168.1.1

>10.0.0.1
