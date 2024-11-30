using System;
using System.Drawing;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class PaymentForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly Member _member;
        private readonly decimal _amount;
        private TextBox txtAmount;
        private TextBox txtDescription;
        private ComboBox cmbPaymentMethod;

        public PaymentForm(Member member, decimal amount)
        {
            _context = new ApplicationDbContext();
            _member = member;
            _amount = amount;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 300);
            this.Text = "To'lov";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Amount Label
            var lblAmount = new Label();
            lblAmount.Text = "Summa:";
            lblAmount.Location = new Point(20, 20);
            lblAmount.AutoSize = true;

            // Amount TextBox
            txtAmount = new TextBox();
            txtAmount.Location = new Point(120, 20);
            txtAmount.Size = new Size(200, 20);
            txtAmount.Text = _amount.ToString("N0");
            txtAmount.ReadOnly = true;

            // Payment Method Label
            var lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "To'lov turi:";
            lblPaymentMethod.Location = new Point(20, 60);
            lblPaymentMethod.AutoSize = true;

            // Payment Method ComboBox
            cmbPaymentMethod = new ComboBox();
            cmbPaymentMethod.Location = new Point(120, 60);
            cmbPaymentMethod.Size = new Size(200, 20);
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Items.AddRange(new string[] { "Naqd", "Bank o'tkazmasi", "Plastik karta" });
            cmbPaymentMethod.SelectedIndex = 0;

            // Description Label
            var lblDescription = new Label();
            lblDescription.Text = "Izoh:";
            lblDescription.Location = new Point(20, 100);
            lblDescription.AutoSize = true;

            // Description TextBox
            txtDescription = new TextBox();
            txtDescription.Location = new Point(120, 100);
            txtDescription.Size = new Size(200, 60);
            txtDescription.Multiline = true;
            txtDescription.Text = $"{_member.FirstName} {_member.LastName} - Trener uchun to'lov";

            // Save Button
            var btnSave = new Button();
            btnSave.Text = "Saqlash";
            btnSave.Location = new Point(120, 180);
            btnSave.Size = new Size(90, 30);
            btnSave.Click += BtnSave_Click;

            // Cancel Button
            var btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new Point(230, 180);
            btnCancel.Size = new Size(90, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] 
            { 
                lblAmount, txtAmount,
                lblPaymentMethod, cmbPaymentMethod,
                lblDescription, txtDescription,
                btnSave, btnCancel
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var payment = new Payment
                {
                    MemberId = _member.Id,
                    Amount = _amount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = cmbPaymentMethod.Text,
                    Description = txtDescription.Text
                };

                _context.Payments.Add(payment);
                _context.SaveChanges();

                MessageBox.Show("To'lov muvaffaqiyatli saqlandi!", 
                    "Ma'lumot", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"To'lovni saqlashda xatolik yuz berdi: {ex.Message}", 
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
