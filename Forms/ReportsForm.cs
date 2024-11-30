using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly ApplicationDbContext _context;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnFilter;
        private DataGridView dgvPayments;
        private DataGridView dgvSessions;
        private DataGridView dgvNewMembers;
        private Label lblTotalPayments;
        private Label lblTotalSessions;
        private Label lblNewMembers;

        public ReportsForm()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadTodayData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1200, 800);
            this.Text = "Hisobotlar";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Date Filter Panel
            var filterPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1160, 40)
            };

            var lblStartDate = new Label
            {
                Text = "Boshlanish sanasi:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            dtpStartDate = new DateTimePicker
            {
                Location = new Point(120, 10),
                Size = new Size(200, 20),
                Format = DateTimePickerFormat.Short
            };

            var lblEndDate = new Label
            {
                Text = "Tugash sanasi:",
                Location = new Point(340, 10),
                AutoSize = true
            };

            dtpEndDate = new DateTimePicker
            {
                Location = new Point(440, 10),
                Size = new Size(200, 20),
                Format = DateTimePickerFormat.Short
            };

            btnFilter = new Button
            {
                Text = "Filtrlash",
                Location = new Point(660, 8),
                Size = new Size(100, 25)
            };
            btnFilter.Click += BtnFilter_Click;

            filterPanel.Controls.AddRange(new Control[] { lblStartDate, dtpStartDate, lblEndDate, dtpEndDate, btnFilter });

            // Payments Section
            var lblPayments = new Label
            {
                Text = "To'lovlar:",
                Location = new Point(10, 60),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblTotalPayments = new Label
            {
                Location = new Point(100, 60),
                AutoSize = true
            };

            dgvPayments = new DataGridView
            {
                Location = new Point(10, 90),
                Size = new Size(1160, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Sessions Section
            var lblSessions = new Label
            {
                Text = "Mashg'ulotlar:",
                Location = new Point(10, 300),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblTotalSessions = new Label
            {
                Location = new Point(120, 300),
                AutoSize = true
            };

            dgvSessions = new DataGridView
            {
                Location = new Point(10, 330),
                Size = new Size(1160, 200),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // New Members Section
            var lblMembers = new Label
            {
                Text = "Yangi a'zolar:",
                Location = new Point(10, 540),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            lblNewMembers = new Label
            {
                Location = new Point(120, 540),
                AutoSize = true
            };

            dgvNewMembers = new DataGridView
            {
                Location = new Point(10, 570),
                Size = new Size(1160, 180),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            this.Controls.AddRange(new Control[] 
            { 
                filterPanel,
                lblPayments, lblTotalPayments, dgvPayments,
                lblSessions, lblTotalSessions, dgvSessions,
                lblMembers, lblNewMembers, dgvNewMembers
            });
        }

        private void LoadData(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Load Payments
                var payments = _context.Payments
                    .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .Select(p => new
                    {
                        Date = p.PaymentDate,
                        Member = p.Member.FirstName + " " + p.Member.LastName,
                        p.Amount,
                        PaymentType = p.PaymentMethod,
                        p.Description
                    })
                    .OrderByDescending(p => p.Date)
                    .ToList();

                dgvPayments.DataSource = payments;
                lblTotalPayments.Text = $"Jami: {payments.Sum(p => p.Amount):N0} so'm";

                // Load Sessions
                var sessions = _context.Sessions
                    .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                    .Select(s => new
                    {
                        s.StartTime,
                        s.EndTime,
                        Member = s.Member.FirstName + " " + s.Member.LastName,
                        Duration = s.EndTime.HasValue ? 
                            (s.EndTime.Value - s.StartTime).TotalHours.ToString("F1") + " soat" : 
                            "Davom etmoqda",
                        s.Status,
                        s.Notes
                    })
                    .OrderByDescending(s => s.StartTime)
                    .ToList();

                dgvSessions.DataSource = sessions;
                lblTotalSessions.Text = $"Jami: {sessions.Count} ta mashg'ulot";

                // Load New Members
                var newMembers = _context.Members
                    .Where(m => m.RegistrationDate >= startDate && m.RegistrationDate <= endDate)
                    .Select(m => new
                    {
                        JoinDate = m.RegistrationDate,
                        m.FirstName,
                        m.LastName,
                        m.PhoneNumber,
                        Trainer = m.Trainer != null ? m.Trainer.FirstName + " " + m.Trainer.LastName : "-"
                    })
                    .OrderByDescending(m => m.JoinDate)
                    .ToList();

                dgvNewMembers.DataSource = newMembers;
                lblNewMembers.Text = $"Jami: {newMembers.Count} ta yangi a'zo";

                // Set Column Headers
                if (dgvPayments.Columns.Count > 0)
                {
                    dgvPayments.Columns["Date"].HeaderText = "Sana";
                    dgvPayments.Columns["Member"].HeaderText = "A'zo";
                    dgvPayments.Columns["Amount"].HeaderText = "Summa";
                    dgvPayments.Columns["PaymentType"].HeaderText = "To'lov turi";
                    dgvPayments.Columns["Description"].HeaderText = "Izoh";
                }

                if (dgvSessions.Columns.Count > 0)
                {
                    dgvSessions.Columns["StartTime"].HeaderText = "Boshlangan vaqt";
                    dgvSessions.Columns["EndTime"].HeaderText = "Tugatilgan vaqt";
                    dgvSessions.Columns["Member"].HeaderText = "A'zo";
                    dgvSessions.Columns["Duration"].HeaderText = "Davomiyligi";
                    dgvSessions.Columns["Status"].HeaderText = "Holati";
                    dgvSessions.Columns["Notes"].HeaderText = "Izohlar";
                }

                if (dgvNewMembers.Columns.Count > 0)
                {
                    dgvNewMembers.Columns["JoinDate"].HeaderText = "A'zo bo'lgan sana";
                    dgvNewMembers.Columns["FirstName"].HeaderText = "Ismi";
                    dgvNewMembers.Columns["LastName"].HeaderText = "Familiyasi";
                    dgvNewMembers.Columns["PhoneNumber"].HeaderText = "Telefon";
                    dgvNewMembers.Columns["Trainer"].HeaderText = "Murabbiy";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ma'lumotlarni yuklashda xatolik yuz berdi: {ex.Message}",
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTodayData()
        {
            var today = DateTime.Today;
            dtpStartDate.Value = today;
            dtpEndDate.Value = today;
            LoadData(today, today.AddDays(1).AddSeconds(-1));
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (dtpEndDate.Value < dtpStartDate.Value)
            {
                MessageBox.Show("Tugash sanasi boshlanish sanasidan oldin bo'lishi mumkin emas!",
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadData(dtpStartDate.Value.Date, dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
