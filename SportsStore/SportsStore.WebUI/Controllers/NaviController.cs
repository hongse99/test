using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class NaviController : Controller
    {
        private IProductRepository repository;

        public NaviController(IProductRepository repo) {
            repository = repo;
        } 

        /*
        public PartialViewResult Menu()
        {
            IEnumerable<string> categories = repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);
            return this.PartialView(categories);
        }
        */

        public ViewResult Menu(string category = null)
        {
            ViewBag.selectedCategory = category;
            IEnumerable<string> categories = repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);
            return View(categories);

        }
    }
}
