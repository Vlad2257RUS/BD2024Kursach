using BD2024Kursach.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Data_employee_page.xaml
    /// </summary>
    public partial class Data_employee_page : Page, IDataOP
    {
        DbContext_Npgsql dbContext;
        
        public Data_employee_page()
        {
            InitializeComponent();

            dbContext = DbContext_Npgsql.GetInstance();
        }


        public void Add()
        {
            try
            {
                string role = (Roles_ComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

                //string user_data = "{" + $"\"age\": {Age_TestBox.Text}," + $"\"email\": {Email_TestBox.Text}," + $"\"phone\": +{Phone_TestBox.Text}," + $"\"address\": {Address_TestBox.Text}" + "}";
                
                var dataObject = new
                {
                    age = Age_TestBox.Text,
                    email = Email_TestBox.Text,
                    phone = "+" + Phone_TestBox.Text,
                    address = Address_TestBox.Text
                };

                string user_data = JsonSerializer.Serialize(dataObject);



                dbContext.SendRequest($"SELECT create_new_user_by_role('{Login_TextBox.Text}', '{Password_TextBox.Password}', '{role}', '{Employee_TextBox.Text}')");
                MessageBox.Show("Пользователь был успешно добавлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Change()
        {

        }

        public void LoadData()
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Проверяем, что вводимый символ - цифра
            if (!IsNumeric(e.Text))
            {
                e.Handled = true; // Отменяем ввод недопустимого символа
            }
        }

        // Метод для проверки, является ли строка числом
        private bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]+$");
        }
    }
}
