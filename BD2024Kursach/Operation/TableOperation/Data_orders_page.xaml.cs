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
    /// Логика взаимодействия для Data_orders_page.xaml
    /// </summary>
    public partial class Data_orders_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        public long id = -1;

        public Data_orders_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }


        public Data_orders_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                var customer = (Customer_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var channel = (Channel_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var orderData = Order_DatePicker.SelectedDate?.ToString("dd.MM.yyyy");
                var time_tipe = (Time_type_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var possibleordernumber = (Possible_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;


                
                string sql = $"SELECT insert_data_orders('{NumberOrder_TextBox.Text}', '{customer}', '{channel}', '{orderData}', '{time_tipe}', '{Duration_TextBox.Text}', '{possibleordernumber}')";

                dbContext.SendRequest(sql);
                MessageBox.Show("Заказ был успешно добавлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var customer = (Customer_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var channel = (Channel_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var orderData = Order_DatePicker.SelectedDate?.ToString("dd.MM.yyyy");
                var time_tipe = (Time_type_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var possibleordernumber = (Possible_o_ComboBox.SelectedItem as ComboBoxItem)?.Content;

                dbContext.SendRequest($"SELECT update_data_orders('{id}', '{NumberOrder_TextBox.Text}', '{customer}', '{channel}', '{orderData}', '{time_tipe}', '{Duration_TextBox.Text}', '{possibleordernumber}')");
                MessageBox.Show("Информация о заказе была успешно обновлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadData()
        {
            var customer_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Заказчик\"  AS \"Заказчик\" FROM get_customer_info();");
            var channel_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Канал\"  AS \"Канал\" FROM get_channel_info();");
            var time_type_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Тип времени\"  AS \"Тип времени\" FROM get_time_type_info();");
            var possibleordernumber_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Номер возможного заказа\"  AS \"Номер возможного заказа\" FROM get_possible_orders_info();");

            OperatorTable.SetContentByListDictionary(Customer_o_ComboBox, customer_s, "Заказчик");
            OperatorTable.SetContentByListDictionary(Channel_o_ComboBox, channel_s, "Канал");
            OperatorTable.SetContentByListDictionary(Time_type_o_ComboBox, time_type_s, "Тип времени");
            OperatorTable.SetContentByListDictionary(Possible_o_ComboBox, possibleordernumber_s, "Номер возможного заказа");
           

            if (id != -1)
            {
                var orders = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_orders_info() WHERE \"ID\" = {id};");

                NumberOrder_TextBox.Text = Convert.ToString(orders["Номер заказа"]);
                var customer = Convert.ToString(orders["Заказчик"]);
                var channel = Convert.ToString(orders["Канал"]);
                Order_DatePicker.SelectedDate = DateTime.Parse(orders["Дата заказа"].ToString());
                var TimeType = Convert.ToString(orders["Тип времени"]);
                Duration_TextBox.Text = Convert.ToString(orders["Стоимость перевозки"]);
                var possible = Convert.ToString(orders["Номер возможного заказа"]);
                

                OperatorTable.SetSelectedItemByContent(Customer_o_ComboBox, customer);
                OperatorTable.SetSelectedItemByContent(Channel_o_ComboBox, channel);
                OperatorTable.SetSelectedItemByContent(Time_type_o_ComboBox, TimeType);
                OperatorTable.SetSelectedItemByContent(Possible_o_ComboBox, possible);
            }
        }
    }
}
