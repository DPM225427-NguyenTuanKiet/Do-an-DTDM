using CarShop.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CarShop.Services
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Constructor nhận IMongoDatabase (đã đăng ký trong Program.cs)
        public MongoDbContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<ChucVu> ChucVu => _database.GetCollection<ChucVu>("chucvu");
        public IMongoCollection<DanhMuc> DanhMuc => _database.GetCollection<DanhMuc>("danhmuc");
        public IMongoCollection<ThuongHieu> ThuongHieu => _database.GetCollection<ThuongHieu>("thuonghieu");
        public IMongoCollection<LoaiGia> LoaiGia => _database.GetCollection<LoaiGia>("loaigia");
        public IMongoCollection<Kho> Kho => _database.GetCollection<Kho>("kho");
        public IMongoCollection<NhaCungCap> NhaCungCap => _database.GetCollection<NhaCungCap>("nhacungcap");
        public IMongoCollection<ThuocTinh> ThuocTinh => _database.GetCollection<ThuocTinh>("thuoctinh");
        public IMongoCollection<QuaTang> QuaTang => _database.GetCollection<QuaTang>("quatang");
        public IMongoCollection<Voucher> Voucher => _database.GetCollection<Voucher>("voucher");
        public IMongoCollection<TaiKhoan> TaiKhoan => _database.GetCollection<TaiKhoan>("taikhoan");
        public IMongoCollection<KhachHang> KhachHang => _database.GetCollection<KhachHang>("khachhang");
        public IMongoCollection<NhanVien> NhanVien => _database.GetCollection<NhanVien>("nhanvien");
        public IMongoCollection<SanPham> SanPham => _database.GetCollection<SanPham>("sanpham");
        public IMongoCollection<DonHang> DonHang => _database.GetCollection<DonHang>("donhang");
        public IMongoCollection<GioHang> GioHang => _database.GetCollection<GioHang>("giohang");
        public IMongoCollection<PhieuNhap> PhieuNhap => _database.GetCollection<PhieuNhap>("phieunhap");
        public IMongoCollection<PhieuXuat> PhieuXuat => _database.GetCollection<PhieuXuat>("phieuxuat");
        public IMongoCollection<KhuyenMai> KhuyenMai => _database.GetCollection<KhuyenMai>("khuyenmai");
        public IMongoCollection<GiaKhuyenMai> GiaKhuyenMai => _database.GetCollection<GiaKhuyenMai>("giakhuyenmai");
        public IMongoCollection<SanPhamKhuyenMai> SanPhamKhuyenMai => _database.GetCollection<SanPhamKhuyenMai>("sanpham_khuyenmai");
        public IMongoCollection<HoaDon> HoaDon => _database.GetCollection<HoaDon>("hoadon");
        public IMongoCollection<ThanhToan> ThanhToan => _database.GetCollection<ThanhToan>("thanhtoan");
        public IMongoCollection<VanChuyen> VanChuyen => _database.GetCollection<VanChuyen>("vanchuyen");
        public IMongoCollection<BaoHanh> BaoHanh => _database.GetCollection<BaoHanh>("baohanh");
        public IMongoCollection<BinhLuan> BinhLuan => _database.GetCollection<BinhLuan>("binhluan");
        public IMongoCollection<HinhAnh> HinhAnh => _database.GetCollection<HinhAnh>("hinhanh");
        public IMongoCollection<DiemTichLuy> DiemTichLuy => _database.GetCollection<DiemTichLuy>("diemtichluy");
        public IMongoCollection<LichSuDiem> LichSuDiem => _database.GetCollection<LichSuDiem>("lichsudiem");
        public IMongoCollection<LichSuDoiDiem> LichSuDoiDiem => _database.GetCollection<LichSuDoiDiem>("lichsudoiiem");
        public IMongoCollection<DiaChiGiao> DiaChiGiao => _database.GetCollection<DiaChiGiao>("diachigiao");
        public IMongoCollection<YeuThich> YeuThich => _database.GetCollection<YeuThich>("yeuthich");
        public IMongoCollection<ActivityLog> ActivityLog => _database.GetCollection<ActivityLog>("activity_log");
        public IMongoCollection<LichSuDangNhap> LichSuDangNhap => _database.GetCollection<LichSuDangNhap>("lichsudangnhap");
        public IMongoCollection<LichSuDonHang> LichSuDonHang => _database.GetCollection<LichSuDonHang>("lichsudonhang");
        public IMongoCollection<Token> Token => _database.GetCollection<Token>("token");
        public IMongoCollection<PhieuLuong> PhieuLuong => _database.GetCollection<PhieuLuong>("phieu_luong");
        public IMongoCollection<BaoCaoDoanhThuNgay> BaoCaoDoanhThuNgay => _database.GetCollection<BaoCaoDoanhThuNgay>("baocao_doanhthu_ngay");
        public IMongoCollection<BaoCaoDoanhThuThang> BaoCaoDoanhThuThang => _database.GetCollection<BaoCaoDoanhThuThang>("baocao_doanhthu_thang");
        public IMongoCollection<BaoCaoDoanhThuNam> BaoCaoDoanhThuNam => _database.GetCollection<BaoCaoDoanhThuNam>("baocao_doanhthu_nam");
        public IMongoCollection<GiaiThuocTinh> GiaiThuocTinh => _database.GetCollection<GiaiThuocTinh>("giaithuoctinh");
        public IMongoCollection<TonKho> TonKho => _database.GetCollection<TonKho>("tonkho");
        public IMongoCollection<ThongSoKyThuat> ThongSoKyThuat => _database.GetCollection<ThongSoKyThuat>("thongsokythuat");
        public IMongoCollection<YeuCauTraHang> YeuCauTraHang => _database.GetCollection<YeuCauTraHang>("yeucautrahang");
    }
}