using System;
using System.Drawing;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class EquipmentEditForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly Equipment _equipment;
        private TextBox txtName;
        private DateTimePicker dtpPurchaseDate;
        private TextBox txtPrice;
        private TextBox txtDescription;
        private ComboBox cmbStatus;

        public EquipmentEditForm(Equipment equipment = null)
        {
            _context = new ApplicationDbContext();
            _equipment = equipment ?? new Equipment();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 400);
            this.Text = _equipment.Id == 0 ? "Yangi jihoz" : "Jihozni tahrirlash";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Name
            var lblName = new Label();
            lblName.Text = "Nomi:";
            lblName.Location = new Point(20, 20);
            lblName.AutoSize = true;

            txtName = new TextBox();
            txtName.Location = new Point(120, 20);
            txtName.Size = new Size(240, 20);

            // Purchase Date
            var lblPurchaseDate = new Label();
            lblPurchaseDate.Text = "Sotib olingan sana:";
            lblPurchaseDate.Location = new Point(20, 60);
            lblPurchaseDate.AutoSize = true;

            dtpPurchaseDate = new DateTimePicker();
            dtpPurchaseDate.Location = new Point(120, 60);
            dtpPurchaseDate.Size = new Size(240, 20);
            dtpPurchaseDate.Format = DateTimePickerFormat.Short;

            // Price
            var lblPrice = new Label();
            lblPrice.Text = "Narxi:";
            lblPrice.Location = new Point(20, 100);
            lblPrice.AutoSize = true;

            txtPrice = new TextBox();
            txtPrice.Location = new Point(120, 100);
            txtPrice.Size = new Size(240, 20);

            // Description
            var lblDescription = new Label();
            lblDescription.Text = "Tavsif:";
            lblDescription.Location = new Point(20, 140);
            lblDescription.AutoSize = true;

            txtDescription = new TextBox();
            txtDescription.Location = new Point(120, 140);
            txtDescription.Size = new Size(240, 60);
            txtDescription.Multiline = true;

            // Status
            var lblStatus = new Label();
            lblStatus.Text = "Holati:";
            lblStatus.Location = new Point(20, 220);
            lblStatus.AutoSize = true;

            cmbStatus = new ComboBox();
            cmbStatus.Location = new Point(120, 220);
            cmbStatus.Size = new Size(240, 20);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Active", "Under Repair", "Retired" });

            // Save Button
            var btnSave = new Button();
            btnSave.Text = "Saqlash";
            btnSave.Location = new Point(120, 300);
            btnSave.Size = new Size(90, 30);
            btnSave.Click += BtnSave_Click;

            // Cancel Button
            var btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new Point(270, 300);
            btnCancel.Size = new Size(90, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] 
            { 
                lblName, txtName,
                lblPurchaseDate, dtpPurchaseDate,
                lblPrice, txtPrice,
                lblDescription, txtDescription,
                lblStatus, cmbStatus,
                btnSave, btnCancel
            });
        }

        private void LoadData()
        {
            if (_equipment.Id != 0)
            {
                txtName.Text = _equipment.Name;
                dtpPurchaseDate.Value = _equipment.PurchaseDate;
                txtPrice.Text = _equipment.Price.ToString("N0");
                txtDescription.Text = _equipment.Description;
                cmbStatus.Text = _equipment.Status;
            }
            else
            {
                dtpPurchaseDate.Value = DateTime.Today;
                cmbStatus.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Iltimos, jihoz nomini kiriting!", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Replace(",", ""), out decimal price))
            {
                MessageBox.Show("Iltimos, to'g'ri narx kiriting!", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _equipment.Name = txtName.Text;
                _equipment.PurchaseDate = dtpPurchaseDate.Value;
                _equipment.Price = price;
                _equipment.Description = txtDescription.Text;
                _equipment.Status = cmbStatus.Text;

                if (_equipment.Id == 0)
                {
                    _context.Equipment.Add(_equipment);
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
