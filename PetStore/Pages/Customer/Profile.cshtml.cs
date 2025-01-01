using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace PetStore.Pages.Customer
{
    public class ProfileModel : PageModel
    {
        private readonly Verify_Profile_Update _emailService;
        public ProfileModel(Verify_Profile_Update emailService)
        {
            _emailService = emailService;
        }
        public Infor Account { get; set; }
        public IActionResult OnGet()
        {
            //string accId = HttpContext.Session.GetString("accId");
            string accId = "1";
            if (accId != null)
            {
                Account = PetStoreContext.Ins.Infors.
                    Include(x => x.Account).
                    Where(x => x.AccountId == int.Parse(accId)).
                    FirstOrDefault();
                return Page();
            }
            else
            {
                return RedirectToPage("/Guest/Restoran_Login/Login");
            }
        }
        private string Fullname { get; set; } = "";
        private string Phone { get; set; } = "";
        private string Address { get; set; } = "";
        private string Gender { get; set; } = "";
        private IFormFile imageFile { get; set; }
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";
        //private String file { get; set; } = ""; 
        public async Task<IActionResult> OnPost(string? fullname, string? phone, string? gender, string? address, IFormFile? image)
        {
            //string? accId = HttpContext.Session.GetString("accId");
            string accId = "1";
            if (accId != null)
            {
                Fullname = fullname;
                Phone = phone;
                Gender = gender;
                Address = address;
                imageFile = image;
                var Infors = PetStoreContext.Ins.Infors.Include(x => x.Account).
                    Where(x => x.AccountId == int.Parse(accId)).
                    FirstOrDefault();
                if (imageFile != null)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }
                    Infors.Image = $"/Images/{uniqueFileName}";
                }
                Infors.Fullname = Fullname;
                Infors.Phone = Phone;
                Infors.Address = Address;
                Infors.Gender = Gender;
                PetStoreContext.Ins.Infors.Update(Infors);
                PetStoreContext.Ins.SaveChanges();
                await _emailService.SendVerificationEmailAsync(Infors.Account.Email);
                Account = PetStoreContext.Ins.Infors.
                    Include(x => x.Account).
                    Where(x => x.AccountId == int.Parse(accId)).
                    FirstOrDefault();
                successMessage = "Update successfully!";
            }
            else
            {
                errorMessage = "Data not found!";
            }
            return Page();
        }

    }
}
