using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CoffeeShopAppBD
{
    public partial class LogInForm : Form
    {
        private Class1 db = new Class1();

        public LogInForm()
        {

            InitializeComponent();
        }

       

        private void LogInbtn_Click(object sender, EventArgs e)
        {
            string email = logInTxt.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                db.openConnection();

                // Проверка на администратора
                string adminQuery = "SELECT COUNT(*) FROM Administrators WHERE email = @Email AND password = @Password";
                SqlCommand adminCmd = new SqlCommand(adminQuery, db.GetSqlConnection());
                adminCmd.Parameters.AddWithValue("@Email", email);
                adminCmd.Parameters.AddWithValue("@Password", password);

                int isAdmin = (int)adminCmd.ExecuteScalar();
                if (isAdmin > 0)
                {
                    MessageBox.Show("Login successful as Administrator!");
                    AdminForm adminForm = new AdminForm(); // Форма администратора
                    adminForm.Show();
                    this.Hide();
                    return;
                }

                // Проверка на клиента
                string customerQuery = "SELECT COUNT(*) FROM Customers WHERE email = @Email AND password = @Password";
                SqlCommand customerCmd = new SqlCommand(customerQuery, db.GetSqlConnection());
                customerCmd.Parameters.AddWithValue("@Email", email);
                customerCmd.Parameters.AddWithValue("@Password", password);

                int isCustomer = (int)customerCmd.ExecuteScalar();
                if (isCustomer > 0)
                {
                    MessageBox.Show("Login successful as Customer!");
                    UserForm userForm = new UserForm(email); // Форма клиента
                    userForm.Show();
                    this.Hide();
                    return;
                }

                // Если пользователь не найден
                MessageBox.Show("Invalid email or password.");
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
       

        private void registration_Click(object sender, EventArgs e)
        {
            RegistrationForm frm = new RegistrationForm();
            frm.Show();

        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }
    }
}
