using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using System.Linq;

namespace PetStore.Pages.Customer
{
    public class ProductModel : PageModel
    {
        public List<Product> lsProduct { get; set; } = new List<Product>();
        public List<Category> lsCategory { get; set; } = new List<Category>();
        

        [BindProperty]
        public string Search { get; set; } = "";
        [BindProperty]
        public string Price { get; set; } = "all";
        [BindProperty]
        public List<string> Category { get; set; } = new List<string>();
        [BindProperty]
        public string dataCate { get; set; } = "";

        [BindProperty]
        public bool isSearch { get; set; } = false;

        [BindProperty]
        public bool isFilter { get; set; } = false;

        public int totalPage = 0;
        private int pageSize = 9;
        public int currentPage = 0;

        public void OnGet(string? categories, int? current = 1)
        {
            var context = new PetStoreContext();
            var products = context.Products.Include(p => p.Category).Where(p => p.Status == "Available").AsQueryable();

            //filter by category
            if (!string.IsNullOrEmpty(categories))
            {
                products = products.Where(p => p.Category.CategoryName.ToLower().Equals(categories.ToLower()));
                dataCate = categories;
                isFilter = true;
                isSearch = false;
            }

            //calculate page size
            calculatePage(products);


            products = products.Skip((current.Value-1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsProduct = products.ToList();
            lsCategory = context.Categories.ToList();
        }

        public void OnPost(string? search, string? price, List<string> category, int? current = 1)
        {
            var context = new PetStoreContext();
            var products = context.Products.Include(p => p.Category).AsQueryable();

            products = Filter(products, category, search, price);

            calculatePage(products);
            isSearch = true;
            isFilter = false;

            //paging
            products = products.Skip((current.Value - 1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsProduct = products.ToList();
            lsCategory = context.Categories.ToList();
        }

        private IQueryable<Product> Filter(IQueryable<Product> products, List<string> category, string? search = "", string? price = "")
        {
            //result search by keywords
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(search.ToLower()));
                Search = search;
            }

            //result filter by price
            if (!string.IsNullOrEmpty(price))
            {
                switch (price)
                {
                    case "below100k":
                        products = products.Where(p => p.Price < 100000);
                        break;
                    case "100k-500k":
                        products = products.Where(p => p.Price >= 100000 && p.Price <= 500000);
                        break;
                    case "500k-1m":
                        products = products.Where(p => p.Price > 500000 && p.Price <= 1000000);
                        break;
                    case "1m-1.5m":
                        products = products.Where(p => p.Price > 1000000 && p.Price <= 1500000);
                        break;
                }
                Price = price;
            }

            //result filter by category
            if (category.Count() != 0)
            {
                products = products.Where(p => category.Contains(p.CategoryId.ToString()));
                Category.AddRange(category);
            }
            return products;
        }

        private void calculatePage(IQueryable<Product> products)
        {
            totalPage = products.Count() / pageSize;
            if (products.Count() % pageSize != 0) totalPage += 1;
        }
    }
}
