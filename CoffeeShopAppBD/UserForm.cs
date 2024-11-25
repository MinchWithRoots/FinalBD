using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CoffeeShopAppBD
{
    public partial class UserForm : Form
    {
        private Class1 db = new Class1();
        private string customerEmail;

        public UserForm(string email)
        {
            InitializeComponent();
            customerEmail = email;
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            try
            {
                db.openConnection();

                string query = @"
                    SELECT Orders.order_id, Orders.order_date, Orders.total
                    FROM Orders
                    INNER JOIN Customers ON Orders.customer_id = Customers.customer_id
                    WHERE Customers.email = @Email";

                SqlCommand command = new SqlCommand(query, db.GetSqlConnection());
                command.Parameters.AddWithValue("@Email", customerEmail);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

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
    }
}
