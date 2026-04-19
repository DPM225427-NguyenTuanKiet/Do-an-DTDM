using Microsoft.AspNetCore.Mvc;
using CarShop.Services;
using CarShop.Models;

namespace CarShop.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly DanhMucService _danhMucService;

        public CategoryMenuViewComponent(DanhMucService danhMucService)
        {
            _danhMucService = danhMucService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _danhMucService.GetAllAsync();
            return View(categories);
        }
    }
}