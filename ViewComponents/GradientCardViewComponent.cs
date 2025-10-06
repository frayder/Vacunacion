using Microsoft.AspNetCore.Mvc;

namespace Highdmin.ViewComponents
{
    public class GradientCardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title, string description, string icon = "ri-building-line", 
            string startColor = "#667eea", string endColor = "#764ba2")
        {
            var model = new GradientCardViewModel
            {
                Title = title,
                Description = description,
                Icon = icon,
                StartColor = startColor,
                EndColor = endColor
            };

            return View(model);
        }
    }

    public class GradientCardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string StartColor { get; set; }
        public string EndColor { get; set; }
    }
}