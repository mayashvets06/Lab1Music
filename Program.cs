using System;

namespace Lab1Music
{
    internal static class Program
    {
        // Плейлист, с которым работает пользователь (для меню).
        private static Playlist _playlist = new Playlist("Мой плейлист", 20);
        private static int _nextNumber = 1;

        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Немного демо-данных, чтобы плейлист не был пустым при старте.
            _playlist.Add(new Track(_nextNumber++, "Bohemian Rhapsody", "Queen", 355));
            _playlist.Add(new Track(_nextNumber++, "Imagine", "John Lennon", 183));

            while (true)
            {
                PrintMenu();
                string choice = (Console.ReadLine() ?? "").Trim();

                try
                {
                    switch (choice)
                    {
                        case "1": AddTrack(); break;
                        case "2": PlayTrack(); break;
                        case "3": ShowAll(); break;
                        case "4": FindByNumber(); break;
                        case "5": RemoveByNumber(); break;
                        case "6": ShowTotalDuration(); break;
                        case "7": ShowMostPlayed(); break;
                        case "8": ShowSorted(); break;
                        case "9": AddGenreToTrack(); break;
                        case "10": DemoUnsafeGenres(); break;
                        case "11": ShowRulesTable(); break; // шпаргалка
                        case "0": return;
                        default:
                            Console.WriteLine("Неизвестная команда. Попробуй ещё раз.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Любая ошибка предметной области ловится здесь, программа продолжает работу.
                    Console.WriteLine("Ошибка: " + ex.GetType().Name + ": " + ex.Message);
                }

                Console.WriteLine();
                Console.WriteLine("Нажми Enter, чтобы продолжить...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        //  МЕНЮ

        private static void PrintMenu()
        {
            Console.WriteLine("===== ПЛЕЙЛИСТ: ГЛАВНОЕ МЕНЮ =====");
            Console.WriteLine("1  — добавить трек");
            Console.WriteLine("2  — проиграть трек (увеличить счётчик)");
            Console.WriteLine("3  — показать все треки");
            Console.WriteLine("4  — найти трек по номеру");
            Console.WriteLine("5  — удалить трек по номеру");
            Console.WriteLine("6  — суммарная длительность");
            Console.WriteLine("7  — самый популярный трек");
            Console.WriteLine("8  — сортировка по прослушиваниям");
            Console.WriteLine("9  — добавить жанр к треку");
            Console.WriteLine("10 — ДЕМО: испортить массив жанров через плохой геттер");
            Console.WriteLine("11 — список правил: поле — правило — метод");
            Console.WriteLine("0  — выход");
            Console.WriteLine();
            Console.Write("Твой выбор: ");
        }

        // ВВОД

        private static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            string s = (Console.ReadLine() ?? "").Trim();
            int value;
            if (!int.TryParse(s, out value))
                throw new ArgumentException("Нужно ввести целое число.");
            return value;
        }

        private static string ReadString(string prompt)
        {
            Console.Write(prompt);
            return (Console.ReadLine() ?? "").Trim();
        }

        // ИНТЕРАКТИВНЫЕ КОМАНДЫ

        private static void AddTrack()
        {
            string title = ReadString("Название: ");
            string artist = ReadString("Исполнитель: ");
            int duration = ReadInt("Длительность в секундах (1.." + Track.MAX_DURATION + "): ");

            // Вся валидация — в конструкторе Track.
            var track = new Track(_nextNumber, title, artist, duration);
            _playlist.Add(track);
            _nextNumber++;

            Console.WriteLine("Добавлено: " + track);
        }

        private static void PlayTrack()
        {
            int number = ReadInt("Номер трека: ");
            Track track = _playlist.FindByNumber(number);
            if (track == null)
            {
                Console.WriteLine("Трек с таким номером не найден.");
                return;
            }

            int times = ReadInt("Сколько раз проиграть (>=1): ");
            track.Play(times); // проверка внутри Track
            Console.WriteLine("Текущее состояние: " + track);
        }

        private static void ShowAll()
        {
            Console.WriteLine(_playlist.ToString());
        }

        private static void FindByNumber()
        {
            int number = ReadInt("Номер трека: ");
            Track track = _playlist.FindByNumber(number);
            Console.WriteLine(track == null ? "Не найдено." : track.ToString());
        }

        private static void RemoveByNumber()
        {
            int number = ReadInt("Номер трека для удаления: ");
            bool ok = _playlist.RemoveByNumber(number);
            Console.WriteLine(ok ? "Удалено." : "Трек с таким номером не найден.");
        }

        private static void ShowTotalDuration()
        {
            Console.WriteLine("Суммарная длительность: " + _playlist.TotalDurationSeconds() + " с");
        }

        private static void ShowMostPlayed()
        {
            Track best = _playlist.MostPlayed();
            Console.WriteLine(best == null ? "Плейлист пуст." : "Самый популярный: " + best);
        }

        private static void ShowSorted()
        {
            Track[] sorted = _playlist.SortedByPlayCountDesc();
            if (sorted.Length == 0)
            {
                Console.WriteLine("Плейлист пуст.");
                return;
            }
            foreach (var t in sorted)
                Console.WriteLine("  " + t);
        }

        private static void AddGenreToTrack()
        {
            int number = ReadInt("Номер трека: ");
            Track track = _playlist.FindByNumber(number);
            if (track == null)
            {
                Console.WriteLine("Трек с таким номером не найден.");
                return;
            }

            string name = ReadString("Название жанра: ");

            track.AddGenre(new Genre(name)); // проверка внутри Genre
            Console.WriteLine("Жанр добавлен. Трек: " + track);
        }

        // Демонстрация ошибки общей ссылки.
        private static void DemoUnsafeGenres()
        {
            int number = ReadInt("Номер трека: ");
            Track track = _playlist.FindByNumber(number);
            if (track == null)
            {
                Console.WriteLine("Трек с таким номером не найден.");
                return;
            }

            Console.WriteLine("До порчи:  " + track);

            Genre[] unsafeGenres = track.GetGenresUnsafe();
            if (unsafeGenres.Length == 0)
            {
                Console.WriteLine("У трека нет жанров — сначала добавь хотя бы один (пункт 9).");
                return;
            }

            unsafeGenres[0] = new Genre("HACKED");
            Console.WriteLine("После GetGenresUnsafe(): " + track);
            Console.WriteLine("(Внешний код изменил внутренние данные трека — так делать нельзя.)");
        }

        private static void ShowRulesTable()
        {
            Console.WriteLine("ПОЛЕ                    | ПРАВИЛО                                 | ГДЕ ПРОВЕРЯЕТСЯ");
            Console.WriteLine("------------------------+-----------------------------------------+---------------------------");
            Console.WriteLine("Track._number           | задаётся один раз, не меняется          | readonly + конструктор");
            Console.WriteLine("Track._title            | непустое                                | конструктор, Rename");
            Console.WriteLine("Track._artist           | непустой                                | конструктор Track");
            Console.WriteLine("Track._durationSeconds  | от 1 до MAX_DURATION (3600)             | конструктор Track");
            Console.WriteLine("Track._playCount        | может только расти                      | Play(int) — отказ при times <= 0");
            Console.WriteLine("Track._genres           | наружу не отдаётся, только копия        | GetGenres()");
            Console.WriteLine("Playlist._tracks        | наружу не отдаётся, только копия        | ToArray()");
            Console.WriteLine("Playlist._count         | не превышает ёмкости                    | Add()");
            Console.WriteLine("Genre.Name              | непустое                                | конструктор Genre");
            Console.WriteLine("Genre (класс целиком)   | неизменяем: сеттеров нет                | только get-свойства");
        }
    }
}
