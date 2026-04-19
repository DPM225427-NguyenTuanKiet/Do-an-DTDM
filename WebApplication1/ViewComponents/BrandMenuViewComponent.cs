using Microsoft.AspNetCore.Mvc;
using CarShop.Services;

namespace CarShop.ViewComponents
{
    public class BrandMenuViewComponent : ViewComponent
    {
        private readonly ThuongHieuService _thuongHieuService;

        public BrandMenuViewComponent(ThuongHieuService thuongHieuService)
        {
            _thuongHieuService = thuongHieuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _thuongHieuService.GetAllAsync();
            return View(brands);
        }
    }
}