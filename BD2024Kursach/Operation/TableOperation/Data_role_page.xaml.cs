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
    /// Логика взаимодействия для Data_role_page.xaml
    /// </summary>
    public partial class Data_role_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        public long id = -1;

        public Data_role_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }


        public Data_role_page(long ID) : this()
        {
            id = ID;
        }

        public void Add()
        {
            try
            {
                dbContext.SendRequest($"SELECT insert_data_role('{NameRoleTextBox.Text}')");
                MessageBox.Show("Роль была успешно добавлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                dbContext.SendRequest($"SELECT update_data_role('{id}', '{NameRoleTextBox.Text}')");
                MessageBox.Show("Роль была успешно обновлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
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
                dynamic type = dbContext.ReadFirstDictionaryRecordFromDatabaseBySQL($"SELECT * FROM get_role_info() WHERE \"ID\" = {id};");
                NameRoleTextBox.Text = type["Тип роли"].ToString();
            }
        }

    }
}
