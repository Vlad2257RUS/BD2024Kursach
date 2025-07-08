using BD2024Kursach.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BD2024Kursach.Operation.TableOperation
{
    /// <summary>
    /// Логика взаимодействия для Data_channel_page.xaml
    /// </summary>
    public partial class Data_channel_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        public long id = -1;

        public Data_channel_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }

        public Data_channel_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                var city = (City_ch_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var typeOwner = (Type_of_ownership_ch_ComboBox.SelectedItem as ComboBoxItem)?.Content;

                dbContext.SendRequest($"SELECT insert_data_channel('{Channel_TextBox.Text}', '{city}', '{typeOwner}', '{StartTimeTextBox.Text}', '{EndTimeTextBox.Text}')");
                MessageBox.Show("Предприятие было успешно добавлено!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Change()
        {
            try
            {
                var city = (City_ch_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var typeOwner = (Type_of_ownership_ch_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var employee = (Employee_ch_ComboBox.SelectedItem as ComboBoxItem)?.Content;

                dbContext.SendRequest($"SELECT update_data_channel('{id}', '{Channel_TextBox.Text}',  '{city}', '{typeOwner}', '{StartTimeTextBox.Text}', '{EndTimeTextBox.Text}')");
                MessageBox.Show("Информация о предприятии была успешно обновлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadData()
        {
            var cities = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT * FROM get_city_info();");
            var typesOwner = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT * FROM get_types_of_ownership_info();");
            

            OperatorTable.SetContentByListDictionary(City_ch_ComboBox, cities, "Город");
            OperatorTable.SetContentByListDictionary(Type_of_ownership_ch_ComboBox, typesOwner, "Тип собственности");
           

            if (id != -1)
            {
                Dictionary<string, 
                    object> channel = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_channel_info() WHERE \"ID\" = {id};");
                Channel_TextBox.Text = Convert.ToString(channel["Название канала"]);

                var cityName = Convert.ToString(channel["Город"]);
                var typeOwner = Convert.ToString(channel["Тип собственности"]);

                //StartTimeTextBox.Text = DateTime.Parse(channel["Время начала работы"].ToString()).TimeOfDay.ToString();
                //EndTimeTextBox.Text = DateTime.Parse(channel["Время окончания работы"].ToString()).TimeOfDay.ToString();

                // Преобразование в объекты DateTime
                DateTime startTime = DateTime.Parse(channel["Время начала работы"].ToString()); 
                DateTime endTime = DateTime.Parse(channel["Время окончания работы"].ToString());

                // Извлечение части времени
                TimeSpan startTimeSpan = startTime.TimeOfDay;
                TimeSpan endTimeSpan = endTime.TimeOfDay;

                // Преобразование в строки
                string startTimeString = startTimeSpan.ToString();
                string endTimeString = endTimeSpan.ToString();

                StartTimeTextBox.Text = Convert.ToString(channel["Время начала работы"]); ;
                EndTimeTextBox.Text = Convert.ToString(channel["Время окончания работы"]); ;

               OperatorTable.SetSelectedItemByContent(City_ch_ComboBox, cityName);

                OperatorTable.SetSelectedItemByContent(Type_of_ownership_ch_ComboBox, typeOwner);
           
            }
        }

    }
}
