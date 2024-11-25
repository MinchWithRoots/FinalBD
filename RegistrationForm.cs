using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeShopAppBD
{
    public partial class RegistrationForm : Form
    {
        private Class1 db = new Class1();
        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim(); // Получение телефона из текстового поля
            string password = txtPassword.Text.Trim();

            // Валидация введенных данных
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            try
            {
                db.openConnection();

                // Проверка, используется ли email
                string checkQuery = "SELECT COUNT(*) FROM Customers WHERE email = @Email";
                SqlCommand checkCommand = new SqlCommand(checkQuery, db.GetSqlConnection());
                checkCommand.Parameters.AddWithValue("@Email", email);

                int count = (int)checkCommand.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Email is already in use.");
                    return;
                }

                // Вставка новой записи
                string query = "INSERT INTO Customers (name, email, phone, password, loyalty_points) VALUES (@Name, @Email, @Phone, @Password, 0)";
                SqlCommand command = new SqlCommand(query, db.GetSqlConnection());
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Phone", phone); // Добавление параметра телефона
                command.Parameters.AddWithValue("@Password", password);

                command.ExecuteNonQuery();

                MessageBox.Show("Registration successful!");
                this.Close();
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

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
    }
}
