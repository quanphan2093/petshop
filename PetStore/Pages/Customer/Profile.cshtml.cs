using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using PetStore.Pages.Common;
using static PetStore.Pages.Customer.ForumModel;

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
        public List<Infor> infors { get; set; } = new List<Infor>();
        public int? AccountId { get; set; } = 0;
        private void LoadUserData(int? id, int? accId)
        {
            if (accId != null)
            {
                if (accId == id)
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
                }
                else
                {
                    InformationUser = PetStoreContext.Ins.Infors.
                    Include(x => x.Account).
                    Where(x => x.AccountId == id).
                    FirstOrDefault();
                    UserAdditionInfo = PetStoreContext.Ins.Accounts.
                        Where(x => x.AccountId == id).
                        FirstOrDefault();
                    lsForum = PetStoreContext.Ins.Forums.Include(f => f.Type)
                        .Where(f => f.AccountId == id && f.Status == "Active").ToList();
                    AccountId = id;
                }
            }
        }
        public IActionResult OnGet(int? id)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId != null)
            {
                LoadUserData(id, accId);
                return Page();
            }
            return RedirectToPage("/Common/Login");
        }
        private string Fullname { get; set; } = "";
        private string Phone { get; set; } = "";
        private string Address { get; set; } = "";
        private string Gender { get; set; } = "";
        private IFormFile imageFile { get; set; }
        public string notificationMessage { get; set; } = "";
        public async Task<IActionResult> OnPost(int?id, string? fullname, string? phone, string? gender, string? address, IFormFile? image)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId == null)
            {
                return Redirect("/login");
            }
            LoadUserData(id, accId);
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

        public JsonResult OnPostLike([FromBody] LikeRequest request)
        {
            try
            {
                var forum = PetStoreContext.Ins.Forums.Find(request.Id);
                if (forum == null)
                {
                    return new JsonResult(new { success = false, message = "Bài viết không tồn tại" });
                }

                forum.Likes += 1;
                PetStoreContext.Ins.Forums.Update(forum);
                PetStoreContext.Ins.SaveChanges();

                return new JsonResult(new { success = true, likes = forum.Likes });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        public IActionResult OnPostPin(int? id)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId == null)
            {
                return Redirect("/Login");
            }
            else
            {
                List<Forum> fList = PetStoreContext.Ins.Forums.Where(x => x.IsPinned == true).ToList();
                Forum f = PetStoreContext.Ins.Forums.Where(x => x.ForumId == id).FirstOrDefault();
                if (f == null) return Redirect("/Forum");
                if (fList.Count >= 2)
                {
                    if (f.IsPinned == true) { 
                        f.IsPinned = false;
                        PetStoreContext.Ins.Forums.Update(f);
                        PetStoreContext.Ins.SaveChanges();
                    }
                    else if (f.IsPinned == false)
                    {
                        notificationMessage = "Chỉ được phép ghim tối đa 2 bài viết! Hãy bỏ ghim bài viết cũ";
                    }
                    return Redirect("/Profile/" + accId);
                }
                else
                {
                    if (f.IsPinned == true) f.IsPinned = false;
                    else f.IsPinned = true;
                    PetStoreContext.Ins.Forums.Update(f);
                    PetStoreContext.Ins.SaveChanges();
                    return Redirect("/Profile/" + accId);
                }
                
                
            }
        }
    }
}
