using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using WebModels;
using System.Collections;
using Kendo.Mvc;
using Kendo.Mvc.Infrastructure;
using System.Linq.Expressions;
using System.ComponentModel;
using System.IO;
using Common;
using WebMatrix.WebData;
using System.Web.Security;
using System.Data;
using System.Transactions;
using System.Data.Entity.Validation;

namespace WEB.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserController : BaseController
    {
        WebContext db = new WebContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users_Read([DataSourceRequest] DataSourceRequest request)
        {
            var users = from x in db.UserProfiles.AsNoTracking()
                        select new { x.UserId, x.UserName, x.AccountType,
                x.Email, x.Mobile, x.FullName };
            return Json(users.ToDataSourceResult(request));
        }

        public JsonResult GetPlayer()
        {
            var users = db.UserProfiles.Where(x => x.IsActive == (int)Status.Public
            && x.AccountType != "Admin").Select(x => new PlayerDto
            {
                UserId = x.UserId,
                FullName = x.FullName + " (" + x.SportType + ")"
            }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Exclude = "")] RegisterModel model, string[] roles, HttpPostedFileBase image)
        {
            ModelState.Remove("UserProfile.UserName");
            model.UserProfile.UserName = model.UserName;
            if (ModelState.IsValid)
            {
                var temp = (from p in db.Set<UserProfile>().AsNoTracking()
                            where p.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)
                            select p).FirstOrDefault();
                if (temp != null)
                {
                    ModelState.AddModelError("", AccountResources.UserNameExists);
                    return View(model);
                }
                else
                {
                    if (image != null)
                    {
                        var name = image.FileName;
                        string extension = Path.GetExtension(name);
                        var newName = model.UserName + extension;
                        var dir = new System.IO.DirectoryInfo(Server.MapPath("/content/uploads/avatars/"));
                        if (!dir.Exists) dir.Create();
                        var uri = "/content/uploads/avatars/" + newName;
                        image.SaveAs(HttpContext.Server.MapPath(uri));
                        try
                        {
                            if (ImageTools.ValidateImage(System.Web.HttpContext.Current.Server.MapPath(uri)))
                            {
                                var result = ImageTools.ResizeImage(Server.MapPath(uri), Server.MapPath(uri), 240, 240, true, 80);
                                model.UserProfile.Avatar = uri;
                            }
                            else
                            {
                                Utility.DeleteFile(uri);
                            }
                        }
                        catch (Exception)
                        { }
                    }

                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        ClubId = model.UserProfile.ClubId,
                        SocialNetworkId = model.UserProfile.SocialNetworkId,
                        Password = model.Password,
                        Email = model.UserProfile.Email,
                        Avatar = model.UserProfile.Avatar,
                        FullName = model.UserProfile.FullName,
                        Birthday = model.UserProfile.Birthday,
                        AccountType = model.UserProfile.AccountType,
                        IsActive = model.UserProfile.IsActive,
                        Point = model.UserProfile.Point,
                        WinMatch = model.UserProfile.WinMatch,
                        CreatedAt = DateTime.Now,
                        CreatedBy = WebSecurity.CurrentUserId,
                        Mobile = model.UserProfile.Mobile,
                    });

                    try
                    {
                        Roles.AddUserToRoles(model.UserName, roles);
                    }
                    catch (Exception)
                    { }
                    ViewBag.StartupScript = "create_success();";
                    return View(model);
                }

            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {

            var user = db.Set<UserProfile>().Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.cUserName = user.UserName;
            ViewBag.Roles = Roles.GetRolesForUser(user.UserName);
            return View("Edit", user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] UserProfile model,
            string cUserName, string[] roles, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.Roles = roles;

                    var temp = (from p in db.Set<UserProfile>().AsNoTracking()
                                where p.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)
                                && !p.UserName.Equals(cUserName, StringComparison.OrdinalIgnoreCase)
                                select p).FirstOrDefault();

                    if (temp != null)
                    {
                        ModelState.AddModelError("", AccountResources.UserNameExists);
                        return View(model);
                    }
                    else
                    {
                        var delavatar = false;
                        if (image != null)
                        {
                            var name = image.FileName;
                            string extension = Path.GetExtension(name);
                            var newName = model.UserName + extension;
                            var dir = new System.IO.DirectoryInfo(Server.MapPath("/content/uploads/avatars/"));
                            if (!dir.Exists) dir.Create();
                            var uri = "/content/uploads/avatars/" + newName;
                            image.SaveAs(HttpContext.Server.MapPath(uri));
                            try
                            {
                                if (ImageTools.ValidateImage(System.Web.HttpContext.Current.Server.MapPath(uri)))
                                {
                                    var result = ImageTools.ResizeImage(Server.MapPath(uri), Server.MapPath(uri), 240, 240, true, 80);
                                    model.Avatar = uri;
                                }
                                else
                                {
                                    Utility.DeleteFile(uri);
                                    model.Avatar = null;
                                }
                            }
                            catch (Exception)
                            { }
                        }
                        else
                        {
                            if (model.Avatar == null)
                            {
                                delavatar = true;
                            }
                        }

                        var modelEdit = db.UserProfiles.Find(model.UserId);
                        modelEdit.ClubId = model.ClubId;
                        modelEdit.SocialNetworkId = model.SocialNetworkId;
                        modelEdit.Email = model.Email;
                        if (!delavatar)
                        {
                            modelEdit.Avatar = model.Avatar;
                        }
                        modelEdit.FullName = model.FullName;
                        modelEdit.Birthday = model.Birthday;
                        modelEdit.AccountType = model.AccountType;
                        modelEdit.IsActive = model.IsActive;
                        modelEdit.Point = model.Point;
                        modelEdit.WinMatch = model.WinMatch;
                        modelEdit.ModifiedAt = DateTime.Now;
                        modelEdit.ModifiedBy = WebSecurity.CurrentUserId;
                        modelEdit.Mobile = model.Mobile;
                        modelEdit.SportType = model.SportType;
                        modelEdit.Gender = model.Gender;
                        modelEdit.UserName = model.UserName;

                        db.SaveChanges();
                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }


                try
                {
                    foreach (var role in Roles.GetRolesForUser(model.UserName))
                    {
                        Roles.RemoveUserFromRole(model.UserName, role);
                    }
                    Roles.AddUserToRoles(model.UserName, roles);
                }
                catch (Exception)
                { }
                ViewBag.StartupScript = "create_success();";
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {

            var user = db.Set<UserProfile>().Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("Delete", user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserProfile model)
        {



            try
            {
                ((SimpleMembershipProvider)Membership.Provider).DeleteUser(model.UserName, true);
                ViewBag.StartupScript = "delete_success();";
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

        }

        public ActionResult Deletes(string id)
        {

            var objects = id.Split(',');
            var lstUserId = new List<int>();
            foreach (var obj in objects)
            {
                try
                {
                    lstUserId.Add(int.Parse(obj.ToString()));
                }
                catch (Exception)
                { }
            }

            var temp = from p in db.Set<UserProfile>()
                       where lstUserId.Contains(p.UserId)
                       select p;

            return View(temp.ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletes(List<UserProfile> model)
        {

            var temp = new List<UserProfile>();
            foreach (var item in model)
            {
                try
                {
                    ((SimpleMembershipProvider)Membership.Provider).DeleteUser(item.UserName, true);
                    db.SaveChanges();

                }
                catch (Exception)
                {
                    temp.Add(item);
                }
            }

            if (temp.Count == 0)
            {
                ViewBag.StartupScript = "deletes_success();";
                return View(temp);
            }
            else if (temp.Count > 0)
            {
                ViewBag.StartupScript = "top.$('#grid').data('kendoGrid').dataSource.read();";
                ModelState.AddModelError("", Resources.Common.ErrorDeleteItems);
                return View(temp);
            }
            else
            {
                ViewBag.StartupScript = "deletes_success();";
                return View();
            }

        }

        public ActionResult ChangePassword(string userName)
        {
            if (!WebSecurity.UserExists(userName))
            {
                return HttpNotFound();
            }

            ViewBag.UserName = userName;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model, string userName)
        {
            bool changePasswordSucceeded;

            try
            {
                string token = WebSecurity.GeneratePasswordResetToken(userName, 30);
                changePasswordSucceeded = WebSecurity.ResetPassword(token, model.NewPassword);
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }

            if (changePasswordSucceeded)
            {
                ViewBag.StartupScript = "change_success();";
            }
            else
            {
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            return View();
        }

        //public ActionResult AdminSitesMapping(int id)
        //{
        //    var user = db.UserProfiles.Find(id);
        //    if (user == null)
        //        return HttpNotFound();
        //    var temp = user.AccessAdminSites.Select(x => x.AdminSite).Select(x => new { ID = x.ID }).ToArray();
        //    string[] accesses = temp.Count() > 0 ? new string[temp.Count()] : new string[0];
        //    for (int i = 0; i < temp.Count(); i++)
        //        accesses[i] = temp[i].ID.ToString();
        //    ViewBag.Tree = GetAdminSitesTree();
        //    ViewBag.UserId = id;
        //    return View(accesses);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AdminSitesMapping(int id, string[] checkedNodes)
        //{
        //    var user = db.UserProfiles.Find(id);
        //    if (user == null)
        //        return HttpNotFound();
        //    List<int> lstSiteID = new List<int>();
        //    try
        //    {
        //        using (var ts = new TransactionScope())
        //        {
        //            if (checkedNodes != null && checkedNodes.Count() > 0)
        //                foreach (var x in checkedNodes)
        //                    lstSiteID.Add(int.Parse(x));
        //            user.AccessAdminSites.Clear();
        //            if (lstSiteID.Count > 0)
        //                foreach (var x in lstSiteID)
        //                    user.AccessAdminSites.Add(new AccessAdminSite() { AdminSiteID = x });
        //            db.SaveChanges();
        //            ts.Complete();
        //            ViewBag.StartupScript = "create_success();";

        //            // clear cache
        //            var sessionKey = "AdminSite-" + Culture + "-" + HttpContext.User.Identity.Name;
        //            Session[sessionKey] = null;

        //            return View();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        var temp = user.AccessAdminSites.Select(x => x.AdminSite).Select(x => new { ID = x.ID }).ToArray();
        //        string[] accesses = temp.Count() > 0 ? new string[temp.Count()] : new string[0];
        //        for (int i = 0; i < temp.Count(); i++)
        //            accesses[i] = temp[i].ID.ToString();
        //        ViewBag.Tree = GetAdminSitesTree();
        //        ViewBag.UserId = id;
        //        return View(accesses);
        //    }
        //}

        //public JsonResult GetAccessAdminSites(int? id)
        //{
        //    var adminSites = from e in db.AdminSites.AsNoTracking()
        //                     where (id.HasValue ? e.ParentID == id : e.ParentID == null)
        //                     select new
        //                     {
        //                         id = e.ID,
        //                         Name = e.Title,
        //                         hasChildren = e.SubAdminSites.Any()
        //                     };
        //    return Json(adminSites, JsonRequestBehavior.AllowGet);
        //}

        //private IEnumerable<TreeViewItemModel> GetAdminSitesTree()
        //{
        //    List<TreeViewItemModel> tree = new List<TreeViewItemModel>();
        //    var roots = db.AdminSites.AsNoTracking().Where(x => x.ParentID == null).AsEnumerable();
        //    if (roots.Count() > 0)
        //        foreach (var item in roots)
        //        {
        //            TreeViewItemModel node = new TreeViewItemModel();
        //            node.Id = item.ID.ToString();
        //            node.Text = item.Title;
        //            node.HasChildren = item.SubAdminSites.Any();
        //            if (node.HasChildren)
        //                SubAdminSitesTree(ref node, ref tree);
        //            tree.Add(node);
        //        }
        //    return tree;
        //}

        private void SubAdminSitesTree(ref TreeViewItemModel parentNode, ref List<TreeViewItemModel> tree)
        {
            int parentID = int.Parse(parentNode.Id);
            var nodes = db.AdminSites.AsNoTracking().Where(x => x.ParentID == parentID).AsEnumerable();
            foreach (var item in nodes)
            {
                TreeViewItemModel node = new TreeViewItemModel();
                node.Id = item.ID.ToString();
                node.Text = item.Title;
                node.HasChildren = item.SubAdminSites.Any();
                parentNode.Items.Add(node);
                if (node.HasChildren)
                    SubAdminSitesTree(ref node, ref tree);
            }
        }

        //public ActionResult ModulesMapping(int id)
        //{
        //    ViewBag.UserId = id;
        //    return View();
        //}

        //[HttpGet]
        //public JsonResult GetModulesPermissions(int id)
        //{
        //    var user = db.UserProfiles.Find(id);
        //    if (user == null)
        //        return Json(null);
        //    var modulesPermissions = user.AccessWebModules
        //        .Select(x =>
        //            new
        //            {
        //                WebModuleID = x.WebModuleID,
        //                View = x.View,
        //                Add = x.Add,
        //                Edit = x.Edit,
        //                Delete = x.Delete
        //            });
        //    return Json(modulesPermissions, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult UpdateModulePerm(AccessWebModule data)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var item = db.AccessWebModules.AsNoTracking().SingleOrDefault(x => x.UserId == data.UserId && x.WebModuleID == data.WebModuleID);
        //        if (item != null)
        //        {
        //            db.Entry(item).State = System.Data.Entity.EntityState.Detached;
        //            db.AccessWebModules.Attach(data);
        //            db.Entry(data).Property(x => x.View).IsModified = true;
        //            db.Entry(data).Property(x => x.Add).IsModified = true;
        //            db.Entry(data).Property(x => x.Edit).IsModified = true;
        //            db.Entry(data).Property(x => x.Delete).IsModified = true;
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            db.AccessWebModules.Add(data);
        //            db.SaveChanges();
        //        }
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false });
        //}
    }
}
