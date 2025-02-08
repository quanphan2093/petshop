using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;
using System;

namespace PetStore.Pages.Customer
{
    public class ForumDetailModel : PageModel
    {   
        public Forum f { get; set; }
        public Infor i { get; set; }
        public List<Comment> comments { get; set; }
        public List<Account> accounts { get; set; }
        public List<Infor> infors { get; set; }
        private readonly PetStoreContext _context;
        public ForumDetailModel(PetStoreContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int? id)
        {
            f = _context.Forums.Where(x => x.ForumId == id).SingleOrDefault();
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
                                  img = i.Image,
                                  content = c.Content,
                                  name = i.Fullname,
                                  createAt = c.CreateAt
                              };
                comment= comment.OrderByDescending(x => x.createAt).ToList();
                f.Views += 1;
                _context.Forums.Update(f);
                _context.SaveChanges();
                ViewData["comment"]= comment;
                return Page();
            }
            return Page();
        }
        
        public IActionResult OnPostLike(int id)
        {
            f = _context.Forums.Where(x => x.ForumId == id).SingleOrDefault();
            string url = "Forum/" + f.ForumId;
            if (f != null)
            {
                f.Likes += 1;
                _context.Forums.Update(f);
                _context.SaveChanges();
                return RedirectToPage();
            }
            return RedirectToPage(url);
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
