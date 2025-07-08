using BD2024Kursach.Operation.TableOperation;
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

namespace BD2024Kursach.Operation
{
    /// <summary>
    /// Логика взаимодействия для InsertUpdate_Page.xaml
    /// </summary>
    public partial class InsertUpdate_Page : Page
    {
        Page contextPage;

        // Приватный конструктор, инициализирующий страницу
        private InsertUpdate_Page()
        {
            InitializeComponent();
        }

        // Публичный конструктор с параметром страницы
        public InsertUpdate_Page(Page page) : this()
        {
            // Проверка страницы
            CheckPage(page);

            // Установка контекстной страницы
            contextPage = page;
        }

        // Публичный конструктор с параметром имени таблицы
        public InsertUpdate_Page(string tableName) : this()
        {
            switch (tableName)
            {
                case "city":
                    contextPage = new Data_city_page();
                    break;

                case "channel":
                    contextPage = new Data_channel_page();
                    break;

                case "type_of_ownership":
                    contextPage = new Data_type_of_ownership_page();
                    break;

                case "customer":
                    contextPage = new Data_customer_page();
                    break;

                case "employee":
                    contextPage = new Data_employee_page();
                    break;

                case "orders":
                    contextPage = new Data_orders_page();
                    break;

                case "possible_orders":
                    contextPage = new Data_possible_orders_page();
                    break;

                case "time_type":
                    contextPage = new Data_time_type_page();
                    break;

                case "role":
                    contextPage = new Data_role_page();
                    break;

            }

            // Проверка страницы
            CheckPage(contextPage);
        }

        public InsertUpdate_Page(string tableName, long ID) : this()
        {
            switch (tableName)
            {
                case "city":
                    contextPage = new Data_city_page(ID);
                    break;

                case "channel":
                    contextPage = new Data_channel_page(ID);
                    break;

                case "type_of_ownership":
                    contextPage = new Data_type_of_ownership_page(ID);
                    break;

                case "customer":
                    contextPage = new Data_customer_page(ID);
                    break;

                case "employee":
                    contextPage = new Data_employee_page();
                    break;

                case "orders":
                    contextPage = new Data_orders_page(ID);
                    break;

                case "possible_orders":
                    contextPage = new Data_possible_orders_page(ID);
                    break;

                case "time_type":
                    contextPage = new Data_time_type_page(ID);
                    break;

                case "role":
                    contextPage = new Data_role_page(ID);
                    break;

            }


            // Проверка страницы
            CheckPage(contextPage);
            ChangeVisibilityToAccept();
        }

        // Метод для проверки корректности страницы
        private void CheckPage(Page page)
        {
            if (page as IDataOP == null)
                throw new ArgumentException("Переданная страница не относится к IDataOP!");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataOP_Frame.Navigate(contextPage);
        }

        private void ChangeVisibilityToAccept()
        {
            AddRecordButton.Visibility = Visibility.Collapsed;
            ChangeRecordButton.Visibility = Visibility.Visible;
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var insert = contextPage as IDataOP;
            insert.Add();
        }

        private void ChangeRecordButton_Click(object sender, RoutedEventArgs e)
        {
            var update = contextPage as IDataOP;
            update.Change();
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
            NavigationService.RemoveBackEntry();
        }

        private void DataOP_Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var page = contextPage as IDataOP;
            page.LoadData();
        }
    }
}
