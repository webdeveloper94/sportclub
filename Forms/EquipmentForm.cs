using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class EquipmentForm : Form
    {
        private readonly ApplicationDbContext _context;
        private DataGridView dgvEquipment;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private TextBox txtSearch;

        public EquipmentForm()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadEquipment();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1000, 600);
            this.Text = "Jihozlar";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Search Box
            var lblSearch = new Label();
            lblSearch.Text = "Qidirish:";
            lblSearch.Location = new Point(10, 10);
            lblSearch.AutoSize = true;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(70, 10);
            txtSearch.Size = new Size(200, 20);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // DataGridView
            dgvEquipment = new DataGridView();
            dgvEquipment.Location = new Point(10, 40);
            dgvEquipment.Size = new Size(960, 460);
            dgvEquipment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEquipment.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEquipment.MultiSelect = false;
            dgvEquipment.AllowUserToAddRows = false;
            dgvEquipment.ReadOnly = true;

            // Buttons
            var buttonPanel = new Panel();
            buttonPanel.Location = new Point(10, 510);
            buttonPanel.Size = new Size(960, 40);

            btnAdd = new Button();
            btnAdd.Text = "Yangi jihoz";
            btnAdd.Location = new Point(0, 0);
            btnAdd.Size = new Size(100, 30);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "O'zgartirish";
            btnEdit.Location = new Point(110, 0);
            btnEdit.Size = new Size(100, 30);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "O'chirish";
            btnDelete.Location = new Point(220, 0);
            btnDelete.Size = new Size(100, 30);
            btnDelete.Click += BtnDelete_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            this.Controls.AddRange(new Control[] { lblSearch, txtSearch, dgvEquipment, buttonPanel });
        }

        private void LoadEquipment(string searchText = "")
        {
            try
            {
                var query = _context.Equipment.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(e => 
                        e.Name.ToLower().Contains(searchText) || 
                        e.Description.ToLower().Contains(searchText));
                }

                var equipment = query
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        PurchaseDate = e.PurchaseDate.ToShortDateString(),
                        Price = $"{e.Price:N0} so'm",
                        e.Description,
                        e.Status
                    })
                    .OrderBy(e => e.Name)
                    .ToList();

                dgvEquipment.DataSource = equipment;

                if (dgvEquipment.Columns.Count > 0)
                {
                    dgvEquipment.Columns["Id"].HeaderText = "ID";
                    dgvEquipment.Columns["Name"].HeaderText = "Nomi";
                    dgvEquipment.Columns["PurchaseDate"].HeaderText = "Sotib olingan sana";
                    dgvEquipment.Columns["Price"].HeaderText = "Narxi";
                    dgvEquipment.Columns["Description"].HeaderText = "Tavsif";
                    dgvEquipment.Columns["Status"].HeaderText = "Holati";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ma'lumotlarni yuklashda xatolik yuz berdi: {ex.Message}", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEquipment(txtSearch.Text);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var equipmentEditForm = new EquipmentEditForm())
            {
                if (equipmentEditForm.ShowDialog() == DialogResult.OK)
                {
                    LoadEquipment(txtSearch.Text);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count > 0)
            {
                var equipmentId = (int)dgvEquipment.SelectedRows[0].Cells["Id"].Value;
                var equipment = _context.Equipment.Find(equipmentId);

                using (var equipmentEditForm = new EquipmentEditForm(equipment))
                {
                    if (equipmentEditForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadEquipment(txtSearch.Text);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'zgartirish uchun jihozni tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count > 0)
            {
                var equipmentId = (int)dgvEquipment.SelectedRows[0].Cells["Id"].Value;
                var equipment = _context.Equipment.Find(equipmentId);

                var result = MessageBox.Show(
                    $"Siz rostdan ham {equipment.Name} ni o'chirmoqchimisiz?",
                    "Tasdiqlash",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _context.Equipment.Remove(equipment);
                        _context.SaveChanges();
                        LoadEquipment(txtSearch.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Jihozni o'chirishda xatolik yuz berdi: {ex.Message}",
                            "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'chirish uchun jihozni tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
