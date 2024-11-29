using System;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class StartSessionForm : Form
    {
        private ComboBox cmbMember;
        private ComboBox cmbSubscriptionType;
        private Button btnStart;
        private Button btnCancel;
        private Button btnNewMember;
        private ApplicationDbContext _context;
        private int? _selectedMemberId;

        public StartSessionForm(int? memberId = null)
        {
            _context = new ApplicationDbContext();
            _selectedMemberId = memberId;
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(400, 250);
            this.Text = "Mashg'ulotni boshlash";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Member ComboBox
            var lblMember = new Label();
            lblMember.Text = "A'zoni tanlang:";
            lblMember.Location = new System.Drawing.Point(20, 20);
            lblMember.AutoSize = true;

            cmbMember = new ComboBox();
            cmbMember.Location = new System.Drawing.Point(20, 40);
            cmbMember.Size = new System.Drawing.Size(250, 25);
            cmbMember.DropDownStyle = ComboBoxStyle.DropDownList;

            // New Member Button
            btnNewMember = new Button();
            btnNewMember.Text = "Yangi a'zo";
            btnNewMember.Location = new System.Drawing.Point(280, 40);
            btnNewMember.Size = new System.Drawing.Size(80, 25);
            btnNewMember.Click += OnNewMemberClick;

            // Subscription Type ComboBox
            var lblType = new Label();
            lblType.Text = "To'lov turi:";
            lblType.Location = new System.Drawing.Point(20, 80);
            lblType.AutoSize = true;

            cmbSubscriptionType = new ComboBox();
            cmbSubscriptionType.Location = new System.Drawing.Point(20, 100);
            cmbSubscriptionType.Size = new System.Drawing.Size(250, 25);
            cmbSubscriptionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSubscriptionType.Items.AddRange(new object[] { 
                "Kunlik", "Soatlik" 
            });
            cmbSubscriptionType.SelectedIndex = 0;

            // Buttons
            btnStart = new Button();
            btnStart.Text = "Boshlash";
            btnStart.Location = new System.Drawing.Point(20, 160);
            btnStart.Size = new System.Drawing.Size(100, 30);
            btnStart.Click += OnStartClick;

            btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new System.Drawing.Point(130, 160);
            btnCancel.Size = new System.Drawing.Size(100, 30);
            btnCancel.Click += OnCancelClick;

            // Add controls
            this.Controls.AddRange(new Control[] { 
                lblMember, cmbMember, btnNewMember,
                lblType, cmbSubscriptionType,
                btnStart, btnCancel 
            });
        }

        private void LoadMembers()
        {
            var members = _context.Members
                .Where(m => m.IsActive)
                .Select(m => new
                {
                    m.Id,
                    FullName = m.FirstName + " " + m.LastName
                })
                .OrderBy(m => m.FullName)
                .ToList();

            cmbMember.DataSource = members;
            cmbMember.DisplayMember = "FullName";
            cmbMember.ValueMember = "Id";

            if (_selectedMemberId.HasValue)
            {
                var selectedMember = members.FirstOrDefault(m => m.Id == _selectedMemberId.Value);
                if (selectedMember != null)
                {
                    cmbMember.SelectedValue = selectedMember.Id;
                }
            }
        }

        private void OnNewMemberClick(object sender, EventArgs e)
        {
            var newMemberForm = new NewMemberForm();
            if (newMemberForm.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void OnStartClick(object sender, EventArgs e)
        {
            if (cmbMember.SelectedValue == null)
            {
                MessageBox.Show("Iltimos, a'zoni tanlang", "Xato", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var memberId = (int)cmbMember.SelectedValue;
            var sessionType = cmbSubscriptionType.SelectedItem.ToString();

            var activeSession = new ActiveSession
            {
                MemberId = memberId,
                StartTime = DateTime.Now,
                SessionType = sessionType
            };

            try
            {
                _context.ActiveSessions.Add(activeSession);
                _context.SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik yuz berdi: {ex.Message}", "Xato", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
