using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class ForumModel : PageModel
    {
        public List<Forum> Forums { get; set; } = new List<Forum>();
        public List<Account> Account { get; set; } = new List<Account>();
        public List<Infor> infors { get; set; } = new List<Infor>();
        public List<Comment> comments { get; set; }
        public List<Hashtag> hashtags { get; set; }
        public List<PostHashtag> postHashtags { get; set; }
        public List<Product> products { get; set; } = new List<Product>();
        public List<ForumType> forumTypes { get; set; }
        public string Search;
        private readonly PetStoreContext _context;
        public string Acc;
        private const int PageSize = 5;
        private IFormFile imageFile { get; set; }
        public ForumModel(PetStoreContext context)
        {
            _context = context;
        }

        private IEnumerable<dynamic> getForum(string search, string type)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId.HasValue)
            {
                var info = _context.Infors.SingleOrDefault(x => x.AccountId == accId);
                if (info != null)
                {
                    Acc = info.Fullname;
                }
            }

            Forums = _context.Forums.ToList();
            Account = _context.Accounts.ToList();
            infors = _context.Infors.ToList();
            postHashtags = _context.PostHashtags.ToList();
            hashtags = _context.Hashtags.ToList();
            forumTypes = _context.ForumTypes.ToList();
            products = _context.Products
                .Include(x => x.Category)
                .Where(x => x.Status == "Available")
                .OrderByDescending(x => x.CreateAt)
                .Take(5)
                .ToList();

            var forum = from a in Account
                        join i in infors on a.AccountId equals i.AccountId
                        join f in Forums on a.AccountId equals f.AccountId
                        where f.Status == "Active"
                        select new
                        {
                            id = f.ForumId,
                            content = f.Content,
                            views = f.Views,
                            likes = f.Likes,
                            comments = f.Comments,
                            title = f.Title,
                            image = f.Image,
                            createAt = f.CreateAt,
                            name = i.Fullname,
                            typeId = f.TypeId,
                            imgProfile = i.Image
                        };

            forum = forum.OrderByDescending(x => x.createAt).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                forum = forum.Where(x => x.title?.ToLower().Contains(search.ToLower()) == true 
                || x.content.ToLower().Contains(search.ToLower()) == true).ToList();
            }

            if (!string.IsNullOrEmpty(type) && !type.Equals("All"))
            {
                if (int.TryParse(type, out int typeId))
                {
                    forum = forum.Where(x => x.typeId == typeId).ToList();
                }
            }

            return forum;
        }

        public void GetData(string search, string type)
        {
            
            var hashtag = _context.PostHashtags
                          .Include(ph => ph.Forum)
                          .Include(ph => ph.Hashtag)
                          .GroupBy(ph => new { ph.HashtagId, ph.Hashtag.Tag })
                          .Select(g => new
                          {
                              HashtagId = g.Key.HashtagId,
                              HashtagName = g.Key.Tag,
                              Count = g.Count()
                          })
                          .OrderByDescending(g => g.Count)
                          .Take(3)
                          .ToList();
            var forum = getForum(search, type);
            forum=forum.Take(PageSize).ToList();
            ViewData["forum"] = forum;
            ViewData["hashtag"] = hashtag;

        }
        public IActionResult OnGet(string search, string type)
        {
            GetData(search, type);
            return Page();
        }

        public JsonResult OnPostLike([FromBody] LikeRequest request)
        {
            try
            {
                var forum = _context.Forums.Find(request.Id);
                if (forum == null)
                {
                    return new JsonResult(new { success = false, message = "Bài viết không tồn tại" });
                }

                forum.Likes += 1;
                _context.Forums.Update(forum);
                _context.SaveChanges();

                return new JsonResult(new { success = true, likes = forum.Likes });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public IActionResult OnPost(string title, string content, IFormFile? file, string search, string type, string forumType)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if (accId == null)
            {
                return RedirectToPage("/Common/Login");
            }
            if(string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content)) 
            {
                TempData["error"] = "Vui lòng điền đầy đủ tất cả các trường";
            }
            GetData(search, type);
            var acc = _context.Accounts.Where(x => x.AccountId == accId).SingleOrDefault();
            var info = _context.Infors.Where(x => x.AccountId == accId).SingleOrDefault();
            if (info == null)
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin của bạn trước khi đăng bài";
                return Redirect("/Profile");
            }
            imageFile = file;
            Forum f = new Forum();
            f.Title = title;
            f.Content = content.Replace("\r\n", "\\r\\n").Replace("\n", "\\n");
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
                f.Image = $"/Images/{uniqueFileName}";
            }
            f.CreateAt = DateTime.Now;
            f.Likes = 0;
            f.Views = 0;
            f.Comments = 0;
            f.AccountId = accId;
            f.Status = "Active";
            f.TypeId = int.Parse(forumType);
            _context.Forums.Add(f);
            _context.SaveChanges();

            return RedirectToPage("Forum");
        }

        public JsonResult OnGetMoreItems(string search, string type, int skip, int take = PageSize)
        {
            var forum = getForum(search, type);
            var moreItems = forum.Skip(skip).Take(take).ToList();
            return new JsonResult(moreItems);
        }
        public class LikeRequest
        {
            public int Id { get; set; }
        }
    }
}
