using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Pages.Customer
{
    public class DeleteCommentModel : PageModel
    {
        public async Task<IActionResult> OnGet(int? id = 1)
        {
            try
            {
                Comment cm = await PetStoreContext.Ins.Comments.FirstOrDefaultAsync(f => f.CommentId == id);
                if (cm == null)
                {
                    return Redirect("/Forum");
                }
                int? accId = HttpContext.Session.GetInt32("acc");
                if (accId != null)
                {
                    if (cm.AccountId == accId)
                    {
                        PetStoreContext.Ins.Remove(cm);
                        await PetStoreContext.Ins.SaveChangesAsync();
                        return Redirect("/Forum/" + cm.ForumId);
                    }
                }
                return Redirect("/Forum");
            }
            catch (Exception ex) {
                return Redirect("/Home");
            }
            
        }
    }
}
