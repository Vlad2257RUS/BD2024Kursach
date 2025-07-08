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
using System.Windows.Shapes;

namespace BD2024Kursach.Generator
{
    /// <summary>
    /// Логика взаимодействия для GeneratorFrom.xaml
    /// </summary>
    public partial class GeneratorFrom : Window
    {
        public static bool isOpen { get; private set; }

        Generator generator;
        public GeneratorFrom()
        {
            InitializeComponent();

            generator = new Generator();
            generator.updateProgressBar = SetProgressBar;

            isOpen = true;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tables_ComboBox.Items.Clear();

            Tables_ComboBox.Items.Add("Канал");
            Tables_ComboBox.Items.Add("Заказчик");
            Tables_ComboBox.Items.Add("Возможный заказ");
            Tables_ComboBox.Items.Add("Заказ");
            Tables_ComboBox.Items.Add("Сотрудник");

            Tables_ComboBox.Items.Add("Заказ по хешу");
            Tables_ComboBox.Items.Add("Заказ по диапазону");
        }



        private async void Accept_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Tables_ComboBox.SelectedItem == null)
                return;

            int Count = int.Parse(CountGenereteRecord_TextBox.Text);

            string key = Tables_ComboBox.SelectedItem.ToString();

            int CountGenereted = 0;

            await Task.Run(() =>
            {
                switch (key)
                {
                    case "Канал":
                        CountGenereted = generator.GenerateChannel(Count, true);
                        break;

                    case "Заказчик":
                        CountGenereted = generator.GenerateCustomer(Count);
                        break;

                    case "Возможный заказ":
                        CountGenereted = generator.GeneratePossibleOrders(Count);
                        break;

                    case "Заказ":
                        CountGenereted = generator.GenerateTransportationTable(Count);
                        break;

                    case "Сотрудник":
                        CountGenereted = generator.GenerateUsers(Count, new string[] { "administrator", "analyst" }).Count; //"owner_atc", 
                        break;

                    case "По хешу":
                        CountGenereted = generator.GenerateTransportationPartitionHash(Count);
                        break;

                    case "По списку":
                        CountGenereted = generator.GenerateTransportationPartitionRange(Count);
                        break;
                }
            });

            MessageBox.Show($"Было успешно сгенерировано записей\r\nв количестве {CountGenereted} для таблицы \"{key}\"", "Оповещение", 0, MessageBoxImage.Information);
        }


        private void SetProgressBar(int progress)
        {
            Records_ProgressBar.Value = progress;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            isOpen = false;
            Close();
        }

        private void Tables_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Accept_Button.IsEnabled = true;

        private void CountGenereteRecord_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.BrowserBack || char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)))
                return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Accept_Button_Click(null, null);
            }

            e.Handled = true;
        }
    }
}
