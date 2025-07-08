﻿using BD2024Kursach;
using BD2024Kursach.Context;
using BD2024Kursach.Operation;
using BD2024Kursach.Operation.TableOperation;
using BD2024Kursach.Operation.RequestOperation;
using Microsoft.Win32;
using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using BD2024Kursach.Generator;

namespace BD2024Kursach
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbContext_Npgsql DbContext;

        DB_Content_Page dbContentPage;
        public MainWindow()
        {
            InitializeComponent();

            DbContext = DbContext_Npgsql.GetInstance();
        }

        private string SelectedTable
        {
            get
            {
                var comboBoxItem = AvailableTablesComboBox.SelectedValue as ComboBoxItem;

                if (comboBoxItem == null)
                    return null;

                var key = comboBoxItem.Tag as string;

                return key;
            }
        }


        private bool ForOwner_ch
        {
            get
            {
                return
                    SelectedTable == "channel" &&
                    DbContext.User != "postgres" &&
                    DbContext.ExecuteScalar<bool>("SELECT pg_has_role(CURRENT_USER, 'emp_ch', 'MEMBER')");
            }
        }
        
        ComboBoxItem SelectedColumn
        {
            get
            {
                return ColumnsComboBox.SelectedValue as ComboBoxItem;
            }
        }


        private void CurrentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dbContentPage = new DB_Content_Page();
            dbContentPage.bySelect = LoadRequestPage;

            MainFrame.Navigate(dbContentPage);
            LoadData();
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ChangePagination();
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTable == null)
            {
                MessageBox.Show("Выбирите таблицу перед тем как добавлять в нее записи.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //var form = new dataOP_transportation_Page();
            //MainFrame.Navigate(new InsertUpdate_Page(form));

            //MainFrame.Source = new Uri($"OperationPages\\TablePages\\dataOP_{SelectedTable}_Page.xaml", UriKind.Relative);

            MainFrame.Navigate(new InsertUpdate_Page(SelectedTable));
        }

        private void ChangeRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTable == null)
            {
                MessageBox.Show("Выбирите таблицу перед тем как изменять в ней записи.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            long ID = dbContentPage.GetIDSelectedRecord();

            if (ID == -1)
            {
                MessageBox.Show("Выбирите запись которую хотите поменять.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MainFrame.Navigate(new InsertUpdate_Page(SelectedTable, ID));
        }

        private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTable == null)
            {
                MessageBox.Show("Выбирите таблицу перед тем как удалять в ней записи.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            long ID = dbContentPage.GetIDSelectedRecord();

            if (ID == -1)
            {
                MessageBox.Show("Выбирите запись которую хотите удалить.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var table = DbContext.GetDataTableByTable($"count_cascading_deletions_{SelectedTable}('{ID}')");

                string result = string.Empty;

                string CountRecordToDelete = string.Empty;

                foreach (DataRow row in table.Rows)
                {
                    string tableName = row["table_name"].ToString();
                    string countDeleted = row["count_deleted"].ToString();

                    if (tableName == "Всего")
                    {
                        CountRecordToDelete = countDeleted;
                    }

                    result += $"{tableName}: {countDeleted}\r\n";
                }

                if (MessageBox.Show($"Вы уверены что хоите удалить запись?\r\nБудут удалены записи в таблицах:\r\n{result}", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DbContext.SendRequest($"SELECT delete_data_{SelectedTable}('{ID}')");
                    MessageBox.Show($"Было успешно удалено {CountRecordToDelete} записей.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshRecordButton_Click(object sender, RoutedEventArgs e) => dbContentPage.RefreshData();

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            while (MainFrame.CanGoBack)
                MainFrame.GoBack();

            while (MainFrame.NavigationService.RemoveBackEntry() != null) ;

            RefreshRecordButton_Click(null, null);
        }

        private void LoadData()
        {
            if (DbContext.User == "postgres")
                OpenGenerator_Button.Visibility = Visibility.Visible;
            else
                OpenGenerator_Button.Visibility = Visibility.Collapsed;

            if (DbContext.CheckRoleUser(DbContext_Npgsql.Roles.Analyst) || DbContext.CheckRoleUser(DbContext_Npgsql.Roles.EmpCH))
                SaveTheReport_Button.Visibility = Visibility.Visible;
            else
                SaveTheReport_Button.Visibility = Visibility.Collapsed;

            LoadTables();
        }

        private void LoadTables() => OperatorTable.SetTablesContentInComboBox(AvailableTablesComboBox);

        private void AvailableTablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var dt = DbContext.GetDataTableByTable($"get_{SelectedTable}_info()");

            dbContentPage.DataSelection.OrderByColumn = null;
            dbContentPage.DataSelection.sort = null;
            dbContentPage.DataSelection.ValueBySearch = null;

            SetContentByDataTable(true);

            if (SelectedTable != null)
            {
                if (ForOwner_ch)
                {
                    AddRecordButton.IsEnabled = false;
                    ChangeRecordButton.IsEnabled = false;
                    DeleteRecordButton.IsEnabled = false;
                    RefreshRecordButton.IsEnabled = false;
                    BackButton.IsEnabled = false;
                }
                else
                {
                    AddRecordButton.IsEnabled = SelectedTable != null;
                    ChangeRecordButton.IsEnabled = SelectedTable != null;
                    DeleteRecordButton.IsEnabled = SelectedTable != null;
                    RefreshRecordButton.IsEnabled = SelectedTable != null;
                    BackButton.IsEnabled = SelectedTable != null;
                }
            }
        }

        private void SetContentByDataTable(bool ForResetColumnsName)
        {
            if (ForOwner_ch)
            {
                long idAtc = DbContext.ExecuteScalar<long>("SELECT id_channel FROM channel");

                MainFrame.Navigate(new InsertUpdate_Page(SelectedTable, idAtc));
                ColumnsComboBox.Items.Clear();
                return;
            }
            else
            {
                dbContentPage.SetDataGridByTableName($"get_{SelectedTable}_info()");

                //BackButton.IsEnabled = false;
            }

            LoadRequestPage();

            CountRecordsLable.Content = "Записей: " + dbContentPage.CountRecords;

            if (ForResetColumnsName)
            {
                SetColumnsNameInColumnsComboBox(dbContentPage.Columns);
            }

            BackButton_Click(null, null);
        }

        private void SetColumnsNameInColumnsComboBox(DataColumnCollection Columns)
        {
            ColumnsComboBox.Items.Clear();

            foreach (DataColumn column in Columns)
            {
                if (column.ColumnName != "ID")
                {
                    ColumnsComboBox.Items.Add(new ComboBoxItem()
                    {
                        Tag = column.DataType,
                        Content = column.ColumnName
                    });
                }
            }
        }

        private void LoadRequestPage(long ID = -1)
        {
            Page contextPage = null;

            switch (SelectedTable)
            {
                case "city":
                    if (ID == -1)
                        contextPage = null;
                    else
                        contextPage = null;
                    break;

                case "time_type":
                    contextPage = null;
                    break;

                case "type_of_ownership":
                    contextPage = null;
                    break;

                case "channel":
                    contextPage = null;
                    break;

                case "employee":
                    contextPage = null;
                    break;

                case "orders":
                    contextPage = null;
                    break;

                case "customer":
                    if (ID == -1)
                        contextPage = null;
                    else
                        contextPage = null;
                    break;

                case "possible_orders":
                    if (ID == -1)
                        contextPage = null;
                    else
                        contextPage = null;
                    break;

                

                default:
                    return;
            }

            if (contextPage == null)
            {
                while (RequestFrame.CanGoBack)
                    RequestFrame.GoBack();

                RequestFrame.Navigate(null);

                return;
            }

            var reqPage = contextPage as IRequest;

            if (reqPage == null)
                throw new ArgumentException("Переданная страница не относится к IRequestPage!");

            reqPage.LoadData();

            RequestFrame.Navigate(contextPage);
        }

        private void SearchDataByColumnButton_Click(object sender, RoutedEventArgs e)
        {
            var nameColumn = SelectedColumn.Content.ToString();

            //var SQL = $"SELECT * FROM get_{SelectedTable}_info() WHERE \"{nameColumn}\" = '{ValueBySearchTextBox.Text}'";

            dbContentPage.DataSelection.OrderByColumn = nameColumn;
            dbContentPage.DataSelection.sort = null;
            dbContentPage.DataSelection.ValueBySearch = ValueBySearchTextBox.Text;

            SetContentByDataTable(false);
        }

        private void ValueBySearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueBySearchTextBox.Text.Length == 0)
            {
                SearchDataByColumnButton.IsEnabled = false;
            }
            else
            {
                SearchDataByColumnButton.IsEnabled = true;
            }
        }

        private void ValueBySearchTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (SelectedColumn == null)
            {
                e.Handled = true;
                return;
            }

            // Получаем тип данных из Tag
            Type columnType = SelectedColumn.Tag as Type;

            if (columnType != null)
            {
                // Получаем позицию курсора в TextBox
                int cursorPosition = ValueBySearchTextBox.SelectionStart;

                // Получаем текущий текст в TextBox
                string currentText = ValueBySearchTextBox.Text;

                // Вставляем введенный символ в текущий текст на позицию курсора
                string finalText = currentText.Substring(0, cursorPosition) + e.Text + currentText.Substring(cursorPosition);

                // Проверяем, является ли тип числовым (double, int и т.д.)
                if (columnType == typeof(double) || columnType == typeof(float) || columnType == typeof(decimal))
                {
                    // Паттерн для проверки числа с точкой
                    Regex regex = new Regex(@"^-?(\d+|\d+\.|\d+\.\d+)$");
                    e.Handled = regex.IsMatch(finalText);
                }
                else if (columnType == typeof(int) || columnType == typeof(long) || columnType == typeof(short))
                {
                    // Паттерн для проверки числа
                    Regex regex = new Regex(@"^(-?\d+|-)$");
                    e.Handled = !regex.IsMatch(finalText);
                }
            }
        }

        private void ColumnsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueBySearchTextBox.Text = string.Empty;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                if (e.Delta > 0)
                {
                    // Прокрутка вверх
                    scrollViewer.LineUp();
                }
                else
                {
                    // Прокрутка вниз
                    scrollViewer.LineDown();
                }
                e.Handled = true;
            }
        }

        private void OpenGenerator_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Generator.GeneratorFrom.isOpen == false) new GeneratorFrom().Show();
        }

        private async void SaveTheReport_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файлы Excel (*.xlsx, *.xls)|*.xlsx;*.xls|Все файлы (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "Общий отчет"; // Начальное имя файла
            saveFileDialog.DefaultExt = ".xlsx"; // Расширение файла по умолчанию

            // Обработка результата диалога
            if (saveFileDialog.ShowDialog() != true)
                return;

            string filePath = saveFileDialog.FileName;

            await Task.Run(() =>
            {
                try
                {
                    ExcelManager excelManager = new ExcelManager();

                    excelManager.AddDataAndChartByTableName("get_top_time_tipe_with_cost()", "Самые дорогие типы времени");
                   // excelManager.AddDataAndChartByTableName("get_top_time_tipe_most_number_of_minuts()", "Типы времени с наибольшей продолжительностью рекламы");
                    //excelManager.AddDataAndChartByTableName("get_top_cities_with_most_customers()", "Города в которых больше всего заказчиков");

                    if (DbContext.CheckRoleUser(DbContext_Npgsql.Roles.Analyst))
                        excelManager.AddDataAndChartByTableName("get_top_cities_with_most_customers()", "Города в которых больше всего заказчиков");
                    //excelManager.AddDataAndChartByTableName("get_top_atcs_with_most_deliveries()", "Каналы, у которых больше всего заказов");

                    excelManager.DocumentSaveAs(filePath); //"Document.xlsx"

                    MessageBox.Show("Отчет был успешно создан!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            });
        }


    }
}
