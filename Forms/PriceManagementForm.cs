using System;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class PriceManagementForm : Form
    {
        private readonly ApplicationDbContext _context;

        public PriceManagementForm()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadCurrentPrices();
        }

        private void InitializeComponent()
        {
            this.Text = "Narxlarni boshqarish";
            this.Size = new System.Drawing.Size(400, 300);

            txtDailyPrice = new TextBox();
            txtHourlyPrice = new TextBox();
            btnSave = new Button();
            lblDailyPrice = new Label();
            lblHourlyPrice = new Label();

            // Daily Price
            lblDailyPrice.Text = "Kunlik narx:";
            lblDailyPrice.Location = new System.Drawing.Point(20, 20);
            lblDailyPrice.Size = new System.Drawing.Size(100, 20);

            txtDailyPrice.Location = new System.Drawing.Point(130, 20);
            txtDailyPrice.Size = new System.Drawing.Size(200, 20);

            // Hourly Price
            lblHourlyPrice.Text = "Soatlik narx:";
            lblHourlyPrice.Location = new System.Drawing.Point(20, 60);
            lblHourlyPrice.Size = new System.Drawing.Size(100, 20);

            txtHourlyPrice.Location = new System.Drawing.Point(130, 60);
            txtHourlyPrice.Size = new System.Drawing.Size(200, 20);

            // Save Button
            btnSave.Text = "Saqlash";
            btnSave.Location = new System.Drawing.Point(130, 100);
            btnSave.Size = new System.Drawing.Size(100, 30);
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { 
                lblDailyPrice, txtDailyPrice,
                lblHourlyPrice, txtHourlyPrice,
                btnSave
            });
        }

        private TextBox txtDailyPrice;
        private TextBox txtHourlyPrice;
        private Button btnSave;
        private Label lblDailyPrice;
        private Label lblHourlyPrice;

        private void LoadCurrentPrices()
        {
            var currentPrice = _context.Prices.OrderByDescending(p => p.LastUpdated).FirstOrDefault();
            if (currentPrice != null)
            {
                txtDailyPrice.Text = currentPrice.DailyPrice.ToString();
                txtHourlyPrice.Text = currentPrice.HourlyPrice.ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtDailyPrice.Text, out decimal dailyPrice) ||
                !decimal.TryParse(txtHourlyPrice.Text, out decimal hourlyPrice))
            {
                MessageBox.Show("Iltimos, to'g'ri narxlarni kiriting!", "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newPrice = new Price
            {
                DailyPrice = dailyPrice,
                HourlyPrice = hourlyPrice,
                LastUpdated = DateTime.Now
            };

            _context.Prices.Add(newPrice);
            _context.SaveChanges();

            MessageBox.Show("Narxlar muvaffaqiyatli saqlandi!", "Muvaffaqiyat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
