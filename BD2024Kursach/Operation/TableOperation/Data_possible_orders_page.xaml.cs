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
    /// Логика взаимодействия для Data_possible_orders_page.xaml
    /// </summary>
    public partial class Data_possible_orders_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        public long id = -1;

        public Data_possible_orders_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }

        public Data_possible_orders_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                
                var channel = (Channel_po_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var time_tipe = (Type_time_po_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                



                string sql = $"SELECT insert_data_possible_orders('{NumberPossible_TextBox.Text}', '{channel}', '{time_tipe}')";

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
                
                var channel = (Channel_po_ComboBox.SelectedItem as ComboBoxItem)?.Content;
                var time_tipe = (Type_time_po_ComboBox.SelectedItem as ComboBoxItem)?.Content;


                dbContext.SendRequest($"SELECT update_data_orders('{id}', '{NumberPossible_TextBox.Text}', '{channel}','{time_tipe}')");
                MessageBox.Show("Информация о заказе была успешно обновлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadData()
        {
            
            var channel_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Название канала\"  AS \"Название канала\" FROM get_channel_info();");
            var time_type_s = dbContext.ReadDictionaryFromDatabaseBySQL($"SELECT \"ID\", \"Тип времени\"  AS \"Тип времени\" FROM get_time_type_info();");
            

            OperatorTable.SetContentByListDictionary(Channel_po_ComboBox, channel_s, "Название канала");
            OperatorTable.SetContentByListDictionary(Type_time_po_ComboBox, time_type_s, "Тип времени");



            if (id != -1)
            {
                var orders = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_possible_orders_info() WHERE \"ID\" = {id};");

                NumberPossible_TextBox.Text = Convert.ToString(orders["Номер возможного заказа"]);

                var channel = Convert.ToString(orders["Название канала"]);
                var TimeType = Convert.ToString(orders["Тип времени"]);

                OperatorTable.SetSelectedItemByContent(Channel_po_ComboBox, channel);
                OperatorTable.SetSelectedItemByContent(Type_time_po_ComboBox, TimeType);

            }
        }

    }
}
