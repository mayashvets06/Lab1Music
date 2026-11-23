using System;
using System.Text;

namespace Lab1Music
{

    /// Класс-сущность «Трек».
    /// Инварианты:
    ///  1) длительность от 1 до MAX_DURATION секунд;
    ///  2) число прослушиваний может только расти;
    ///  3) название непустое;
    ///  4) номер задаётся один раз и не меняется.
    public class Track
    {
        // Статические члены
        private static int _tracksCreated = 0;    //  Кто такой статик??? статическое поле, которое принадлежит всему классу, а не только объекту
        public static int TracksCreated => _tracksCreated; // Что такое свойство и => сокращённая запись свойства, у которого тело состоит из одного выражения

        public const int MAX_DURATION = 3600; // статическая константа

        // Поля: все private (инкапсуляция)
        private readonly int _number;          // номер не меняется
        private string _title;                 // непустое
        private string _artist;                // непустое
        private int _durationSeconds;          // 1..MAX_DURATION
        private int _playCount;                // только растёт
        private Genre[] _genres = Array.Empty<Genre>();  // вложенный массив 

        // Конструкторы
        public Track(int number, string title, string artist, int durationSeconds)
        {
            if (number <= 0)
                throw new ArgumentException("Номер трека должен быть положительным.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название трека не может быть пустым.");
            if (string.IsNullOrWhiteSpace(artist))
                throw new ArgumentException("Исполнитель не может быть пустым.");
            if (durationSeconds < 1 || durationSeconds > MAX_DURATION)
                throw new ArgumentException($"Длительность должна быть от 1 до {MAX_DURATION} секунд.");

            _number = number;
            _title = title.Trim();
            _artist = artist.Trim();
            _durationSeconds = durationSeconds;
            _playCount = 0;

            _tracksCreated++;
        }

        // Упрощённый конструктор.
        public Track(string title, string artist)
            : this(++_tracksCreated, title, artist, 180)
        {
        }

        // Геттеры 
        public int Number => _number;
        public string Title => _title;
        public string Artist => _artist;
        public int DurationSeconds => _durationSeconds;
        public int PlayCount => _playCount;

        // Методы предметной области (вместо сеттеров)

        // Проиграть один раз.
        public void Play() => Play(1);

        // Проиграть N раз. Перегрузка: короткая версия вызывает полную.
        public void Play(int times)
        {
            if (times <= 0)
                throw new ArgumentException("Количество прослушиваний должно быть строго больше нуля.");
            _playCount += times;
        }

        // Переименовать трек с проверкой (метод вместо сеттера, защищает инвариант в двух местах).
        public void Rename(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Новое название не может быть пустым.");
            _title = newTitle.Trim();
        }

        // Работа с жанрами

        // Добавить жанр.
        public void AddGenre(Genre genre)
        {
            if (genre == null) throw new ArgumentNullException(nameof(genre));

            var copy = new Genre[_genres.Length + 1];
            Array.Copy(_genres, copy, _genres.Length);
            copy[copy.Length - 1] = genre;
            _genres = copy;
        }

        // ПЛОХО: возвращает сам внутренний массив — внешний код может его испортить.
        // Нужен только для демонстрации проблемы в Main.
        public Genre[] GetGenresUnsafe() => _genres;

        // ПРАВИЛЬНО: защитное копирование массива.
        public Genre[] GetGenres() => (Genre[])_genres.Clone();

        // toString()
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Track #{_number} \"{_title}\" — {_artist}, ");
            sb.Append($"{_durationSeconds}с, прослушиваний: {_playCount}");
            if (_genres.Length > 0)
            {
                sb.Append(", жанры: ");
                for (int i = 0; i < _genres.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(_genres[i].Name);
                }
            }
            return sb.ToString();
        }
    }
}
