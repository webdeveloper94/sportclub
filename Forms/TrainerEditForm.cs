using System;
using System.Drawing;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace SportCenter.Forms
{
    public partial class TrainerEditForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly Trainer _trainer;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private NumericUpDown numAge;
        private NumericUpDown numMonthlyFee;
        private Button btnSave;
        private Button btnCancel;

        public TrainerEditForm(Trainer trainer = null)
        {
            _context = new ApplicationDbContext();
            _trainer = trainer ?? new Trainer();
            InitializeComponent();
            LoadTrainerData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 300);
            this.Text = _trainer.Id == 0 ? "Yangi trener" : "Trener ma'lumotlarini o'zgartirish";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // First Name
            var lblFirstName = new Label();
            lblFirstName.Text = "Ismi:";
            lblFirstName.Location = new Point(20, 20);
            lblFirstName.AutoSize = true;

            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(20, 40);
            txtFirstName.Size = new Size(340, 20);

            // Last Name
            var lblLastName = new Label();
            lblLastName.Text = "Familiyasi:";
            lblLastName.Location = new Point(20, 70);
            lblLastName.AutoSize = true;

            txtLastName = new TextBox();
            txtLastName.Location = new Point(20, 90);
            txtLastName.Size = new Size(340, 20);

            // Age
            var lblAge = new Label();
            lblAge.Text = "Yoshi:";
            lblAge.Location = new Point(20, 120);
            lblAge.AutoSize = true;

            numAge = new NumericUpDown();
            numAge.Location = new Point(20, 140);
            numAge.Size = new Size(100, 20);
            numAge.Minimum = 18;
            numAge.Maximum = 100;

            // Monthly Fee
            var lblMonthlyFee = new Label();
            lblMonthlyFee.Text = "Oylik to'lov:";
            lblMonthlyFee.Location = new Point(20, 170);
            lblMonthlyFee.AutoSize = true;

            numMonthlyFee = new NumericUpDown();
            numMonthlyFee.Location = new Point(20, 190);
            numMonthlyFee.Size = new Size(150, 20);
            numMonthlyFee.Minimum = 0;
            numMonthlyFee.Maximum = 10000000;
            numMonthlyFee.Increment = 10000;
            numMonthlyFee.ThousandsSeparator = true;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Saqlash";
            btnSave.Location = new Point(190, 220);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new Point(280, 220);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] {
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblAge, numAge,
                lblMonthlyFee, numMonthlyFee,
                btnSave, btnCancel
            });
        }

        private void LoadTrainerData()
        {
            if (_trainer.Id != 0)
            {
                txtFirstName.Text = _trainer.FirstName;
                txtLastName.Text = _trainer.LastName;
                numAge.Value = _trainer.Age;
                numMonthlyFee.Value = _trainer.MonthlyFee;
            }
            else
            {
                numAge.Value = 25;
                numMonthlyFee.Value = 500000;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || 
                    string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Iltimos, barcha maydonlarni to'ldiring!", 
                        "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _trainer.FirstName = txtFirstName.Text.Trim();
                _trainer.LastName = txtLastName.Text.Trim();
                _trainer.Age = (int)numAge.Value;
                _trainer.MonthlyFee = numMonthlyFee.Value;

                if (_trainer.Id == 0)
                {
                    _context.Trainers.Add(_trainer);
                }
                else
                {
                    var entry = _context.Entry(_trainer);
                    if (entry.State == EntityState.Detached)
                    {
                        _context.Trainers.Attach(_trainer);
                        entry.State = EntityState.Modified;
                    }
                }

                _context.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ma'lumotlarni saqlashda xatolik yuz berdi: {ex.Message}", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
