using System;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyDSsinhvien
{
    public partial class Form1 : Form
    {
        private QLSinhVienContextDB context;
        private Sinhvien currentSinhvien;
        private bool isAdding;

        public Form1()
        {
            InitializeComponent();
            context = new QLSinhVienContextDB();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadComboBoxLop();
            ResetForm();

            btLuu.Enabled = false;
            btKhong.Enabled = false;
        }


        private void LoadData()
        {
            var sinhvienList = context.Sinhviens
                .Select(sv => new
                {
                    sv.MaSV,
                    sv.HoTenSV,
                    sv.NgaySinh,
                    sv.MaLop,
                    TenLop = sv.Lop.TenLop
                })
                .ToList();

            dgvSinhvien.DataSource = sinhvienList;
        }

        private void LoadComboBoxLop()
        {
            var lopList = context.Lops.ToList();
            cboLop.DataSource = lopList;
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
        }

        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTenSV.Clear();
            dtNgaySinh.Value = DateTime.Now;
            cboLop.SelectedIndex = -1;
            currentSinhvien = null;
            isAdding = false;
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string search = txtTimKiem.Text.Trim();
            var result = context.Sinhviens
                .Where(sv => sv.MaSV.Contains(search) || sv.HoTenSV.Contains(search))
                .Select(sv => new
                {
                    sv.MaSV,
                    sv.HoTenSV,
                    sv.NgaySinh,
                    sv.MaLop,
                    TenLop = sv.Lop.TenLop
                })
                .ToList();

            dgvSinhvien.DataSource = result;
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            ResetForm();
            isAdding = true;

            // Kích hoạt nút Lưu và K.Lưu
            btLuu.Enabled = true;
            btKhong.Enabled = true;

            txtMaSV.Focus();
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (currentSinhvien != null) 
            {
                var confirmResult = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa sinh viên này?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes) 
                {
                    try
                    {
                        context.Sinhviens.Remove(currentSinhvien);
                        context.SaveChanges();
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); 
                        ResetForm(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btSua_Click(object sender, EventArgs e)
        {
            if (currentSinhvien != null)
            {
                txtMaSV.Text = currentSinhvien.MaSV;
                txtHoTenSV.Text = currentSinhvien.HoTenSV;
                dtNgaySinh.Value = currentSinhvien.NgaySinh;
                cboLop.SelectedValue = currentSinhvien.MaLop;

                // Kích hoạt nút Lưu và K.Lưu
                btLuu.Enabled = true;
                btKhong.Enabled = true;
                isAdding = false;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên để sửa.");
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            if (isAdding)
            {
                Sinhvien newSinhvien = new Sinhvien
                {
                    MaSV = txtMaSV.Text.Trim(),
                    HoTenSV = txtHoTenSV.Text.Trim(),
                    NgaySinh = dtNgaySinh.Value,
                    MaLop = cboLop.SelectedValue.ToString()
                };
                context.Sinhviens.Add(newSinhvien);
            }
            else if (currentSinhvien != null)
            {
                currentSinhvien.HoTenSV = txtHoTenSV.Text.Trim();
                currentSinhvien.NgaySinh = dtNgaySinh.Value;
                currentSinhvien.MaLop = cboLop.SelectedValue.ToString();
            }

            context.SaveChanges();
            MessageBox.Show("Lưu thành công!");

            btLuu.Enabled = false;
            btKhong.Enabled = false;

            LoadData();
            ResetForm();
        }

        private void btKhong_Click(object sender, EventArgs e)
        {
            btLuu.Enabled = false;
            btKhong.Enabled = false;
            ResetForm();

            MessageBox.Show("Hủy các thay đổi");
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvSinhvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string maSV = dgvSinhvien.Rows[e.RowIndex].Cells["MaSV"].Value.ToString();
                currentSinhvien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == maSV);

                if (currentSinhvien != null)
                {
                    txtMaSV.Text = currentSinhvien.MaSV;
                    txtHoTenSV.Text = currentSinhvien.HoTenSV;
                    dtNgaySinh.Value = currentSinhvien.NgaySinh;
                    cboLop.SelectedValue = currentSinhvien.MaLop;
                }
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
