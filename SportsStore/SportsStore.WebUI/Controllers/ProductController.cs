using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;


namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 4;
        private IProductRepository repository;

        public ProductController(IProductRepository productRepository) {
            repository = productRepository;
        }
        /*
        public ViewResult List(int page=1) {
            //return View(repository.Products);
            return View(repository.Products
                .OrderBy(p=>p.ProductID)
                .Skip((page -1)*PageSize)
                .Take(PageSize) );
        }
        */

        public ViewResult List(string category, int page = 1) {
            ProductsListViewModel viewModel = new ProductsListViewModel
            {
                Products = repository.Products
                    .Where(p => category ==null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? 
                        repository.Products.Count() : 
                        repository.Products.Where(p => p.Category == category).Count()
                },
                CurrentCategory = category
            };
            return View(viewModel);
        }

        //public actionresult index()
        //{
        //    return view();
        //}

    }
}
