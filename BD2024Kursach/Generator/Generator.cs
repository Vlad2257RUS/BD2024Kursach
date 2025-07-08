using BD2024Kursach.Context;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BD2024Kursach.Generator
{
    delegate void UpdateProgressBarDelegate(int Progress);
    
    internal class Generator
    {
        private Random random = new Random();

        DbContext_Npgsql dbContext = DbContext_Npgsql.GetInstance();

        public UpdateProgressBarDelegate updateProgressBar { private get; set; }

        //private static string contentPath = Path.Combine(Directory.GetCurrentDirectory(), "Content");

        const int MAX_ERRORS_TO_GENERATE = 20;
        public const string DATE_FORMAT = "dd.MM.yyyy";
        public const string TIME_FORMAT = "hh:mm";

        public Dictionary<string, string> GenerateUsers(int count, string[] roles)
        {


            string[] fnames = {"Анастасия Федорова",
"Александр Васильев","Степан Никитин","Светлана Захарова","Александр Жаров","Тимофей Богомолов","Екатерина Шапошникова","Дарья Воронина","Милана Муравьева","Мария Иванова",
"Али Денисов","Светлана Филиппова","Ярослав Власов","Полина Владимирова","Лея Короткова","Фатима Логинова","Мирослава Белкина","Алиса Логинова",
"Мирослава Глушкова","Роман Воробьев","Иван Алексеев","Екатерина Грибова","Артём Орлов","Владимир Глебов","Анна Суслова","Артём Миронов","Василиса Завьялова",
"Маргарита Филиппова","Фёдор Панкратов","София Михайлова","Алиса Игнатьева","Ева Афанасьева","Дарья Панина","Елисей Андреев","Арина Петрова","Владимир Ильин","Александр Карпов",
"Лев Севастьянов","Егор Зеленин","Адам Гришин","Вероника Сазонова","Алёна Власова","Артемий Тимофеев","Полина Кудрявцева","София Кузьмина","Даниил Черный","Марк Романов","Мария Андреева",
"Светлана Федорова","Лев Николаев","Софья Петрова","Варвара Короткова","Эмилия Виноградова","Семён Харитонов","Николай Лопатин","Ева Орлова",
"Эмилия Антонова","Эмилия Карпова","Алиса Павлова","Есения Богданова","Мира Белякова","Фёдор Пастухов","Марк Дементьев","Иван Данилов","Анна Жукова","Артём Александров","Мария Ларина",
"Ульяна Кузнецова","Мирон Муратов","Ибрагим Болдырев","Даниил Кузин","Амина Никифорова","Даниэль Соколов","Анна Осипова","Ирина Маркина","Валерия Никитина","Амина Попова","Демьян Крылов",
"Алексей Никитин","Анастасия Мартынова","Даниил Мартынов","София Кузнецова","Таисия Лебедева","Мирон Гладков","Елизавета Пугачева","Дмитрий Федотов","Мария Астахова","Зоя Давыдова","Матвей Попов","Алексей Сычев","Михаил Алехин",
"Артём Сергеев","Василиса Маркелова","Никита Николаев","Елисей Котов","Максим Соловьев","Светлана Виноградова","Георгий Поздняков","Арсений Овчинников","Андрей Субботин"   
};

            string role = roles[random.Next(roles.Length)];

            var createdUsers = new Dictionary<string, string>();

            try
            {
                int countErrorsToGenerete = 0;

                int countRecords = dbContext.ExecuteScalarInt("SELECT COUNT(*) FROM employee");

                for (int i = 0; i < count; i++)
                {
                    bool gender = random.Next(10) < 7;

                    string userLogin = $"user{countRecords + i + 1}";
                    string password = GenerateRandomPassword();
                    
                    string name = string.Empty;
                    
                    string roleName = role;
                   // string user_data = GenerateUserDataJSON(gender);

                        name = fnames[random.Next(fnames.Length)];
                   

                    try
                    {
                        dbContext.SendRequest($"SELECT create_new_user_by_role('{userLogin}', '{password}', '{roleName}',  '{name}')");

                        createdUsers.Add(userLogin, password);
                        countErrorsToGenerete = 0;

                        Console.WriteLine($"Был создан Пользователь: {userLogin}\tПароль: {password}\tРоль: {roleName}.");
                    }
                    catch (Exception ex)
                    {
                        if (countErrorsToGenerete >= MAX_ERRORS_TO_GENERATE)
                            throw new Exception($"Превышено количество попыток генерации записей, последняя ошибка: {ex.Message}");

                        count++;
                        countErrorsToGenerete++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return createdUsers;
        }

        private string GenerateUserDataJSON(bool gender)
        {
            string[] hobbies = {
    "Рисование", "Фотография", "Вязание", "Плавание", "Чтение", "Йога", "Медитация", "Путешествия",
    "Пение", "Игра на гитаре", "Игра на пианино", "Гончарное дело", "Вышивание крестиком", "Коллекционирование марок",
    "Коллекционирование монет", "Рыбалка", "Походы", "Кемпинг", "Кулинария", "Выпечка", "Садоводство",
    "Разведение комнатных растений", "Аквариумистика", "Флористика", "Волонтерство", "Работа с деревом",
    "Лепка из глины", "Моделирование", "Аэрография", "Настольные игры", "Пазлы", "Танцы", "Катание на коньках",
    "Катание на роликах", "Велоспорт", "Бег", "Фитнес", "Подводное плавание", "Сноубординг", "Лыжи", "Серфинг",
    "Скейтбординг", "Графический дизайн", "Программирование", "Робототехника", "Изучение иностранных языков",
    "Каллиграфия", "Писательство", "Ведение блога", "Видеоблоггинг", "Подкастинг", "Игра в шахматы", "Игра в шашки",
    "Рукопашный бой", "Карате", "Джиу-джитсу", "Тай-чи", "Стрельба из лука", "Пейнтбол", "Лазертаг", "Косплей",
    "Моделирование железных дорог", "Коллекционирование фигурок", "Занятие астрономией", "Ведение дневника",
    "Кроссворды", "Садоводство", "Стрельба", "Гольф", "Теннис", "Бадминтон", "Футбол", "Баскетбол", "Волейбол",
    "Крикет", "Американский футбол", "Регби", "Сквош", "Боуллинг", "Фрисби", "Катание на лодке", "Виндсерфинг",
    "Парапланеризм", "Дельтапланеризм", "Парашютизм", "Полёты на воздушном шаре", "Скалолазание", "Альпинизм",
    "Спелеология", "Геокешинг", "Бёрдинг (наблюдение за птицами)", "Астрология", "Нумерология", "Таро",
    "Моделирование одежды", "Макияж", "Парикмахерское искусство", "Барменинг", "Смешивание ароматов",
    "Занятие йогой для животных"
};


            var dataObject = new
            {
                personal_info = new
                {
                    age = new Random().Next(22, 65),
                    gender = gender ? "male" : "fmale",
                    address = GenerateRandomAddress()
                },
                contact_info = new
                {
                    email = GenerateRandomEmail(),
                    phones = new
                    {
                        home = "+" + GenerateRandomPhoneNumber().ToString(),
                        work = "+" + GenerateRandomPhoneNumber().ToString(),
                    }
                },
                preferences = new
                {
                    theme = random.Next(10) >= 5 ? "dark" : "light",
                    language = random.Next(10) >= 4 ? "ru" : "en",
                    notifications = new
                    {
                        email = random.Next(10) >= 4,
                        sms = random.Next(10) >= 2
                    }
                },
                additional_info = new
                {
                    height = random.Next(129, 185),
                    weight = random.Next(60, 100),
                    hobbies = hobbies.OrderBy(x => random.Next()).Take(random.Next(1, 12)).ToArray()
                }
            };

            string user_data = JsonSerializer.Serialize(dataObject);

            return user_data;
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] passwordChars = new char[random.Next(5, 13)];
            for (int i = 0; i < passwordChars.Length; i++)
            {
                passwordChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(passwordChars);
        }

        public int GenerateChannel(int count, bool TakeExisting)
        {

            
            string name = string.Empty;
            string[] adjectives = { "ПервыйДом","НаКухне","Детский","СовФильм","Rain","Doom","Город","Мой","На местах","1 Россия","2 Россия","Беларусь","Москва","Время","Спорт","Мировые","Производство","Сельский","87","9",
            "Новый взгляд","Новости 34","Вперед","Курс","Перемены","МебельТВ","Для офиса","Генерация","Полезное","Дом2","Путешествие","Юг","Питерский","Рим","NewTime","Среднеазиатский","Погода","Новости сегодня","Язык","Мир животных","Восток","Ком","Звезда","Вы","Новое","Технологии","Галилео","Интересное","Строительство","Юриспунденция","История","Рыбалка","Море","КТУ","ТНТ","У дяди","Что-то"};
            name = adjectives[random.Next(adjectives.Length)];

            int[] idCitys = dbContext.GetDataTableBySQL($"SELECT id_city FROM city").AsEnumerable().Select(row => row.Field<int>("id")).ToArray();
            int[] idOwnershipTypes = dbContext.GetDataTableBySQL($"SELECT id_type_of_ownership FROM type_of_ownership").AsEnumerable().Select(row => row.Field<int>("id")).ToArray();
            TimeSpan[] start = { TimeSpan.Parse("00:00"), TimeSpan.Parse("00:30"), TimeSpan.Parse("01:00"), TimeSpan.Parse("02:00"), TimeSpan.Parse("03:00"), TimeSpan.Parse("04:00"), TimeSpan.Parse("05:00"), TimeSpan.Parse("06:00"), TimeSpan.Parse("07:00"), TimeSpan.Parse("08:00"), TimeSpan.Parse("09:00"), TimeSpan.Parse("10:00"), TimeSpan.Parse("11:00"), };
            TimeSpan[] end = { TimeSpan.Parse("13:00"), TimeSpan.Parse("14:00"), TimeSpan.Parse("15:00"), TimeSpan.Parse("16:00"), TimeSpan.Parse("17:00"), TimeSpan.Parse("18:00"), TimeSpan.Parse("19:00"), TimeSpan.Parse("20:00"), TimeSpan.Parse("21:00"), TimeSpan.Parse("22:00"), TimeSpan.Parse("23:00"), TimeSpan.Parse("23:59"), };
            int randomIndexst = random.Next(start.Length);
            int randomIndexen = random.Next(end.Length);
            //TimeSpan starttime = start[random.Next(start.Length)];
            //TimeSpan endtime = end[random.Next(end.Length)];

            string[] logins = null;

            if (TakeExisting)
            {
                // Извлечение логинов из DataTable и преобразование их в массив
                logins = dbContext.GetDataTableByTable($"get_free_atc_owners({count})")
                                                    .AsEnumerable()
                                                    .Select(row => row.Field<string>("login"))
                                                    .ToArray();

                string[] genereted_users = GenerateUsers(count - logins.Length, new string[] { "emp_ch" }).Select(x => x.Key).ToArray();

                // Объединение массивов логинов
                logins = logins.Concat(genereted_users).ToArray();
            }
            else
                logins = GenerateUsers(count, new string[] { "emp_ch" }).Select(x => x.Key).ToArray();

            int countCreatedATC = 0;

            try
            {
                int countErrorsToGenerete = 0;

                for (int i = 0; i < logins.Length; i++)
                {
                    string ChannelName = string.Empty;
                    int idCity = idCitys[random.Next(idCitys.Length)];
                    int idOwnershipType = idOwnershipTypes[random.Next(idOwnershipTypes.Length)];
                    TimeSpan starttime = start[random.Next(start.Length)];
                    TimeSpan endtime = end[random.Next(end.Length)];
                    string login = logins[i];

                        ChannelName = adjectives[random.Next(adjectives.Length)];


                    try
                    {
                        dbContext.SendRequest($"INSERT INTO channel (channel_name, fid_city, fid_type_of_ownership, start_work, end_work, fid_employee) VALUES ('{ChannelName}', '{idCity}', '{idOwnershipType}', '{idOwnershipType}', '{starttime}', '{endtime}', '{login}')");

                        countCreatedATC++;
                        countErrorsToGenerete = 0;
                        UpdateProgressBar(i + 1, count);

                        Console.WriteLine($"Был создан канал: {ChannelName} в городе с id: {idCity}.");
                    }
                    catch (Exception ex)
                    {
                        if (countErrorsToGenerete >= MAX_ERRORS_TO_GENERATE)
                            throw new Exception($"Превышено количество попыток генерации записей, последняя ошибка: {ex.Message}");

                        countErrorsToGenerete++;
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return countCreatedATC;
        }

        public int GenerateCustomer(int count)
        {
            //const string path = "FIO";

            string[] fnames = {"Анастасия Федорова",
"Александр Васильев","Степан Никитин","Светлана Захарова","Александр Жаров","Тимофей Богомолов","Екатерина Шапошникова","Дарья Воронина","Милана Муравьева","Мария Иванова",
"Али Денисов","Светлана Филиппова","Ярослав Власов","Полина Владимирова","Лея Короткова","Фатима Логинова","Мирослава Белкина","Алиса Логинова",
"Мирослава Глушкова","Роман Воробьев","Иван Алексеев","Екатерина Грибова","Артём Орлов","Владимир Глебов","Анна Суслова","Артём Миронов","Василиса Завьялова",
"Маргарита Филиппова","Фёдор Панкратов","София Михайлова","Алиса Игнатьева","Ева Афанасьева","Дарья Панина","Елисей Андреев","Арина Петрова","Владимир Ильин","Александр Карпов",
"Лев Севастьянов","Егор Зеленин","Адам Гришин","Вероника Сазонова","Алёна Власова","Артемий Тимофеев","Полина Кудрявцева","София Кузьмина","Даниил Черный","Марк Романов","Мария Андреева",
"Светлана Федорова","Лев Николаев","Софья Петрова","Варвара Короткова","Эмилия Виноградова","Семён Харитонов","Николай Лопатин","Ева Орлова",
"Эмилия Антонова","Эмилия Карпова","Алиса Павлова","Есения Богданова","Мира Белякова","Фёдор Пастухов","Марк Дементьев","Иван Данилов","Анна Жукова","Артём Александров","Мария Ларина",
"Ульяна Кузнецова","Мирон Муратов","Ибрагим Болдырев","Даниил Кузин","Амина Никифорова","Даниэль Соколов","Анна Осипова","Ирина Маркина","Валерия Никитина","Амина Попова","Демьян Крылов",
"Алексей Никитин","Анастасия Мартынова","Даниил Мартынов","София Кузнецова","Таисия Лебедева","Мирон Гладков","Елизавета Пугачева","Дмитрий Федотов","Мария Астахова","Зоя Давыдова","Матвей Попов","Алексей Сычев","Михаил Алехин",
"Артём Сергеев","Василиса Маркелова","Никита Николаев","Елисей Котов","Максим Соловьев","Светлана Виноградова","Георгий Поздняков","Арсений Овчинников","Андрей Субботин"
};

            long[] idCitys = dbContext.GetDataTableBySQL($"SELECT id_city FROM city").AsEnumerable().Select(row => row.Field<long>("id_city")).ToArray();

            int countCreatedDrivers = 0;

            try
            {
                int countErrorsToGenerate = 0;

                for (int i = 0; i < count; i++)
                {
                    string CustomerName = string.Empty;
                    long idCity = idCitys[random.Next(idCitys.Length)];
                    long phone = GenerateRandomPhoneNumber();
                    string address = (string)GenerateRandomAddress();

                    // CustomerName = fnames[random.Next(fnames.Length)];
                    CustomerName = "Дима";



                    try
                    {
                        dbContext.SendRequest($"INSERT INTO customer (name_customer, fid_city, phone_number, address) " +
                                              $"VALUES ('{CustomerName}', '{idCity}', '{phone}', '{address}')");

                        countCreatedDrivers++;
                        countErrorsToGenerate = 0;
                        UpdateProgressBar(i + 1, count);

                        Console.WriteLine($"Customer created:{CustomerName}, City: {idCity}, Phone: {phone}, Address: {address}.");
                    }
                    catch (Exception ex)
                    {
                        if (countErrorsToGenerate >= MAX_ERRORS_TO_GENERATE)
                            throw new Exception($"Exceeded maximum attempts to generate records, last error: {ex.Message}");

                        countErrorsToGenerate++;
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return countCreatedDrivers;
        }

        public int GeneratePossibleOrders(int count)
        {
            long[] idCannel = dbContext.GetDataTableBySQL($"SELECT id_channel FROM channel").AsEnumerable().Select(row => row.Field<long>("id_channel")).ToArray();
            long[] idtime_type = dbContext.GetDataTableBySQL($"SELECT id_time_type FROM time_type").AsEnumerable().Select(row => row.Field<long>("id_time_type")).ToArray();

            var createdpossibleorders = 0;

            try
            {
                int countErrorsToGenerate = 0;

                for (int i = 0; i < count; i++)
                {
                    long numbposord = GenerateRandomPhoneNumber();
                    long fidCannel = idCannel[random.Next(idCannel.Length)];
                    long fidtime_type = idtime_type[random.Next(idtime_type.Length)];

                    try
                    {
                        dbContext.SendRequest($"INSERT INTO possible_orders (number_possible_orders, fid_channel, fid_time_type) " +
                                              $"VALUES ('{numbposord}', '{fidCannel}', '{fidtime_type}')");

                        createdpossibleorders++;
                        countErrorsToGenerate = 0;
                        UpdateProgressBar(i + 1, count);

                        Console.WriteLine($"Possible order created: Numver: {numbposord}, ChannelID: {fidCannel}, IDTimeType: {fidtime_type}.");
                    }
                    catch (Exception ex)
                    {
                        if (countErrorsToGenerate >= MAX_ERRORS_TO_GENERATE)
                            throw new Exception($"Exceeded maximum attempts to generate records, last error: {ex.Message}");

                        countErrorsToGenerate++;
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return createdpossibleorders;
        }


        public int GenerateTransportationTable(int count)
        {
            return GenerateForTransportationsTables(count, "orders");
        }

        public int GenerateTransportationPartitionHash(int count)
        {
            return GenerateForTransportationsTables(count, "transportation_hash");
        }

        public int GenerateTransportationPartitionRange(int count)
        {
            return GenerateForTransportationsTables(count, "transportation_range");
        }

        private int GenerateForTransportationsTables(int count, string table)
        {
            var idCustomers = dbContext.GetDataTableBySQL("SELECT id_customer FROM customer").AsEnumerable()
                            .Select(row => row.Field<long>("id_customer")).ToArray();
            var idCannels = dbContext.GetDataTableBySQL("SELECT id_channel FROM channel").AsEnumerable()
                            .Select(row => row.Field<long>("id_channel")).ToArray();
            var idTimeTipes = dbContext.GetDataTableBySQL("SELECT id_time_type FROM time_type").AsEnumerable()
                .Select(row => row.Field<long>("id_time_type")).ToArray();
            var idPossibles = dbContext.GetDataTableBySQL("SELECT id_possible_orders FROM possible_orders").AsEnumerable()
                .Select(row => row.Field<long>("id_possible_orders")).ToArray();

            var createdOrders = 0;

            try
            {
                int countErrorsToGenerate = 0;

                for (int i = 0; i < count; i++)
                {


                    var idCustomer = idCustomers[random.Next(idCustomers.Length)];
                    var idCannel = idCannels[random.Next(idCannels.Length)];
                    DateTime startDate = new DateTime(1991, 1, 1);
                    var stDate = GenerateRandomDate(startDate, DateTime.Now);
                    
                    var idTimeTipe = idTimeTipes[random.Next(idTimeTipes.Length)];
                    int duration = random.Next(10, 100);
                    long idPossible = idPossibles[random.Next(idPossibles.Length)];
                    long numborder = idPossible;
                        //GenerateRandomPhoneNumber();

                    try
                    {
                        dbContext.SendRequest($"INSERT INTO {table} (fid_customer, fid_channel, order_date, fid_time_type, duration, fid_possible_order, number_order) " +
                                $"VALUES ('{idCustomer}', '{idCannel}', '{stDate}', '{idTimeTipe}', '{duration}', '{idPossible}', '{numborder}')");

                        createdOrders++;
                        countErrorsToGenerate = 0;
                        UpdateProgressBar(i + 1, count);

                        Console.WriteLine($"Order created: Customer: {idCustomer}, Number of order: {numborder}, Channel: {idCannel}, Date: {stDate}, TimeType: {idTimeTipe}, Duration: {duration}, Possible: {idPossible}.");
                    }
                    catch (Exception ex)
                    {
                        if (countErrorsToGenerate >= MAX_ERRORS_TO_GENERATE)
                            throw new Exception($"Exceeded maximum attempts to generate records, last error: {ex.Message}");

                        countErrorsToGenerate++;
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return createdOrders;
        }

        private void UpdateProgressBar(int progress, int finalValue)
        {
            if (updateProgressBar != null)
            {
                int percentProgress = (progress * 100) / finalValue;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    updateProgressBar(percentProgress);
                });
            }
        }


        public DateTime GenerateRandomDate(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("startDate must be earlier than endDate");
            }

            Random random = new Random(this.random.Next(int.MaxValue));
            int range = (endDate - startDate).Days;
            return startDate.AddDays(random.Next(range));
        }

        private long GenerateRandomPhoneNumber()
        {
            long min = 1000000000; // 10-digit phone number
            long max = 9999999999;
            return random.Next((int)min, (int)max);
        }

        private string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Доступные символы
            StringBuilder stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        private string GenerateRandomEmail()
        {
            string[] emailDomains = { "gmail.com", "yahoo.com", "outlook.com", "mail.ru", "yandex.ru", "aol.com", "protonmail.com", "icloud.com", "zoho.com", "gmx.com" };

            return GenerateRandomString(random.Next(3, 20)) + "@" + emailDomains[random.Next(emailDomains.Length)];
        }

        static object GenerateRandomAddress()
        {
            Random random = new Random();

            
            string u_street_type;
            string u_street;
            int u_house_number;
            

            if (random.Next(10) <= 4)
            {
                string[] usStreetNames = { "Клен", "Дуб", "Сосна", "Кедр", "Ельм", "Кентр", "Первый", "Второй", "Третий", "Дальний" };
                string[] usStreetTypes = { "Улица", "Проспект", "Переулок", "Дорога", "Бульвар", "Двор" };
               
                int usHouseNumber = random.Next(1, 1000);
                string usStreetName = usStreetNames[random.Next(usStreetNames.Length)];
                string usStreetType = usStreetTypes[random.Next(usStreetTypes.Length)];

                //return $"{usHouseNumber} {usStreetName} {usStreetType}, {usCity}, {usState}";


                
                u_street_type = usStreetType;
                u_street = usStreetName;
                u_house_number = usHouseNumber;
                
            }
            else
            {
                string[] ruStreetNames = { "Ленина", "Пушкина", "Гагарина", "Советская", "Мира", "Центральная", "Новая", "Садовая", "Школьная" };
                string[] ruStreetTypes = { "ул.", "пр-т.", "пер.", "пл.", "проезд", "бул." };
                
                int ruHouseNumber = random.Next(1, 200);
                string ruStreetName = ruStreetNames[random.Next(ruStreetNames.Length)];
                string ruStreetType = ruStreetTypes[random.Next(ruStreetTypes.Length)];

               
                u_street_type = ruStreetType;
                u_street = ruStreetName;
                u_house_number = ruHouseNumber;
                
            }

            var address = new
            {
                
                street_type = u_street_type,
                street = u_street,
                house_number = u_house_number,
                
            };

            return address;
        }



        private string[] ReadLinesFromFile(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                MessageBox.Show($"An error occurred while reading the file: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return lines.ToArray();
        }

        private string[] GetLinesFromBytes(byte[] bytes)
        {
            // Получаем массив байтов из ресурсов
            byte[] byteArray = bytes;

            // Преобразуем массив байтов в строку, используя определенную кодировку
            string content = Encoding.UTF8.GetString(byteArray); // Используйте правильную кодировку

            // Разделяем строку на массив строк по разделителю (если строки разделены, например, символом новой строки)
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }
    }
    
}
