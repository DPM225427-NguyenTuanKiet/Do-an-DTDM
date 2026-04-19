using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarShop.Controllers
{
    public class ThuongHieuController : Controller
    {
        private readonly IMongoCollection<ThuongHieu> _thuongHieuCollection;
        private readonly IMongoCollection<SanPham> _sanPhamCollection;
        private readonly IMongoCollection<HinhAnh> _hinhAnhCollection;
        private readonly IMongoCollection<DanhMuc> _danhMucCollection;

        public ThuongHieuController(IMongoDatabase database)
        {
            _thuongHieuCollection = database.GetCollection<ThuongHieu>("thuonghieu");
            _sanPhamCollection = database.GetCollection<SanPham>("sanpham");
            _hinhAnhCollection = database.GetCollection<HinhAnh>("hinhanh");
            _danhMucCollection = database.GetCollection<DanhMuc>("danhmuc");
        }

        // Hiển thị danh sách thương hiệu
        public async Task<IActionResult> Index()
        {
            var thuongHieus = await _thuongHieuCollection.Find(_ => true).ToListAsync();
            return View(thuongHieus);
        }

        // Hiển thị sản phẩm theo thương hiệu
        public async Task<IActionResult> SanPhamTheoThuongHieu(int id) // id = IDTH
        {
            var sanPhams = await _sanPhamCollection.Find(sp => sp.IDTH == id).ToListAsync();

            foreach (var sp in sanPhams)
            {
                sp.HinhAnh = await _hinhAnhCollection.Find(ha => ha.IDSP == sp.IDSP).ToListAsync();
                sp.DanhMuc = await _danhMucCollection.Find(dm => dm.IDDM == sp.IDDM).FirstOrDefaultAsync();
            }

            var thuongHieu = await _thuongHieuCollection.Find(th => th.IDTH == id).FirstOrDefaultAsync();
            ViewBag.ThuongHieu = thuongHieu;
            return View(sanPhams);
        }
    }
}