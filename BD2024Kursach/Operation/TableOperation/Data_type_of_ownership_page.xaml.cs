using BD2024Kursach.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для Data_type_of_ownership_page.xaml
    /// </summary>
    public partial class Data_type_of_ownership_page : Page, IDataOP
    {

        DbContext_Npgsql dbContext;
        public long id = -1;
        public Data_type_of_ownership_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }



        public Data_type_of_ownership_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                dbContext.SendRequest($"SELECT insert_data_type_of_ownership('{NameTypeTextBox.Text}')");
                MessageBox.Show("Тип собственности был успешно добавлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                dbContext.SendRequest($"SELECT update_data_type_of_ownership('{id}', '{NameTypeTextBox.Text}')");
                MessageBox.Show("Тип собственности был успешно обновлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                dynamic type = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_type_of_ownership_info() WHERE \"ID\" = {id};");
                NameTypeTextBox.Text = type["Тип собственности"].ToString();
            }
        }

    }
}
