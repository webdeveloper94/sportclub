using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace SportCenter.Forms
{
    public partial class TrainerForm : Form
    {
        private readonly ApplicationDbContext _context;
        private DataGridView dgvTrainers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        public TrainerForm()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadTrainers();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = "Trenerlar";
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            dgvTrainers = new DataGridView();
            dgvTrainers.Location = new Point(10, 10);
            dgvTrainers.Size = new Size(760, 480);
            dgvTrainers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTrainers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTrainers.MultiSelect = false;
            dgvTrainers.AllowUserToAddRows = false;
            dgvTrainers.ReadOnly = true;

            // Buttons
            var buttonPanel = new Panel();
            buttonPanel.Location = new Point(10, 500);
            buttonPanel.Size = new Size(760, 40);

            btnAdd = new Button();
            btnAdd.Text = "Qo'shish";
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

            this.Controls.AddRange(new Control[] { dgvTrainers, buttonPanel });
        }

        private void LoadTrainers()
        {
            try
            {
                var trainers = _context.Trainers
                    .Include(t => t.Members)
                    .Select(t => new
                    {
                        t.Id,
                        t.FirstName,
                        t.LastName,
                        t.Age,
                        MonthlyFee = $"{t.MonthlyFee:N0} so'm",
                        MembersCount = t.Members.Count
                    })
                    .ToList();

                dgvTrainers.DataSource = trainers;

                if (dgvTrainers.Columns.Count > 0)
                {
                    dgvTrainers.Columns["Id"].HeaderText = "ID";
                    dgvTrainers.Columns["FirstName"].HeaderText = "Ismi";
                    dgvTrainers.Columns["LastName"].HeaderText = "Familiyasi";
                    dgvTrainers.Columns["Age"].HeaderText = "Yoshi";
                    dgvTrainers.Columns["MonthlyFee"].HeaderText = "Oylik to'lov";
                    dgvTrainers.Columns["MembersCount"].HeaderText = "A'zolar soni";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ma'lumotlarni yuklashda xatolik yuz berdi: {ex.Message}", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var addForm = new TrainerEditForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadTrainers();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTrainers.SelectedRows.Count > 0)
            {
                var trainerId = (int)dgvTrainers.SelectedRows[0].Cells["Id"].Value;
                var trainer = _context.Trainers.Find(trainerId);
                
                var editForm = new TrainerEditForm(trainer);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadTrainers();
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'zgartirish uchun trener tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTrainers.SelectedRows.Count > 0)
            {
                var trainerId = (int)dgvTrainers.SelectedRows[0].Cells["Id"].Value;
                var trainer = _context.Trainers.Find(trainerId);

                var result = MessageBox.Show(
                    $"Siz rostdan ham {trainer.FirstName} {trainer.LastName} ni o'chirmoqchimisiz?",
                    "Tasdiqlash",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _context.Trainers.Remove(trainer);
                    _context.SaveChanges();
                    LoadTrainers();
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'chirish uchun trener tanlang!", 
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
