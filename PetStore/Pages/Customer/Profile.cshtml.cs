﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using PetStore.Pages.Common;

namespace PetStore.Pages.Customer
{
    public class ProfileModel : PageModel
    {
        private readonly Verify_Profile_Update _emailService;
        private readonly AzureBlobService _blobService;
        public List<Forum> lsForum { get; set; } = new List<Forum>();
        public ProfileModel(Verify_Profile_Update emailService, AzureBlobService blobService)
        {
            _emailService = emailService;
            _blobService = blobService;
        }
        public Infor InformationUser { get; set; }
        public Account UserAdditionInfo { get; set; }
        public IActionResult OnGet()
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId != null)
            {
                InformationUser = PetStoreContext.Ins.Infors.
                    Include(x => x.Account).
                    Where(x => x.AccountId == accId).
                    FirstOrDefault();
                UserAdditionInfo = PetStoreContext.Ins.Accounts.
                    Where(x => x.AccountId == accId).
                    FirstOrDefault();
                lsForum = PetStoreContext.Ins.Forums.Include(f => f.Type)
                    .Where(f => f.AccountId == accId && f.Status == "Active").ToList();
                Console.WriteLine(lsForum.Count);
                return Page();
            }
            else
            {
                return RedirectToPage("/Common/Login");
            }
        }
        private string Fullname { get; set; } = "";
        private string Phone { get; set; } = "";
        private string Address { get; set; } = "";
        private string Gender { get; set; } = "";
        private IFormFile imageFile { get; set; }
        public string notificationMessage { get; set; } = "";
        public async Task<IActionResult> OnPost(string? fullname, string? phone, string? gender, string? address, IFormFile? image)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId == null)
            {
                return Redirect("/login");
            }
            InformationUser = PetStoreContext.Ins.Infors.
                   Include(x => x.Account).
                   Where(x => x.AccountId == accId).
                   FirstOrDefault();
            if (accId != null)
            {
                Fullname = fullname ?? "";
                Phone = phone ?? "";
                Gender = gender;
                Address = address ?? "";
                imageFile = image;
                if (InformationUser != null)
                {
                    var Infors = PetStoreContext.Ins.Infors.Include(x => x.Account).
                    Where(x => x.AccountId == accId).
                    FirstOrDefault();
                    if (imageFile != null)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        using (var stream = imageFile.OpenReadStream())
                        {
                            Infors.Image = await _blobService.UploadImageAsync(stream, fileName);
                        }
                    }
                    Infors.Fullname = Fullname;
                    Infors.Phone = Phone;
                    Infors.Address = Address;
                    Infors.Gender = Gender;
                    PetStoreContext.Ins.Infors.Update(Infors);
                    PetStoreContext.Ins.SaveChanges();
                    //await _emailService.SendVerificationEmailAsync(Infors.Account.Email);
                }
                else
                {
                    var Infors = new Infor();
                    if (imageFile != null)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        using (var stream = imageFile.OpenReadStream())
                        {
                            Infors.Image = await _blobService.UploadImageAsync(stream, fileName);
                        }
                    }
                    Infors.Fullname = Fullname;
                    Infors.Phone = Phone;
                    Infors.Address = Address;
                    Infors.Gender = Gender;
                    Infors.AccountId = accId;
                    Infors.StateId = 1;
                    PetStoreContext.Ins.Infors.Add(Infors);
                    PetStoreContext.Ins.SaveChanges();
                    UserAdditionInfo = PetStoreContext.Ins.Accounts.
                    Where(x => x.AccountId == accId).
                    FirstOrDefault();
                    //await _emailService.SendVerificationEmailAsync(UserAdditionInfo.Email);
                }

                InformationUser = PetStoreContext.Ins.Infors.
                    Include(x => x.Account).
                    Where(x => x.AccountId == accId).
                    FirstOrDefault();
                notificationMessage = "Cập nhật thành công!";
            }
            return Page();
        }

    }
}
