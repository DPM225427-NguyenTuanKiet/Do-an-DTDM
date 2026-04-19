namespace CarShop.ViewModels
{
    public class SanPhamListViewModel
    {
        public int IDSP { get; set; }
        public string TENSP { get; set; } = null!;
        public decimal GIA { get; set; }
        public string? HINHANH { get; set; }
        public string TENDANHMUC { get; set; } = null!;
        public string TENTHUONGHIEU { get; set; } = null!;
    }
}
