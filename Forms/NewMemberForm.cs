using System;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class NewMemberForm : Form
    {
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhone;
        private DateTimePicker dtpBirthDate;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblPhone;
        private Label lblBirthDate;
        private Label lblAddress;

        public NewMemberForm()
        {
            InitializeComponent();
            this.Text = "Yangi a'zo qo'shish";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 350);

            // Labels
            lblFirstName = new Label();
            lblFirstName.Text = "Ism:";
            lblFirstName.Location = new System.Drawing.Point(20, 20);
            lblFirstName.AutoSize = true;

            lblLastName = new Label();
            lblLastName.Text = "Familiya:";
            lblLastName.Location = new System.Drawing.Point(20, 60);
            lblLastName.AutoSize = true;

            lblPhone = new Label();
            lblPhone.Text = "Telefon:";
            lblPhone.Location = new System.Drawing.Point(20, 100);
            lblPhone.AutoSize = true;

            lblBirthDate = new Label();
            lblBirthDate.Text = "Tug'ilgan sana:";
            lblBirthDate.Location = new System.Drawing.Point(20, 140);
            lblBirthDate.AutoSize = true;

            lblAddress = new Label();
            lblAddress.Text = "Manzil:";
            lblAddress.Location = new System.Drawing.Point(20, 180);
            lblAddress.AutoSize = true;

            // TextBoxes and DateTimePicker
            txtFirstName = new TextBox();
            txtFirstName.Location = new System.Drawing.Point(120, 20);
            txtFirstName.Size = new System.Drawing.Size(200, 20);

            txtLastName = new TextBox();
            txtLastName.Location = new System.Drawing.Point(120, 60);
            txtLastName.Size = new System.Drawing.Size(200, 20);

            txtPhone = new TextBox();
            txtPhone.Location = new System.Drawing.Point(120, 100);
            txtPhone.Size = new System.Drawing.Size(200, 20);

            dtpBirthDate = new DateTimePicker();
            dtpBirthDate.Location = new System.Drawing.Point(120, 140);
            dtpBirthDate.Size = new System.Drawing.Size(200, 20);
            dtpBirthDate.Format = DateTimePickerFormat.Short;

            txtAddress = new TextBox();
            txtAddress.Location = new System.Drawing.Point(120, 180);
            txtAddress.Size = new System.Drawing.Size(200, 20);
            txtAddress.Multiline = true;
            txtAddress.Height = 60;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Saqlash";
            btnSave.Location = new System.Drawing.Point(120, 260);
            btnSave.Click += new EventHandler(btnSave_Click);

            btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new System.Drawing.Point(220, 260);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblFirstName, lblLastName, lblPhone, lblBirthDate, lblAddress,
                txtFirstName, txtLastName, txtPhone, dtpBirthDate, txtAddress,
                btnSave, btnCancel
            });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new ApplicationDbContext())
                {
                    var member = new Member
                    {
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        PhoneNumber = txtPhone.Text,
                        DateOfBirth = dtpBirthDate.Value,
                        Address = txtAddress.Text,
                        RegistrationDate = DateTime.Now,
                        IsActive = true
                    };

                    context.Members.Add(member);
                    context.SaveChanges();

                    MessageBox.Show("A'zo muvaffaqiyatli qo'shildi!", "Muvaffaqiyat", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Iltimos, ismni kiriting!", "Xato", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Iltimos, familiyani kiriting!", "Xato", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Iltimos, telefon raqamini kiriting!", "Xato", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
