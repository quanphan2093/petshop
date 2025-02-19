using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;
using PetStore.Pages.Common;

namespace PetStore.Pages.Customer
{
    public class HomeModel : PageModel
    {   
        public List<Product> products { get; set; }= new List<Product>();
        public List<Forum> forums { get; set; }= new List<Forum>();
        public List<Banner> banner { get; set; } = new List<Banner>();
        public void OnGet()
        {
            products= PetStoreContext.Ins.Products.Include(x => x.Category).Where(x => x.Status.Equals("Available")).OrderByDescending(x => x.CreateAt).ToList();  
            forums = PetStoreContext.Ins.Forums.Where(x => x.Status.Equals("Available")).OrderByDescending(x => x.CreateAt).ToList();
            banner = PetStoreContext.Ins.Banners.Where(b => b.Status == "Active").ToList();
        }
	}
}
