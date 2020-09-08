using System.Web.Mvc;
using WebModels;

namespace WEB.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AccessWebModuleController : BaseController
    {
        WebContext db = new WebContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        //
        // GET: /Admin/AccessLog/

        public ActionResult _Index(int id)
        {
            
            return PartialView();
        }
    }
}
