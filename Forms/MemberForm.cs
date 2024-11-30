using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SportCenter.Data;
using SportCenter.Models;

namespace SportCenter.Forms
{
    public partial class MemberForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly Member _member;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhoneNumber;
        private TextBox txtAddress;
        private DateTimePicker dtpDateOfBirth;
        private ComboBox cmbTrainer;
        private Button btnSave;
        private Button btnCancel;

        public MemberForm(Member member = null)
        {
            _context = new ApplicationDbContext();
            _member = member ?? new Member();
            InitializeComponent();
            LoadTrainers();
            LoadMemberData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 400);
            this.Text = _member.Id == 0 ? "Yangi a'zo" : "A'zo ma'lumotlarini o'zgartirish";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var y = 20;
            var gap = 50;

            // First Name
            var lblFirstName = new Label();
            lblFirstName.Text = "Ismi:";
            lblFirstName.Location = new Point(20, y);
            lblFirstName.AutoSize = true;

            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(20, y + 20);
            txtFirstName.Size = new Size(340, 20);

            y += gap;

            // Last Name
            var lblLastName = new Label();
            lblLastName.Text = "Familiyasi:";
            lblLastName.Location = new Point(20, y);
            lblLastName.AutoSize = true;

            txtLastName = new TextBox();
            txtLastName.Location = new Point(20, y + 20);
            txtLastName.Size = new Size(340, 20);

            y += gap;

            // Phone Number
            var lblPhoneNumber = new Label();
            lblPhoneNumber.Text = "Telefon:";
            lblPhoneNumber.Location = new Point(20, y);
            lblPhoneNumber.AutoSize = true;

            txtPhoneNumber = new TextBox();
            txtPhoneNumber.Location = new Point(20, y + 20);
            txtPhoneNumber.Size = new Size(340, 20);

            y += gap;

            // Date of Birth
            var lblDateOfBirth = new Label();
            lblDateOfBirth.Text = "Tug'ilgan sana:";
            lblDateOfBirth.Location = new Point(20, y);
            lblDateOfBirth.AutoSize = true;

            dtpDateOfBirth = new DateTimePicker();
            dtpDateOfBirth.Location = new Point(20, y + 20);
            dtpDateOfBirth.Size = new Size(340, 20);
            dtpDateOfBirth.Format = DateTimePickerFormat.Short;

            y += gap;

            // Address
            var lblAddress = new Label();
            lblAddress.Text = "Manzil:";
            lblAddress.Location = new Point(20, y);
            lblAddress.AutoSize = true;

            txtAddress = new TextBox();
            txtAddress.Location = new Point(20, y + 20);
            txtAddress.Size = new Size(340, 20);

            y += gap;

            // Trainer
            var lblTrainer = new Label();
            lblTrainer.Text = "Trener:";
            lblTrainer.Location = new Point(20, y);
            lblTrainer.AutoSize = true;

            cmbTrainer = new ComboBox();
            cmbTrainer.Location = new Point(20, y + 20);
            cmbTrainer.Size = new Size(340, 20);
            cmbTrainer.DropDownStyle = ComboBoxStyle.DropDownList;

            y += gap;

            // Buttons
            btnSave = new Button();
            btnSave.Text = "Saqlash";
            btnSave.Location = new Point(190, y);
            btnSave.Size = new Size(80, 30);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "Bekor qilish";
            btnCancel.Location = new Point(280, y);
            btnCancel.Size = new Size(80, 30);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] {
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblPhoneNumber, txtPhoneNumber,
                lblDateOfBirth, dtpDateOfBirth,
                lblAddress, txtAddress,
                lblTrainer, cmbTrainer,
                btnSave, btnCancel
            });
        }

        private void LoadTrainers()
        {
            var trainers = _context.Trainers
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.LastName)
                .Select(t => new
                {
                    Id = t.Id,
                    FullName = $"{t.FirstName} {t.LastName}"
                })
                .ToList();

            trainers.Insert(0, new { Id = 0, FullName = "Trener tanlanmagan" });

            cmbTrainer.DisplayMember = "FullName";
            cmbTrainer.ValueMember = "Id";
            cmbTrainer.DataSource = trainers;
        }

        private void LoadMemberData()
        {
            if (_member.Id != 0)
            {
                txtFirstName.Text = _member.FirstName;
                txtLastName.Text = _member.LastName;
                txtPhoneNumber.Text = _member.PhoneNumber;
                dtpDateOfBirth.Value = _member.DateOfBirth;
                txtAddress.Text = _member.Address;
                cmbTrainer.SelectedValue = _member.TrainerId ?? 0;
            }
            else
            {
                dtpDateOfBirth.Value = DateTime.Today.AddYears(-18);
                cmbTrainer.SelectedValue = 0;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || 
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Iltimos, barcha majburiy maydonlarni to'ldiring!", 
                    "Xato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _member.FirstName = txtFirstName.Text;
            _member.LastName = txtLastName.Text;
            _member.PhoneNumber = txtPhoneNumber.Text;
            _member.DateOfBirth = dtpDateOfBirth.Value;
            _member.Address = txtAddress.Text;
            _member.TrainerId = (int)cmbTrainer.SelectedValue == 0 ? null : (int)cmbTrainer.SelectedValue;

            if (_member.Id == 0)
            {
                _member.RegistrationDate = DateTime.Now;
                _member.IsActive = true;
                _context.Members.Add(_member);
            }
            else
            {
                _context.Members.Update(_member);
            }

            _context.SaveChanges();
            this.DialogResult = DialogResult.OK;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _context.Dispose();
        }
    }
}
