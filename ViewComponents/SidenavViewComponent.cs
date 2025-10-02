using Highdmin.Services;
using Highdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Highdmin.ViewComponents
{
    public class SidenavViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public SidenavViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userName = HttpContext.User.Identity?.Name ?? "admin";
            var sidenavData = await _menuService.GetSidenavDataAsync(userName);
            
            return View(sidenavData);
        }
    }
}