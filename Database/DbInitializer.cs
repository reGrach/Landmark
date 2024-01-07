using Landmark.Database.Model;

namespace Landmark.Database
{
    public static class DbInitializer
    {
        public static void Initialize(LandmarkContext context)
        {
            //context.Database.EnsureCreated();

            if (context.Participant.Any())
            {
                return;   // DB has been seeded
            }

            var participants = new Participant[]
            {
                new Participant {  Name = "Араратов Павел" },
                new Participant {  Name = "Быстряков Антон" },
                new Participant {  Name = "Костикова Екатерина" },
                new Participant {  Name = "Костикова Варвара" },
                new Participant {  Name = "Бабенкова Алекса" },
                new Participant {  Name = "Иванов Алексей" },
                new Participant {  Name = "Тележкин Алексей" },
                new Participant {  Name = "Гавриленко Ростислав" },
                new Participant {  Name = "Сидорин Иван" },
                new Participant {  Name = "Павленко Артём" },
                new Participant {  Name = "Рыбченко Полина" },
                new Participant {  Name = "Мишуловина Анастасия" },
                new Participant {  Name = "Филева Ксения" },
                new Participant {  Name = "Ронкин Леонид" },
                new Participant {  Name = "Жуковский Андрей" },
                new Participant {  Name = "Оршанский Тихон" },
                new Participant {  Name = "Шагоян Анастасия" },
                new Participant {  Name = "Иванова Татьяна" },
                new Participant {  Name = "Воронов Егор" },
                new Participant {  Name = "Кривобоков Егор" },
                new Participant {  Name = "Цветков Леонид" },
                new Participant {  Name = "Рыкин Иван" },
                new Participant {  Name = "Четыркина София" },
                new Participant {  Name = "Малый Александр" },
                new Participant {  Name = "Модягина Дарья" },
                new Participant {  Name = "Давлетов Эмиль" },
                new Participant {  Name = "Шапиро Федор" },
                new Participant {  Name = "Саркисян Владислав" },
                new Participant {  Name = "Кармазиненко Александр" },
                new Participant {  Name = "Борщёв Егор" },
                new Participant {  Name = "Ковалёв Кирилл" },
                new Participant {  Name = "Танин Максим" },
                new Participant {  Name = "Остапенко Анастасия" },
                new Participant {  Name = "Араччиге Вероника" },
                new Participant {  Name = "Павинская Ирина" },
                new Participant {  Name = "Калинина Мари" },
                new Participant {  Name = "Гаврилина Александра" },
                new Participant {  Name = "Кашуба Марианна" },
                new Participant {  Name = "Ламов Святослав" },
                new Participant {  Name = "Ефремова Ксения" },
                new Participant {  Name = "Полозкова Елизавета" },
                new Participant {  Name = "Иванова Дарья" },
                new Participant {  Name = "Лаевская Алиса" },
                new Participant {  Name = "Гвоздецкая Юлия" },
                new Participant {  Name = "Музыка Арина" },
                new Participant {  Name = "Улуханова Кристина" },
                new Participant {  Name = "Подгорнова Ольга" },
                new Participant {  Name = "Нечаева Софья" },
                new Participant {  Name = "Фёдорова Ульяна" },
                new Participant {  Name = "Инталёв Константин" },
                new Participant {  Name = "Басков Антон" },
                new Participant {  Name = "Дементьев Владислав" },
            };

            context.Participant.AddRange(participants);
            context.SaveChanges();
        }
    }
}