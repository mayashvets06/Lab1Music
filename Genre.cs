using System;

namespace Lab1Music
{
    // Класс-значение «Жанр». Неизменяемый: поля только для чтения, сеттеров нет.
    // Изменение возвращает новый объект.
    public sealed class Genre
    {
        public string Name { get; }
        public Genre(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название жанра не может быть пустым.");
            Name = name.Trim();
        }
    }
}
