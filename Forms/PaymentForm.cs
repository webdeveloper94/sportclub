using System;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class PaymentForm : Form
    {
        private readonly ActiveSession _session;
        private readonly ApplicationDbContext _context;
        private Label lblAmount;
        private Label lblPaymentMethod;
        private ComboBox cmbPaymentMethod;
        private Button btnPay;
        private Button btnCancel;
        private RadioButton rbDaily;
        private RadioButton rbHourly;
        private decimal _amount;
        private Price _currentPrice;

        public PaymentForm(ActiveSession session)
        {
            _session = session;
            _context = new ApplicationDbContext();
            _currentPrice = _context.Prices.OrderByDescending(p => p.LastUpdated).First();
            InitializeComponent();
            CalculateAmount();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 300);
            this.Text = "To'lov";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Payment Type Group
            var gbPaymentType = new GroupBox();
            gbPaymentType.Text = "To'lov turi";
            gbPaymentType.Location = new System.Drawing.Point(20, 20);
            gbPaymentType.Size = new System.Drawing.Size(340, 60);

            rbDaily = new RadioButton();
            rbDaily.Text = "Kunlik";
            rbDaily.Location = new System.Drawing.Point(20, 20);
            rbDaily.Checked = true;
            rbDaily.CheckedChanged += OnPaymentTypeChanged;

            rbHourly = new RadioButton();
            rbHourly.Text = "Soatlik";
            rbHourly.Location = new System.Drawing.Point(180, 20);
            rbHourly.CheckedChanged += OnPaymentTypeChanged;

            gbPaymentType.Controls.AddRange(new Control[] { rbDaily, rbHourly });

            // Amount Label
            var lblAmountTitle = new Label();
            lblAmountTitle.Text = "To'lov summasi:";
            lblAmountTitle.Location = new System.Drawing.Point(20, 100);
            lblAmountTitle.AutoSize = true;

            lblAmount = new Label();
            lblAmount.Location = new System.Drawing.Point(20, 120);
            lblAmount.AutoSize = true;
            lblAmount.Font = new System.Drawing.Font(lblAmount.Font, System.Drawing.FontStyle.Bold);

            // Payment Method
            lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "To'lov usuli:";
            lblPaymentMethod.Location = new System.Drawing.Point(20, 160);
            lblPaymentMethod.AutoSize = true;

            cmbPaymentMethod = new ComboBox();
            cmbPaymentMethod.Location = new System.Drawing.Point(20, 180);
            cmbPaymentMethod.Size = new System.Drawing.Size(340, 25);
            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Items.AddRange(new string[] { "Naqd", "Plastik karta", "Click", "Payme" });
            cmbPaymentMethod.SelectedIndex = 0;

            // Buttons
            btnPay = new Button();
            btnPay.Text = "To'lash";
            btnPay.Location = new System.Drawing.Point(190, 220);
            btnPay.Size = new System.Drawing.Size(80, 30);
            btnPay.Click += BtnPay_Click;

            btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new System.Drawing.Point(280, 220);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                gbPaymentType,
                lblAmountTitle, lblAmount,
                lblPaymentMethod, cmbPaymentMethod,
                btnPay, btnCancel
            });
        }

        private void OnPaymentTypeChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        private void CalculateAmount()
        {
            var duration = (_session.EndTime ?? DateTime.Now) - _session.StartTime;
            
            if (rbDaily.Checked)
            {
                _amount = _currentPrice.DailyPrice;
            }
            else
            {
                var hours = Math.Ceiling(duration.TotalHours);
                _amount = _currentPrice.HourlyPrice * (decimal)hours;
            }

            lblAmount.Text = $"{_amount:N0} so'm";
        }

        private void BtnPay_Click(object sender, EventArgs e)
        {
            var payment = new Payment
            {
                MemberId = _session.MemberId,
                Amount = _amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = cmbPaymentMethod.Text,
                Description = $"{(_session.EndTime.Value - _session.StartTime).TotalHours:F1} soatlik mashg'ulot uchun to'lov"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            PrintReceipt(payment);
            
            MessageBox.Show("To'lov muvaffaqiyatli amalga oshirildi!", 
                "Muvaffaqiyat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.Close();
        }

        private void PrintReceipt(Payment payment)
        {
            var printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) =>
            {
                var graphics = e.Graphics;
                var font = new System.Drawing.Font("Arial", 12);
                var brush = System.Drawing.Brushes.Black;
                var y = 10;

                // Header
                graphics.DrawString("SPORT CENTER", new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold), brush, 10, y);
                y += 30;

                // Payment details
                var member = _context.Members.Find(_session.MemberId);
                graphics.DrawString($"Sana: {payment.PaymentDate:dd.MM.yyyy HH:mm}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"A'zo: {member.FirstName} {member.LastName}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"Boshlangan vaqt: {_session.StartTime:dd.MM.yyyy HH:mm}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"Tugatilgan vaqt: {_session.EndTime:dd.MM.yyyy HH:mm}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"Davomiyligi: {(_session.EndTime.Value - _session.StartTime).TotalHours:F1} soat", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"To'lov turi: {(rbDaily.Checked ? "Kunlik" : "Soatlik")}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"To'lov usuli: {payment.PaymentMethod}", font, brush, 10, y);
                y += 20;
                graphics.DrawString($"Summa: {payment.Amount:N0} so'm", font, brush, 10, y);
                y += 40;

                // Footer
                graphics.DrawString("Xizmatlardan foydalanganingiz uchun rahmat!", font, brush, 10, y);
            };

            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chek chop etishda xatolik yuz berdi: {ex.Message}", 
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
