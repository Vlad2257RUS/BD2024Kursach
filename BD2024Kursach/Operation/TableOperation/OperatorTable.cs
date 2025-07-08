using BD2024Kursach.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace BD2024Kursach.Operation.TableOperation
{
    internal class OperatorTable
    {

        public static void SetSelectedItemByContent(ComboBox comboBox, string Content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == Content)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        public static void SetContentByListDictionary(ComboBox comboBox, List<Dictionary<string, object>> listDictionaries, string ContentName)
        {
            if (comboBox.Items.Count > 0)
                comboBox.Items.Clear();

            foreach (var dictionary in listDictionaries)
            {
                var item = new ComboBoxItem()
                {
                    Tag = dictionary["ID"],
                    Content = dictionary[ContentName]
                };

                comboBox.Items.Add(item);
            }
        }

        public static void SetTablesContentInComboBox(ComboBox comboBox)
        {
            var dbContext = DbContext_Npgsql.GetInstance();

            var availableTables = dbContext.ReadObjectsFromDatabaseByTable("get_available_tables_between_role()");

            comboBox.Items.Clear();

            foreach (var table in availableTables)
            {
                var key = table.Таблицы;

                var comboBoxItem = new ComboBoxItem { Tag = key };

                switch (key)
                {

                    case "employee":
                        comboBoxItem.Content = "Сотрудник";
                        break;

                    case "customer":
                        comboBoxItem.Content = "Заказчик";
                        break;

                    case "orders":
                        comboBoxItem.Content = "Заказ";
                        break;

                    case "type_of_ownership":
                        comboBoxItem.Content = "Тип собственности";
                        break;

                    case "city":
                        comboBoxItem.Content = "Город";
                        break;

                    case "time_type":
                        comboBoxItem.Content = "Тип времени";
                        break;

                   // case "role":
                   //     comboBoxItem.Content = "Тип роли";
                   //     break;

                    case "channel":
                        comboBoxItem.Content = "Канал";
                        break;

                    case "possible_orders":
                        comboBoxItem.Content = "Возможный заказ";
                        break;

                    default:
                        continue;
                }

                comboBox.Items.Add(comboBoxItem);
            }
        }

        public static void SetContentDataGridByDataTable(DataGrid dg, DataTable dt)
        {
            // Применяем DataTable к ItemsSource DataGrid
            dg.ItemsSource = dt.DefaultView;

            // Настраиваем формат отображения даты для всех столбцов, содержащих тип данных DateTime
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    var dateColumn = dg.Columns.FirstOrDefault(c => c.Header.ToString() == column.ColumnName);
                    if (dateColumn != null && dateColumn is DataGridTextColumn)
                    {
                        // Задаем формат отображения даты
                        ((DataGridTextColumn)dateColumn).Binding.StringFormat = "dd.MM.yyyy";
                    }
                }
            }

            // Настраиваем формат отображения времени для всех столбцов, содержащих тип данных DateSpan
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(TimeSpan))
                {
                    var dateColumn = dg.Columns.FirstOrDefault(c => c.Header.ToString() == column.ColumnName);
                    if (dateColumn != null && dateColumn is DataGridTextColumn)
                    {
                        // Задаем формат отображения времени
                        ((DataGridTextColumn)dateColumn).Binding.StringFormat = "hh:mm:ss";
                    }
                }
            }



            // Прячем столбец с именем "ID"
            var idColumn = dg.Columns.FirstOrDefault(c => c.Header.ToString() == "ID");
            if (idColumn != null)
            {
                idColumn.Visibility = Visibility.Collapsed;
            }
        }

    }
}
