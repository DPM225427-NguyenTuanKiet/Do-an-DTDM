using CarShop.Extensions;
using CarShop.Models;
using CarShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Controllers
{
    public class SoSanhController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private const string CompareSessionKey = "CompareList";

        public SoSanhController(SanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        private List<int> GetCompareList()
        {
            return HttpContext.Session.GetObject<List<int>>(CompareSessionKey) ?? new List<int>();
        }

        private void SaveCompareList(List<int> list)
        {
            HttpContext.Session.SetObject(CompareSessionKey, list);
        }

        [HttpPost]
        public IActionResult AddToCompare(int productId)
        {
            var list = GetCompareList();
            if (!list.Contains(productId) && list.Count < 3)
            {
                list.Add(productId);
                SaveCompareList(list);
                TempData["Success"] = "Đã thêm xe vào danh sách so sánh";
            }
            else if (list.Count >= 3)
            {
                TempData["Error"] = "Chỉ được so sánh tối đa 3 xe";
            }
            return RedirectToAction("Index", "Sosanh");
        }

        [HttpPost]
        public IActionResult RemoveFromCompare(int productId)
        {
            var list = GetCompareList();
            list.Remove(productId);
            SaveCompareList(list);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Compare()
        {
            var list = GetCompareList();
            var products = new List<SanPham>();
            foreach (var id in list)
            {
                var p = await _sanPhamService.GetByIdAsync(id);
                if (p != null) products.Add(p);
            }
            return View(products);
        }
    }
}