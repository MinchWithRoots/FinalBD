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

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
