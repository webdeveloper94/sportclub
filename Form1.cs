using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SportCenter.Data;
using SportCenter.Forms;
using SportCenter.Models;

namespace SportCenter
{
    public partial class Form1 : Form
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem membersMenu, employeesMenu, reportsMenu, settingsMenu;
        private ToolStripMenuItem equipmentMenu;
        private DataGridView dgvActiveSessions;
        private Button btnEndSession;
        private Label lblActiveSessions;
        private readonly ApplicationDbContext _context;
        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            InitializeTimer();
            LoadActiveSessions();
            this.FormClosing += OnFormClosing;
            
        }

        private void InitializeTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // Her sekundda yangilanadi
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dgvActiveSessions?.Rows?.Count > 0)
            {
                foreach (DataGridViewRow row in dgvActiveSessions.Rows)
                {
                    if (row?.Cells["StartTime"]?.Value != null)
                    {
                        DateTime startTime = (DateTime)row.Cells["StartTime"].Value;
                        row.Cells["Duration"].Value = EvaluateElapsedTime(startTime);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            // Form settings
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Sport Center";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Menu Strip
            menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;

            // Members Menu
            membersMenu = new ToolStripMenuItem("A'zolar");
            var newMemberMenu = new ToolStripMenuItem("Yangi a'zo", null, (s, e) => 
            {
                using (var memberForm = new MemberForm())
                {
                    if (memberForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadActiveSessions();
                    }
                }
            });

            var memberListMenu = new ToolStripMenuItem("A'zolar ro'yxati", null, (s, e) => 
            {
                using (var membersListForm = new MembersListForm())
                {
                    membersListForm.Owner = this;
                    membersListForm.ShowDialog();
                }
            });

            membersMenu.DropDownItems.AddRange(new ToolStripItem[] { newMemberMenu, memberListMenu });

            // Settings Menu
            settingsMenu = new ToolStripMenuItem("Sozlash");
            var priceSettingsMenu = new ToolStripMenuItem("Narxlar", null, OnPriceSettingsClick);
            settingsMenu.DropDownItems.Add(priceSettingsMenu);

            // Equipment Menu
            equipmentMenu = new ToolStripMenuItem("Jihozlar");
            equipmentMenu.Click += (s, e) =>
            {
                using (var equipmentForm = new EquipmentForm())
                {
                    equipmentForm.ShowDialog();
                }
            };

            // Other Menus
            employeesMenu = new ToolStripMenuItem("Trenerlar");
            employeesMenu.Click += (s, e) => 
            {
                using (var trainerForm = new TrainerForm())
                {
                    trainerForm.ShowDialog();
                }
            };
            reportsMenu = new ToolStripMenuItem("Hisobotlar");
            reportsMenu.Click += (s, e) =>
            {
                using (var reportsForm = new ReportsForm())
                {
                    reportsForm.ShowDialog();
                }
            };

            menuStrip.Items.AddRange(new ToolStripItem[] { 
                membersMenu, employeesMenu, reportsMenu, settingsMenu, equipmentMenu 
            });

            // Active Sessions Label
            lblActiveSessions = new Label();
            lblActiveSessions.Text = "Faol mashg'ulotlar:";
            lblActiveSessions.Location = new System.Drawing.Point(10, 40);
            lblActiveSessions.AutoSize = true;
            lblActiveSessions.Font = new System.Drawing.Font(lblActiveSessions.Font, System.Drawing.FontStyle.Bold);

            // Active Sessions DataGridView
            dgvActiveSessions = new DataGridView();
            dgvActiveSessions.Location = new System.Drawing.Point(10, 70);
            dgvActiveSessions.Size = new System.Drawing.Size(1160, 500);
            dgvActiveSessions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvActiveSessions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvActiveSessions.MultiSelect = false;
            dgvActiveSessions.AllowUserToAddRows = false;
            dgvActiveSessions.ReadOnly = true;
            dgvActiveSessions.BackgroundColor = System.Drawing.Color.White;

            // End Session Button
            btnEndSession = new Button();
            btnEndSession.Text = "Mashg'ulotni yakunlash";
            btnEndSession.Location = new System.Drawing.Point(10, 580);
            btnEndSession.Size = new System.Drawing.Size(150, 35);
            btnEndSession.Click += BtnEndSession_Click;
            btnEndSession.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
            btnEndSession.ForeColor = System.Drawing.Color.White;
            btnEndSession.FlatStyle = FlatStyle.Flat;
            btnEndSession.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // Add controls to form
            this.Controls.Clear();
            this.Controls.Add(menuStrip);
            this.Controls.Add(lblActiveSessions);
            this.Controls.Add(dgvActiveSessions);
            this.Controls.Add(btnEndSession);
        }

        private void OnPriceSettingsClick(object sender, EventArgs e)
        {
            var priceForm = new PriceManagementForm();
            priceForm.ShowDialog();
        }

        private void OnNewMemberClick(object sender, EventArgs e)
        {
            var newMemberForm = new NewMemberForm();
            newMemberForm.ShowDialog();
        }

        private void OnMemberListClick(object sender, EventArgs e)
        {
            var memberListForm = new MemberListForm();
            memberListForm.Owner = this;
            memberListForm.Show();
        }

        public void RefreshActiveSessions()
        {
            LoadActiveSessions();
        }

        public void LoadActiveSessions()
        {
            try 
            {
                var activeSessions = _context.ActiveSessions
                    .Include(s => s.Member)
                    .Where(s => s.EndTime == null)
                    .Select(s => new {
                        s.Id,
                        MemberName = s.Member.FirstName + " " + s.Member.LastName,
                        StartTime = s.StartTime,
                        Duration = EvaluateElapsedTime(s.StartTime)
                    })
                    .ToList();

                dgvActiveSessions.DataSource = null;
                dgvActiveSessions.DataSource = activeSessions;

                if (dgvActiveSessions.Columns.Count > 0)
                {
                    dgvActiveSessions.Columns["Id"].HeaderText = "ID";
                    dgvActiveSessions.Columns["MemberName"].HeaderText = "A'zo";
                    dgvActiveSessions.Columns["StartTime"].HeaderText = "Boshlangan vaqt";
                    dgvActiveSessions.Columns["Duration"].HeaderText = "Davomiyligi";

                    dgvActiveSessions.Columns["Id"].Width = 50;
                    dgvActiveSessions.Columns["MemberName"].Width = 200;
                    dgvActiveSessions.Columns["StartTime"].Width = 150;
                    dgvActiveSessions.Columns["Duration"].Width = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ma'lumotlarni yuklashda xatolik: {ex.Message}", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EvaluateElapsedTime(DateTime startTime)
        {
            var elapsed = DateTime.Now - startTime;
            return $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }

        private void BtnEndSession_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvActiveSessions.SelectedRows.Count > 0)
                {
                    var sessionId = (int)dgvActiveSessions.SelectedRows[0].Cells["Id"].Value;
                    var session = _context.ActiveSessions
                        .Include(s => s.Member)
                        .FirstOrDefault(s => s.Id == sessionId);

                    if (session != null)
                    {
                        var endTime = DateTime.Now;
                        var duration = endTime.Subtract(session.StartTime);
                        var price = _context.Prices.OrderByDescending(p => p.LastUpdated).FirstOrDefault();
                        
                        if (price == null)
                        {
                            MessageBox.Show("Narxlar topilmadi. Iltimos, avval narxlarni sozlang!", 
                                "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Daqiqalar bo'yicha narx hisoblash
                        decimal minutePrice = price.HourlyPrice / 60; // Bir daqiqa narxi
                        int totalMinutes = (int)Math.Ceiling(duration.TotalMinutes); // Umumiy daqiqalar
                        decimal amount = minutePrice * totalMinutes;

                        using (var paymentForm = new PaymentForm(session.Member, amount))
                        {
                            if (paymentForm.ShowDialog() == DialogResult.OK)
                            {
                                session.EndTime = endTime;
                                _context.SaveChanges();
                                LoadActiveSessions();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Iltimos, yakunlamoqchi bo'lgan mashg'ulotni tanlang!", 
                        "Ogohlantirish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mashg'ulotni yakunlashda xatolik yuz berdi: {ex.Message}", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            _context.Dispose();
        }
    }
}
