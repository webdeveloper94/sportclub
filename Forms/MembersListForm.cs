using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class MembersListForm : Form
    {
        private readonly ApplicationDbContext _context;
        private DataGridView dgvMembers;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnStartSession;
        private Button btnTrainerPayment;
        private TextBox txtSearch;

        public MembersListForm()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1000, 600);
            this.Text = "A'zolar ro'yxati";
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
            dgvMembers = new DataGridView();
            dgvMembers.Location = new Point(10, 40);
            dgvMembers.Size = new Size(960, 460);
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.ReadOnly = true;

            // Buttons
            var buttonPanel = new Panel();
            buttonPanel.Location = new Point(10, 510);
            buttonPanel.Size = new Size(960, 40);

            btnAdd = new Button();
            btnAdd.Text = "Yangi a'zo";
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

            btnStartSession = new Button();
            btnStartSession.Text = "Mashg'ulotni boshlash";
            btnStartSession.Location = new Point(330, 0);
            btnStartSession.Size = new Size(130, 30);
            btnStartSession.Click += BtnStartSession_Click;

            btnTrainerPayment = new Button();
            btnTrainerPayment.Text = "Trenerga to'lov";
            btnTrainerPayment.Location = new Point(470, 0);
            btnTrainerPayment.Size = new Size(120, 30);
            btnTrainerPayment.Click += BtnTrainerPayment_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnStartSession, btnTrainerPayment });

            this.Controls.AddRange(new Control[] { lblSearch, txtSearch, dgvMembers, buttonPanel });
        }

        private void LoadMembers(string searchText = "")
        {
            try
            {
                var query = _context.Members
                    .Include(m => m.Trainer)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(m => 
                        m.FirstName.ToLower().Contains(searchText) || 
                        m.LastName.ToLower().Contains(searchText) ||
                        m.PhoneNumber.Contains(searchText));
                }

                var members = query
                    .Select(m => new
                    {
                        m.Id,
                        m.FirstName,
                        m.LastName,
                        m.PhoneNumber,
                        DateOfBirth = m.DateOfBirth.ToShortDateString(),
                        m.Address,
                        TrainerName = m.Trainer != null ? $"{m.Trainer.FirstName} {m.Trainer.LastName}" : "Biriktirilmagan",
                        RegistrationDate = m.RegistrationDate.ToShortDateString(),
                        Status = m.IsActive ? "Faol" : "Faol emas"
                    })
                    .OrderBy(m => m.LastName)
                    .ThenBy(m => m.FirstName)
                    .ToList();

                dgvMembers.DataSource = members;

                if (dgvMembers.Columns.Count > 0)
                {
                    dgvMembers.Columns["Id"].HeaderText = "ID";
                    dgvMembers.Columns["FirstName"].HeaderText = "Ismi";
                    dgvMembers.Columns["LastName"].HeaderText = "Familiyasi";
                    dgvMembers.Columns["PhoneNumber"].HeaderText = "Telefon";
                    dgvMembers.Columns["DateOfBirth"].HeaderText = "Tug'ilgan sana";
                    dgvMembers.Columns["Address"].HeaderText = "Manzil";
                    dgvMembers.Columns["TrainerName"].HeaderText = "Treneri";
                    dgvMembers.Columns["RegistrationDate"].HeaderText = "Ro'yxatdan o'tgan sana";
                    dgvMembers.Columns["Status"].HeaderText = "Holati";
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
            LoadMembers(txtSearch.Text);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var memberForm = new MemberForm())
            {
                if (memberForm.ShowDialog() == DialogResult.OK)
                {
                    LoadMembers(txtSearch.Text);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                var member = _context.Members.Find(memberId);

                using (var memberForm = new MemberForm(member))
                {
                    if (memberForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadMembers(txtSearch.Text);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'zgartirish uchun a'zoni tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                var member = _context.Members.Find(memberId);

                var result = MessageBox.Show(
                    $"Siz rostdan ham {member.FirstName} {member.LastName} ni o'chirmoqchimisiz?",
                    "Tasdiqlash",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _context.Members.Remove(member);
                        _context.SaveChanges();
                        LoadMembers(txtSearch.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"A'zoni o'chirishda xatolik yuz berdi: {ex.Message}",
                            "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, o'chirish uchun a'zoni tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnStartSession_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                var member = _context.Members.Find(memberId);

                if (member != null)
                {
                    try
                    {
                        var session = new Session
                        {
                            MemberId = memberId,
                            StartTime = DateTime.Now,
                            Status = "Active"
                        };

                        _context.Sessions.Add(session);
                        _context.SaveChanges();

                        var activeSession = new ActiveSession
                        {
                            MemberId = memberId,
                            StartTime = DateTime.Now
                        };

                        _context.ActiveSessions.Add(activeSession);
                        _context.SaveChanges();

                        MessageBox.Show($"{member.FirstName} {member.LastName} uchun mashg'ulot boshlandi!", 
                            "Ma'lumot", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Asosiy oynani yangilash
                        if (Owner is Form1 mainForm)
                        {
                            mainForm.LoadActiveSessions();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Mashg'ulotni boshlashda xatolik yuz berdi: {ex.Message}", 
                            "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, mashg'ulot boshlash uchun a'zoni tanlang!", 
                    "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnTrainerPayment_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                var member = _context.Members
                    .Include(m => m.Trainer)
                    .FirstOrDefault(m => m.Id == memberId);

                if (member?.Trainer == null)
                {
                    MessageBox.Show("Bu a'zoga trener biriktirilmagan!", 
                        "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var paymentForm = new PaymentForm(member, member.Trainer.MonthlyFee))
                {
                    paymentForm.Owner = this;
                    if (paymentForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadMembers(txtSearch.Text);
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, to'lov qilish uchun a'zoni tanlang!", 
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
