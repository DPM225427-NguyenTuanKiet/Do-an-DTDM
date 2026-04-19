using CarShop.Models; 

namespace CarShop.ViewModels 
{
    public class HomeViewModel
    {
        public List<DanhMuc> Categories { get; set; } = new();
        public List<ThuongHieu> Brands { get; set; } = new();
        public List<SanPham> NewProducts { get; set; } = new();
        public List<SanPham> SaleProducts { get; set; } = new();
        public List<KhuyenMai> Promotions { get; set; } = new();
    }
}