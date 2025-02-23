using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using PetStore.Pages.Common;
using System;
using System.Xml.Linq;
using static PetStore.Pages.Customer.ForumModel;

namespace PetStore.Pages.Customer
{
    public class ForumDetailModel : PageModel
    {
        public Forum f { get; set; }
        public Infor i { get; set; }
        public List<Comment> comments { get; set; }
        public List<Account> accounts { get; set; }
        public List<Infor> infors { get; set; }
        public List<ForumType> forumTypes { get; set; }
        private IFormFile imageFile { get; set; }
        public Infor info { get; set; }
        private readonly PetStoreContext _context;
        private readonly AzureBlobService _blobService;
        public ForumDetailModel(PetStoreContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }
        public IActionResult OnGet(int? id)
        {
            forumTypes = _context.ForumTypes.ToList();
            f = _context.Forums.Where(x => x.ForumId == id).SingleOrDefault();
            info = _context.Infors.FirstOrDefault(x => x.AccountId == f.AccountId);
            comments=_context.Comments.ToList();
            infors=_context.Infors.ToList();
            accounts=_context.Accounts.ToList();
            if(f != null)
            {
                i = _context.Infors.Where(x => x.AccountId == f.AccountId).SingleOrDefault();
                var comment = from c in comments
                              join i in infors on c.AccountId equals i.AccountId
                              where c.ForumId == id
                              select new
                              {
                                  commentId = c.CommentId,
                                  img = i.Image,
                                  content = c.Content,
                                  name = i.Fullname,
                                  createAt = c.CreateAt,
                                  accountId = c.AccountId,
                              };
                var commentReply = from c in comments
                                   join i in infors on c.AccountId equals i.AccountId
                                   where c.ForumId == id && c.ParentCommentId != null
                                   select new
                                   {
                                       commentId = c.CommentId,
                                       img = i.Image,
                                       content = c.Content,
                                       name = i.Fullname,
                                       createAt = c.CreateAt,
                                       accountId = c.AccountId,
                                   };
                comment = comment.OrderByDescending(x => x.createAt).ToList();
                f.Views += 1;
                _context.Forums.Update(f);
                _context.SaveChanges();
                ViewData["comment"]= comment;
                return Page();
            }
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

        public async Task<IActionResult> OnPostEdit(int id, string title, string content, IFormFile? file, string forumType)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            var forum = _context.Forums.Find(id);
            if (accId == null || forum.AccountId != accId)
            {
                return RedirectToPage("/Common/Login");
            }
            f = _context.Forums.Where(x => x.ForumId == id).SingleOrDefault();
            if (forum == null) return RedirectToPage();
            imageFile = file;
            if (imageFile != null)
            {
                string fileName = Path.GetFileName(imageFile.FileName);
                using (var stream = imageFile.OpenReadStream())
                {
                    forum.Image = await _blobService.UploadImageAsync(stream, fileName);
                }
            }
            forum.Title = title;
            forum.Content = content.Replace("\r\n", "\\r\\n").Replace("\n", "\\n");
            forum.TypeId = int.Parse(forumType);
            _context.Forums.Update(forum);
            _context.SaveChanges();

            return RedirectToPage();
        }
        public IActionResult OnPostComment(int id, string content)
        {
            int? accId = HttpContext.Session.GetInt32("acc");
            if(accId == null)
            {
                return RedirectToPage("/Common/Login");
            }
            f = _context.Forums.Where(x => x.ForumId == id).SingleOrDefault();
            string url = "Forum/" + f.ForumId;
            if (f != null)
            {
                Comment c = new Comment();
                c.Content = content;
                c.ForumId = id;
                c.CreateAt= DateTime.Now;
                c.AccountId = accId;
                _context.Comments.Add(c);
                f.Comments += 1;
                _context.Forums.Update(f);
                _context.SaveChanges();
                return RedirectToPage();
            }
            return RedirectToPage(url);
        }

    }
}


