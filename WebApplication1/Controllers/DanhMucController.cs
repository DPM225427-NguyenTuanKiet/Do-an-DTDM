using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarShop.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly IMongoCollection<DanhMuc> _danhMucCollection;
        private readonly IMongoCollection<SanPham> _sanPhamCollection;
        private readonly IMongoCollection<HinhAnh> _hinhAnhCollection;
        private readonly IMongoCollection<ThuongHieu> _thuongHieuCollection;

        public DanhMucController(IMongoDatabase database)
        {
            _danhMucCollection = database.GetCollection<DanhMuc>("danhmuc");
            _sanPhamCollection = database.GetCollection<SanPham>("sanpham");
            _hinhAnhCollection = database.GetCollection<HinhAnh>("hinhanh");
            _thuongHieuCollection = database.GetCollection<ThuongHieu>("thuonghieu");
        }

        // Hiển thị danh sách danh mục (Code siêu gọn vì ảnh đã lưu trong DB)
        public async Task<IActionResult> Index()
        {
            var danhMucs = await _danhMucCollection.Find(_ => true).ToListAsync();
            return View(danhMucs);
        }

        // Hiển thị sản phẩm theo danh mục
        public async Task<IActionResult> SanPhamTheoDanhMuc(int id) // id = IDDM
        {
            var sanPhams = await _sanPhamCollection.Find(sp => sp.IDDM == id).ToListAsync();

            // Lấy thêm hình ảnh và thương hiệu cho mỗi sản phẩm
            foreach (var sp in sanPhams)
            {
                sp.HinhAnh = await _hinhAnhCollection.Find(ha => ha.IDSP == sp.IDSP).ToListAsync();
                sp.ThuongHieu = await _thuongHieuCollection.Find(th => th.IDTH == sp.IDTH).FirstOrDefaultAsync();
            }

            var danhMuc = await _danhMucCollection.Find(dm => dm.IDDM == id).FirstOrDefaultAsync();
            ViewBag.TenDanhMuc = danhMuc?.TENDANHMUC ?? "Danh mục không xác định";

            return View(sanPhams);
        }
    }
}