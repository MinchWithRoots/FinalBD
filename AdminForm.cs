using System;
using System.Data;
using System.Data.SqlClient;
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
            // Загрузить доступные таблицы
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

        private void EditMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверить, есть ли выделенная строка
                if (dataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите строку для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получить выбранную строку
                DataGridViewRow selectedRow = dataGridView.SelectedRows[0];

                // Проверить, привязан ли DataGridView к DataTable
                if (dataGridView.DataSource is DataTable currentTable)
                {
                    // Переключить строку в режим редактирования
                    dataGridView.BeginEdit(true);

                    // После завершения редактирования и выхода из ячейки обновить данные в базе данных
                    dataGridView.CellEndEdit += (s, args) =>
                    {
                        try
                        {
                            // Сохранить изменения в базу данных
                            using (SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {comboTables.SelectedItem}", db.GetSqlConnection()))
                            {
                                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                                adapter.Update(currentTable); // Сохраняем изменения из DataTable в базу данных
                            }

                            MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };
                }
                else
                {
                    MessageBox.Show("Нет данных для редактирования. Выберите таблицу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboTables_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
