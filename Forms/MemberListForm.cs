using System;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class MemberListForm : Form
    {
        private DataGridView dgvMembers;
        private Button btnRefresh;
        private Button btnEdit;
        private Button btnDelete;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnStartSession;
        private readonly ApplicationDbContext _context;

        public MemberListForm()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            this.Text = "A'zolar ro'yxati";
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(800, 600);

            // Search TextBox
            txtSearch = new TextBox();
            txtSearch.Location = new System.Drawing.Point(10, 10);
            txtSearch.Size = new System.Drawing.Size(200, 20);
            txtSearch.PlaceholderText = "Ism yoki telefon raqami...";

            // Search Button
            btnSearch = new Button();
            btnSearch.Text = "Qidirish";
            btnSearch.Location = new System.Drawing.Point(220, 10);
            btnSearch.Size = new System.Drawing.Size(75, 23);
            btnSearch.Click += new EventHandler(btnSearch_Click);

            // DataGridView
            dgvMembers = new DataGridView();
            dgvMembers.Location = new System.Drawing.Point(10, 40);
            dgvMembers.Size = new System.Drawing.Size(760, 470);
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.MultiSelect = false;
            dgvMembers.AllowUserToAddRows = false;

            // Buttons Panel
            var buttonsPanel = new Panel();
            buttonsPanel.Location = new System.Drawing.Point(10, 520);
            buttonsPanel.Size = new System.Drawing.Size(760, 35);

            btnRefresh = new Button();
            btnRefresh.Text = "Yangilash";
            btnRefresh.Location = new System.Drawing.Point(0, 0);
            btnRefresh.Size = new System.Drawing.Size(75, 30);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

            btnEdit = new Button();
            btnEdit.Text = "Tahrirlash";
            btnEdit.Location = new System.Drawing.Point(85, 0);
            btnEdit.Size = new System.Drawing.Size(75, 30);
            btnEdit.Click += new EventHandler(btnEdit_Click);

            btnDelete = new Button();
            btnDelete.Text = "O'chirish";
            btnDelete.Location = new System.Drawing.Point(170, 0);
            btnDelete.Size = new System.Drawing.Size(75, 30);
            btnDelete.Click += new EventHandler(btnDelete_Click);

            btnStartSession = new Button();
            btnStartSession.Text = "Mashg'ulotni boshlash";
            btnStartSession.Location = new System.Drawing.Point(255, 0);
            btnStartSession.Size = new System.Drawing.Size(150, 30);
            btnStartSession.Click += new EventHandler(btnStartSession_Click);

            buttonsPanel.Controls.AddRange(new Control[] { btnRefresh, btnEdit, btnDelete, btnStartSession });

            this.Controls.AddRange(new Control[] { 
                txtSearch, btnSearch, dgvMembers, buttonsPanel
            });
        }

        private void LoadMembers()
        {
            using (var context = new ApplicationDbContext())
            {
                var members = context.Members.Select(m => new {
                    m.Id,
                    m.FirstName,
                    m.LastName,
                    m.PhoneNumber,
                    m.RegistrationDate
                }).ToList();

                dgvMembers.DataSource = members;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                var searchText = txtSearch.Text.ToLower();
                var members = context.Members
                    .Where(m => m.FirstName.ToLower().Contains(searchText) ||
                               m.LastName.ToLower().Contains(searchText) ||
                               m.PhoneNumber.Contains(searchText))
                    .Select(m => new {
                        m.Id,
                        m.FirstName,
                        m.LastName,
                        m.PhoneNumber,
                        m.RegistrationDate
                    })
                    .ToList();

                dgvMembers.DataSource = members;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMembers();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                // TODO: Edit member implementation
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                if (MessageBox.Show("Haqiqatan ham bu a'zoni o'chirmoqchimisiz?", "Tasdiqlash",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var member = context.Members.Find(memberId);
                        if (member != null)
                        {
                            context.Members.Remove(member);
                            context.SaveChanges();
                            LoadMembers();
                        }
                    }
                }
            }
        }

        private void btnStartSession_Click(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count > 0)
            {
                var memberId = (int)dgvMembers.SelectedRows[0].Cells["Id"].Value;
                var member = _context.Members.Find(memberId);
                
                if (member != null)
                {
                    // Check if member already has active session
                    var existingSession = _context.ActiveSessions
                        .FirstOrDefault(s => s.MemberId == memberId && s.EndTime == null);

                    if (existingSession != null)
                    {
                        MessageBox.Show("Bu a'zoning mashg'uloti allaqachon boshlangan!", 
                            "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var session = new ActiveSession
                    {
                        MemberId = memberId,
                        StartTime = DateTime.Now,
                        EndTime = null
                    };

                    _context.ActiveSessions.Add(session);
                    _context.SaveChanges();

                    MessageBox.Show("Mashg'ulot muvaffaqiyatli boshlandi!", 
                        "Muvaffaqiyat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    // Notify parent form to refresh active sessions
                    if (Owner is Form1 mainForm)
                    {
                        mainForm.RefreshActiveSessions();
                    }
                }
            }
            else
            {
                MessageBox.Show("Iltimos, a'zoni tanlang!", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
