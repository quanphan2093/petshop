using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using PetStore.Pages.Common;

namespace PetStore.Pages.Admin
{
    public class BannerModel : PageModel
    {
        private readonly AzureBlobService _blobService;
        public BannerModel(AzureBlobService blobService)
        {
            _blobService = blobService;
        }
        public List<Banner> lsBanner { get; set; } = new List<Banner>();
        public int totalPage = 0;
        private int pageSize = 10;
        public int currentPage = 0;
        public IActionResult OnGet(int? current = 1)
        {
            string? roleName = HttpContext.Session.GetString("roleName");
            if (roleName == null || roleName != "Admin")
            {
                return Redirect("/Home");
            }
            var banner = PetStoreContext.Ins.Banners.OrderBy(s => s.Status).AsQueryable();
            totalPage = banner.Count() / pageSize;
            if (banner.Count() % pageSize != 0) totalPage += 1;

            banner = banner.Skip((current.Value - 1) * pageSize).Take(pageSize);
            currentPage = current.Value;

            lsBanner = banner.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPost(string method, string url, IFormFile img)
        {
            //string? roleName = HttpContext.Session.GetString("roleName");
            //if (roleName == null || roleName != "Admin")
            //{
            //    return Redirect("/Home");
            //}
            if (method == "create")
            {
                string fileName = Path.GetFileName(img.FileName);
                string pathImg = "";
                using (var stream = img.OpenReadStream())
                {
                    pathImg = await _blobService.UploadImageAsync(stream, fileName);
                }
                Banner banner = new Banner();
                banner.BannerUrl = url;
                banner.BannerImage = pathImg;
                banner.ClickCount = 0;
                banner.CreateAt = DateTime.Now;
                banner.Status = "Active";
                PetStoreContext.Ins.Banners.Add(banner);
                await PetStoreContext.Ins.SaveChangesAsync();
            }
            else if (method == "update")
            {
                int bannerId = int.Parse(Request.Form["bannerId"]);
                Banner banner = PetStoreContext.Ins.Banners.Where(b => b.BannerId == bannerId).FirstOrDefault();
                if(banner != null)
                {
                    banner.BannerUrl = url;
                    if(img != null)
                    {
                        string fileName = Path.GetFileName(img.FileName);
                        string pathImg = "";
                        using (var stream = img.OpenReadStream())
                        {
                            pathImg = await _blobService.UploadImageAsync(stream, fileName);
                        }
                        banner.BannerImage = pathImg;
                    }
                    PetStoreContext.Ins.Banners.Update(banner);
                    await PetStoreContext.Ins.SaveChangesAsync();
                }
            }
            return Redirect("/Admin/Banner");
        }
    }
}
