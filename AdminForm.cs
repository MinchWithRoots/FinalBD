using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace CoffeeShopAppBD
{
    public partial class AdminForm : Form
    {
        private Class1 db = new Class1();

        public AdminForm()
        {
            InitializeComponent();
            // Подписка на событие выбора элемента в ComboBox
            comboTables.SelectedIndexChanged += ComboTables_SelectedIndexChanged;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            // Добавление фильтров в ComboBox
            filtration.Items.Add("Все записи");
            filtration.Items.Add("Недавние заказы");
            filtration.Items.Add("Товары в наличии");
            filtration.Items.Add("Сотрудники на должности 'Manager'");
            filtration.Items.Add("Заказы с суммой больше 50");
            filtration.Items.Add("Дорогие товары (цена > 50)");

            // Подписка на изменение фильтров
            filtration.SelectedIndexChanged += filtration_SelectedIndexChanged;

            // Заполнение списка таблиц
            comboTables.Items.Add("Customers");
            comboTables.Items.Add("Employees");
            comboTables.Items.Add("Positions");
            comboTables.Items.Add("Orders");
            comboTables.Items.Add("OrderItems");
            comboTables.Items.Add("Inventory");
            comboTables.Items.Add("Products");
            comboTables.Items.Add("ProductCategories");
            comboTables.Items.Add("Suppliers");
            comboTables.Items.Add("Shipments");
            comboTables.Items.Add("ShipmentItems");
        }

        private void ComboTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = comboTables.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Выберите таблицу.");
                return;
            }

            try
            {
                db.openConnection();

                // Загрузка данных выбранной таблицы
                string query = $"SELECT * FROM {selectedTable}";
                SqlDataAdapter adapter = new SqlDataAdapter(query, db.GetSqlConnection());
                DataTable table = new DataTable();
                adapter.Fill(table);

                // Отображение данных в DataGridView
                dataGridView.DataSource = table;

                // Скрытие столбцов с ID (кроме Inventory и OrderItems)
                if (selectedTable != "Inventory" && selectedTable != "OrderItems")
                {
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (column.Name.IndexOf("id", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            column.Visible = false; // Скрываем колонку с ID
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                db.closeConnection();
            }
        }

        private void AddMenu_Click(object sender, EventArgs e)
        {
            try
            {
                // Получить текущую таблицу из DataGridView
                DataTable currentTable = dataGridView.DataSource as DataTable;

                if (currentTable == null)
                {
                    MessageBox.Show("Нет данных для добавления строки. Выберите таблицу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Создать новую строку
                DataRow newRow = currentTable.NewRow();

                // Добавить строку в DataTable
                currentTable.Rows.Add(newRow);

                // Установить выделение на новую строку для редактирования
                dataGridView.CurrentCell = dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[0];
                dataGridView.BeginEdit(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении строки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void deleteMenu_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверить, есть ли выделенная строка
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получить выделенную строку
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

                // Получить текущую таблицу из DataGridView
                DataTable currentTable = dataGridView.DataSource as DataTable;

                if (currentTable == null)
                {
                    MessageBox.Show("Нет данных для удаления строки. Выберите таблицу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Подтверждение удаления
                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                                                    "Подтверждение удаления",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (confirmResult != DialogResult.Yes)
                    return;

                // Удаление строки из DataTable
                currentTable.Rows[selectedRow.Index].Delete();

                // Сохранение изменений в базу данных
                using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {comboTables.SelectedItem}", db.GetSqlConnection()))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.Update(currentTable);
                }

                MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении строки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SortMenu_Click(object sender, EventArgs e)
        {
            
        }

        private void filtration_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTableData();
        }

        private void LoadTableData()
        {
            string selectedTable = comboTables.SelectedItem?.ToString();
            string selectedFilter = filtration.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Выберите таблицу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filterQuery = "";

            // Построение фильтра для выбранной таблицы
            if (!string.IsNullOrEmpty(selectedFilter) && selectedFilter != "Все записи")
            {
                switch (selectedTable)
                {
                    case "Orders":
                        if (selectedFilter == "Недавние заказы")
                            filterQuery = "WHERE order_date >= DATEADD(DAY, -30, GETDATE())";
                        else if (selectedFilter == "Заказы с суммой больше 50")
                            filterQuery = "WHERE total > 50";
                        break;

                    case "Products":
                        if (selectedFilter == "Товары в наличии")
                            filterQuery = "WHERE stock > 0";
                        else if (selectedFilter == "Дорогие товары (цена > 50)")
                            filterQuery = "WHERE price > 50";
                        break;

                    case "Employees":
                        if (selectedFilter == "Сотрудники на должности 'Manager'")
                            filterQuery = "WHERE position_id = '2'";
                        break;

                    default:
                        MessageBox.Show("Выбранный фильтр не применим к данной таблице.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }
            }

            try
            {
                db.openConnection();

                // Выполнение запроса с фильтром
                string query = $"SELECT * FROM {selectedTable} {filterQuery}";
                SqlDataAdapter adapter = new SqlDataAdapter(query, db.GetSqlConnection());
                DataTable table = new DataTable();
                adapter.Fill(table);

                // Отображение данных в DataGridView
                dataGridView.DataSource = table;
            
                
           
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.closeConnection();
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView.DataSource is DataTable currentTable && !string.IsNullOrEmpty(comboTables.SelectedItem?.ToString()))
                {
                    string selectedTable = comboTables.SelectedItem.ToString();

                    // Сохранение изменений в базу данных
                    using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {selectedTable}", db.GetSqlConnection()))
                    {
                        SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                        adapter.Update(currentTable);
                    }

                    // Отключение редактирования после сохранения
                    dataGridView.ReadOnly = true;
                    dataGridView.AllowUserToAddRows = false;
                    dataGridView.AllowUserToDeleteRows = false;

                    MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Нет данных для сохранения. Убедитесь, что выбрана таблица и внесены изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editMenu_Click_1(object sender, EventArgs e)
        {
            dataGridView.ReadOnly = false;
            dataGridView.AllowUserToAddRows = true;
            dataGridView.AllowUserToDeleteRows = true;
        }
    }
}
