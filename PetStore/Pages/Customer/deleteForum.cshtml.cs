using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class deleteForumModel : PageModel
    {
        public IActionResult OnGet(int? id = 0)
        {
            int? acc = HttpContext.Session.GetInt32("acc");
            if (acc != null)
            {
                Forum forum = PetStoreContext.Ins.Forums.Where(f => f.ForumId == id).FirstOrDefault();
                if (forum != null)
                {
                    if (acc == forum.AccountId)
                    {
                        forum.Status = "Inactive";
                        PetStoreContext.Ins.Forums.Update(forum);
                        PetStoreContext.Ins.SaveChanges();
                        return Redirect("/Forum");
                    }
                }
            }
            return Redirect("/Forum/" + id);
        }
    }
}
