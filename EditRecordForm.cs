using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CoffeeShopAppBD
{
    public partial class EditRecordForm : Form
    {
        public Dictionary<string, string> UpdatedValues { get; private set; }

        public EditRecordForm(Dictionary<string, string> currentValues)
        {
            InitializeComponent();

            // Заполняем текстовые поля текущими значениями
            foreach (var pair in currentValues)
            {
                var textBox = new TextBox
                {
                    Name = $"txt{pair.Key}",
                    Text = pair.Value,
                    Width = 200
                };

                var label = new Label
                {
                    Text = pair.Key,
                    AutoSize = true
                };

                flowLayoutPanel.Controls.Add(label); // Добавить метку
                flowLayoutPanel.Controls.Add(textBox); // Добавить текстовое поле
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Собираем новые значения
            UpdatedValues = new Dictionary<string, string>();

            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is TextBox textBox)
                {
                    string columnName = textBox.Name.Replace("txt", "");
                    UpdatedValues[columnName] = textBox.Text;
                }
            }

            DialogResult = DialogResult.OK; // Завершаем форму с результатом "OK"
            Close();
        }
    }
}

