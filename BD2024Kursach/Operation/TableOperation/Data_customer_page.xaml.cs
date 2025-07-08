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
    /// Логика взаимодействия для Data_customer_page.xaml
    /// </summary>
    public partial class Data_customer_page : Page, IDataOP
    {

        DbContext_Npgsql dbContext;
        public long id = -1;
        public Data_customer_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }

        public Data_customer_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                var city = (City_ComboBox.SelectedItem as ComboBoxItem)?.Content;
               

                dbContext.SendRequest($"SELECT insert_data_customer('{Customer_TextBox.Text}', '{city}', '{Phone_TextBox.Text}', '{Address_TextBox.Text}')");
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
                var city = (City_ComboBox.SelectedItem as ComboBoxItem)?.Content;


                dbContext.SendRequest($"SELECT update_data_customer('{id}', '{Customer_TextBox.Text}',  '{city}', '{Phone_TextBox.Text}', '{Address_TextBox.Text}')");
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
            var typesOwner = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT * FROM get_type_of_ownership_info();");

            OperatorTable.SetContentByListDictionary(City_ComboBox, cities, "Город");

            if (id != -1)
            {
                Dictionary<string, object> channel = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_customer_info() WHERE \"ID\" = {id};");
                Customer_TextBox.Text = Convert.ToString(channel["Заказчик"]);

                var cityName = Convert.ToString(channel["Город"]);

                Phone_TextBox.Text = Convert.ToString(channel["Телефон"]);
                Address_TextBox.Text = Convert.ToString(channel["Адрес"]);

                OperatorTable.SetSelectedItemByContent(City_ComboBox, cityName);

            }
        }


    }
}
