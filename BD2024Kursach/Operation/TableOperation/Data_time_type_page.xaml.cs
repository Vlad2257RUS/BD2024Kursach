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
    /// Логика взаимодействия для Data_time_type_page.xaml
    /// </summary>
    public partial class Data_time_type_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        public long id = -1;

        public Data_time_type_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }


        public Data_time_type_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {

                string sql = $"SELECT insert_data_possible_orders('{Timetype_TextBox.Text}', '{Number_of_minutes_TextBox.Text}', '{Cost_per_minute_TextBox.Text}')";

                dbContext.SendRequest(sql);
                MessageBox.Show("Тип времени был успешно добавлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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


                dbContext.SendRequest($"SELECT update_data_time_type('{id}','{Timetype_TextBox.Text}', '{Number_of_minutes_TextBox.Text}', '{Cost_per_minute_TextBox.Text}')");
                MessageBox.Show("Информация о типе времени была успешно обновлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadData()
        {

            if (id != -1)
            {
                dynamic type = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_time_type_info() WHERE \"ID\" = {id};");

                Timetype_TextBox.Text = Convert.ToString(type["Тип времени"]);
                Number_of_minutes_TextBox.Text = Convert.ToString(type["Количество минут"]);
                Cost_per_minute_TextBox.Text = Convert.ToString(type["Цена за минуту"]);


            }
        }
    }
}
