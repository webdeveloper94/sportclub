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
        private ToolStripMenuItem membersMenu, subscriptionsMenu, paymentsMenu, employeesMenu, reportsMenu, settingsMenu;
        private DataGridView dgvActiveSessions;
        private Button btnEndSession;
        private Label lblActiveSessions;
        private readonly ApplicationDbContext _context;

        public Form1()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadActiveSessions();
            this.FormClosing += OnFormClosing;
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(1200, 800);
            this.Text = "Sport Center";

            // Menu Strip
            menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;

            // Members Menu
            membersMenu = new ToolStripMenuItem("Members");
            var newMemberMenu = new ToolStripMenuItem("New Member", null, OnNewMemberClick);
            var memberListMenu = new ToolStripMenuItem("Member List", null, OnMemberListClick);
            membersMenu.DropDownItems.AddRange(new ToolStripItem[] { newMemberMenu, memberListMenu });

            // Settings Menu
            settingsMenu = new ToolStripMenuItem("Settings");
            var priceSettingsMenu = new ToolStripMenuItem("Price Management", null, OnPriceSettingsClick);
            settingsMenu.DropDownItems.Add(priceSettingsMenu);

            // Other Menus
            subscriptionsMenu = new ToolStripMenuItem("Subscriptions");
            paymentsMenu = new ToolStripMenuItem("Payments");
            employeesMenu = new ToolStripMenuItem("Employees");
            reportsMenu = new ToolStripMenuItem("Reports");

            menuStrip.Items.AddRange(new ToolStripItem[] { 
                membersMenu, subscriptionsMenu, paymentsMenu, employeesMenu, reportsMenu, settingsMenu 
            });

            // Active Sessions Label
            lblActiveSessions = new Label();
            lblActiveSessions.Text = "Active Sessions:";
            lblActiveSessions.Location = new System.Drawing.Point(10, 40);
            lblActiveSessions.AutoSize = true;
            lblActiveSessions.Font = new System.Drawing.Font(lblActiveSessions.Font, System.Drawing.FontStyle.Bold);

            // Active Sessions DataGridView
            dgvActiveSessions = new DataGridView();
            dgvActiveSessions.Location = new System.Drawing.Point(10, 70);
            dgvActiveSessions.Size = new System.Drawing.Size(1160, 600);
            dgvActiveSessions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvActiveSessions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvActiveSessions.MultiSelect = false;
            dgvActiveSessions.AllowUserToAddRows = false;
            dgvActiveSessions.ReadOnly = true;

            // End Session Button
            btnEndSession = new Button();
            btnEndSession.Text = "End Session";
            btnEndSession.Location = new System.Drawing.Point(10, 680);
            btnEndSession.Size = new System.Drawing.Size(150, 30);
            btnEndSession.Click += BtnEndSession_Click;

            this.Controls.AddRange(new Control[] {
                menuStrip,
                lblActiveSessions,
                dgvActiveSessions,
                btnEndSession
            });
        }

        private void OnPriceSettingsClick(object sender, EventArgs e)
        {
            var priceForm = new PriceManagementForm();
            priceForm.ShowDialog();
        }

        private void OnNewMemberClick(object sender, EventArgs e)
        {
            // TODO: Implement new member form
            MessageBox.Show("New member form is not implemented yet");
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

        private void LoadActiveSessions()
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

            dgvActiveSessions.DataSource = activeSessions;
        }

        private static string EvaluateElapsedTime(DateTime startTime)
        {
            var elapsed = DateTime.Now - startTime;
            return $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }

        private void BtnEndSession_Click(object sender, EventArgs e)
        {
            if (dgvActiveSessions.SelectedRows.Count > 0)
            {
                var sessionId = (int)dgvActiveSessions.SelectedRows[0].Cells["Id"].Value;
                var session = _context.ActiveSessions
                    .Include(s => s.Member)
                    .FirstOrDefault(s => s.Id == sessionId);

                if (session != null)
                {
                    session.EndTime = DateTime.Now;
                    _context.SaveChanges();

                    var paymentForm = new PaymentForm(session);
                    paymentForm.ShowDialog();

                    LoadActiveSessions();
                }
            }
            else
            {
                MessageBox.Show("Please select a session to end!", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _context.Dispose();
        }
    }
}
