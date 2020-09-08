using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WebModels;
using WEB.Models;
using System.Web.Caching;
using Newtonsoft.Json.Linq;
using Kendo.Mvc;
using WebMatrix.WebData;
using Common;
using System.Text.RegularExpressions;

namespace WEB.Areas.ContentType.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseController
    {
        WebModels.WebContext db = new WebModels.WebContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [ChildActionOnly]
        public ActionResult _Index(int id)
        {


            ViewBag.ID = id;
            return PartialView();
        }

        //---------------------------------------------------
        [ChildActionOnly]
        public ActionResult _ProductNew(int take)
        {

            return PartialView();
        }

        public ActionResult _ProductNew_Read(int take)
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_ProductNew_Type");
            var data = ids.Split(',').ToList();
            var languageId = ApplicationService.Culture;
            var result = (from x in db.WebContents
                          where x.WebModuleID != null && data.Contains(x.WebModule.ContentTypeID)
                          orderby x.ID descending
                          select x).Skip(0).Take(take);
            var jarray = new JArray();
            foreach (var node in result)
            {
                var item = node;
                var moduletran = item.WebModule;
                var jobject = new JObject() { new JProperty("ID", item.ID), new JProperty("Title", item.Title), new JProperty("MetaTitle", item.MetaTitle), new JProperty("WebModuleID", item.WebModuleID), new JProperty("WebModule_MetaTitle", moduletran.MetaTitle) };
                jarray.Add(jobject);
            }


            return Json(new { json = jarray.ToString() }, JsonRequestBehavior.AllowGet);
        }







        //-----------------------------------------
        private List<WebContent> GetListContents(int webModuleId, List<WebContent> results)
        {
            var webContents = db.WebContents.Where(x => x.WebModuleID == webModuleId && x.Status == (int)Status.Public);
            results.AddRange(webContents);

            var childWebModules = db.WebModules.Where(x => x.ParentID == webModuleId);

            foreach (var childWebModule in childWebModules)
            {
                GetListContents(childWebModule.ID, results);
            }

            return results;
        }
        [ChildActionOnly]
        public ActionResult _Highlight(int take)
        {

            return PartialView();
        }

        public ActionResult EditHighlight()
        {
            return View();
        }

        public ActionResult _Highlight_Read()
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_Highlight");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var lstContent = new List<WebContent>();
            var jarray = new JArray();
            foreach (var item in data)
            {
                var obj = db.WebContents.Where(x => x.ID == item && x.Status == (int)Status.Public).FirstOrDefault();
                if (obj != null)
                {
                    lstContent.Add(obj);
                }
            }

            foreach (var c in lstContent)
            {
                var item = c;//.TranslateTo(ApplicationService.Culture);
                var moduletran = item.WebModule;//.TranslateTo(ApplicationService.Culture);
                var jobjectcontent = new JObject() { new JProperty("Lang", ApplicationService.TwoLetterISOLanguage), new JProperty("ID", item.ID), new JProperty("Title", item.Title), new JProperty("MetaTitle", item.MetaTitle), new JProperty("WebModuleID", item.WebModuleID), new JProperty("WebModule_MetaTitle", moduletran.MetaTitle) };
                jarray.Add(jobjectcontent);
            }

            return Json(new { JsonArray = jarray.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Highlight_Read([DataSourceRequest] DataSourceRequest request)
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_Highlight");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var result = new List<WebContent>();
            foreach (var item in data)
            {
                var obj = db.WebContents.Where(x => x.ID == item && x.Status == (int)Status.Public).FirstOrDefault();
                if (obj != null)
                {
                    result.Add(obj);
                }
            }
            return Json(result.Select(x => new { x.ID, x.Title, x.Description, x.Image, x.ModifiedBy, x.ModifiedDate, x.MetaTitle }).ToDataSourceResult(request));
        }
        public ActionResult AllHighlight_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<WebContent> content = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals("sanpham")).FirstOrDefault();

            var items = GetListContents(module.ID, content).Where(x => x.Status == (int)Status.Public)
                .Select(x => new { x.ID, x.Title, x.Description, x.Image, x.ModifiedBy, x.ModifiedDate });
            {
                request.Sorts.Add(new SortDescriptor("ID", System.ComponentModel.ListSortDirection.Descending));
            }

            return Json(items.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult Highlight_Update(int id, int order)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                var ids = ConfigurationManager.AppSettings.Get("Home_Highlight");
                var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
                data.Remove(id);
                if (order < 1) order = 1;
                else if (order > data.Count + 1) order = data.Count + 1;
                data.Insert(order - 1, id);
                data = data.Distinct().ToList();
                config.AppSettings.Settings.Remove("Home_Highlight");
                config.AppSettings.Settings.Add("Home_Highlight", string.Join(",", data));
                config.Save();
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = true,
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult Highlight_Delete(int id)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                var ids = ConfigurationManager.AppSettings.Get("Home_Highlight");
                var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
                data.Remove(id);
                data = data.Distinct().ToList();
                config.AppSettings.Settings.Remove("Home_Highlight");
                config.AppSettings.Settings.Add("Home_Highlight", string.Join(",", data));
                config.Save();
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = true,
                    error = ex.Message
                });
            }
        }


        //-----------------------------------------
        [ChildActionOnly]
        public ActionResult _ModuleHome()
        {

            return PartialView();
        }
        public ActionResult EditModuleHome()
        {
            return View();
        }
        public ActionResult _ModuleHome_Read(int take)
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_ModuleHome");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var modules = new List<WebModuleTree>();
            var jarray = new JArray();
            foreach (var item in data)
            {
                modules.Add(WebModuleStore.WebModuleInline().Single(x => x.ID == item));
            }
            foreach (var m in modules)
            {
                var jobject = new JObject() { new JProperty("Lang", ApplicationService.TwoLetterISOLanguage), new JProperty("ID", m.ID), new JProperty("Title", m.Title), new JProperty("MetaTitle", m.MetaTitle) };

                var jarraycontent = new JArray();
                var contents = (from x in db.WebContents
                                where x.WebModuleID != null && m.ID == x.WebModuleID
                                orderby x.ID descending
                                select x).Skip(0).Take(take);
                foreach (var c in contents)
                {
                    var item = c;//.TranslateTo(ApplicationService.Culture);
                    var moduletran = item.WebModule;//.TranslateTo(ApplicationService.Culture);
                    var jobjectcontent = new JObject() { new JProperty("Lang", ApplicationService.TwoLetterISOLanguage), new JProperty("ID", item.ID), new JProperty("Title", item.Title), new JProperty("MetaTitle", item.MetaTitle), new JProperty("WebModuleID", item.WebModuleID), new JProperty("WebModule_MetaTitle", moduletran.MetaTitle) };
                    jarraycontent.Add(jobjectcontent);
                }
                jobject.Add(new JProperty("JContent", jarraycontent));

                jarray.Add(jobject);
            }


            return Json(new { JsonArray = jarray.ToString() }, JsonRequestBehavior.AllowGet); //Json(data2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModuleHome_Read([DataSourceRequest] DataSourceRequest request)
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_ModuleHome");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var result = new List<WebModuleTree>();
            foreach (var item in data)
            {
                result.Add(WebModuleStore.WebModuleInline().Single(x => x.ID == item));
            }
            return Json(result.ToDataSourceResult(request));
        }
        public ActionResult AllModuleHome_Read([DataSourceRequest] DataSourceRequest request)
        {
            var type = ConfigurationManager.AppSettings.Get("Home_ModuleHome_Type");
            var items = WebModuleStore.WebModuleInline().Where(x => type != null && type.Length > 0 && type.ToLower().Split(',').Contains(x.ContentTypeID.ToLower()));
            return Json(items.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult ModuleHome_Update(int id, int order)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                var ids = ConfigurationManager.AppSettings.Get("Home_ModuleHome");
                var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
                data.Remove(id);
                if (order < 1) order = 1;
                else if (order > data.Count + 1) order = data.Count + 1;
                data.Insert(order - 1, id);
                data = data.Distinct().ToList();
                config.AppSettings.Settings.Remove("Home_ModuleHome");
                config.AppSettings.Settings.Add("Home_ModuleHome", string.Join(",", data));
                config.Save();
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = true,
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult ModuleHome_Delete(int id)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                var ids = ConfigurationManager.AppSettings.Get("Home_ModuleHome");
                var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
                data.Remove(id);
                data = data.Distinct().ToList();
                config.AppSettings.Settings.Remove("Home_ModuleHome");
                config.AppSettings.Settings.Add("Home_ModuleHome", string.Join(",", data));
                config.Save();
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = true,
                    error = ex.Message
                });
            }
        }


        //-----------------------------------------

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _PubIndex(int id)
        {
            WebModule webmodule = null;
            if (TempData["WebModule"] != null)
            {
                webmodule = TempData["WebModule"] as WebModule;
            }
            else webmodule = db.Set<WebModule>().Find(id);
            ViewBag.WebModule = webmodule;
            ViewBag.ID = id;

            var lstDestination1 = db.WebModules.Where(x => x.ParentID == 2 && x.Status == (int)Status.Public).ToList();
            var lstDestination2 = db.WebModules.Where(x => x.ParentID == 3 && x.Status == (int)Status.Public).ToList();

            lstDestination1.AddRange(lstDestination2);

            ViewBag.Destination = lstDestination1;

            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult _PubHighlight()
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_Highlight");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var result = new List<WebContent>();
            foreach (var item in data)
            {
                var obj = db.WebContents.Find(item);
                if (obj != null)
                {
                    result.Add(obj);
                }
            }

            return PartialView(result);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        [OutputCache(Duration = 60)]
        public ActionResult _PubModuleHome()
        {

            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult _PubModuleHome_Read(int take)
        {
            var ids = ConfigurationManager.AppSettings.Get("Home_ModuleHome");
            var data = ids.Split(',').ToList().ConvertStringToInt().ToList();
            var modules = new List<WebModuleTree>();
            var jarray = new JArray();

            foreach (var d in data)
            {
                var module = db.WebModules.Find(d);
                if (module != null)
                {
                    var m = module;// 
                    var jobject = new JObject() { new JProperty("Lang", ApplicationService.TwoLetterISOLanguage), new JProperty("ID", m.ID), new JProperty("Title", m.Title), new JProperty("MetaTitle", m.MetaTitle) };

                    var jarraycontent = new JArray();
                    var contents = (from x in db.WebContents
                                    where x.WebModuleID != null && m.ID == x.WebModuleID
                                    orderby x.ID descending
                                    select x).Skip(0).Take(take);
                    foreach (var c in contents)
                    {
                        var item = c;//.TranslateTo(ApplicationService.Culture);
                        var moduletran = item.WebModule;//.TranslateTo(ApplicationService.Culture);
                        var jobjectcontent = new JObject() { new JProperty("Lang", ApplicationService.TwoLetterISOLanguage), new JProperty("ID", item.ID), new JProperty("Title", item.Title), new JProperty("Description", item.Description), new JProperty("Image", item.Image), new JProperty("MetaTitle", item.MetaTitle), new JProperty("CreatedDate", item.CreatedDate.ToString()), new JProperty("WebModuleID", item.WebModuleID), new JProperty("WebModule_MetaTitle", moduletran.MetaTitle) };
                        jarraycontent.Add(jobjectcontent);
                    }
                    jobject.Add(new JProperty("JContent", jarraycontent));

                    jarray.Add(jobject);
                }
            }


            return Json(new { JsonArray = jarray.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _PubSlide()
        {

            IList<WebSlide> slides = this.db.WebSlides.Where(m => m.Status == (int)Status.Public).OrderBy(m => m.Order).ToList();

            return PartialView(slides);
        }

        [AllowAnonymous]
        public ActionResult _GetLetter()
        {
            return PartialView();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult _GetLetter(string email)
        {
            try
            {
                if (!Regex.IsMatch(email, @"[\w\d._%+-]+@[\w\d.-]+\.\w{2,4}"))
                {
                    throw new Exception(WebResources.EmailNotValid);
                }

                //if (!this.db.SubscribeNotices.Where(s => s.Email == email).Any())
                //{
                //    this.db.SubscribeNotices.Add(new SubscribeNotice()
                //    {
                //        CreatedBy = WebSecurity.CurrentUserName,
                //        ModifiedBy = WebSecurity.CurrentUserName,
                //        Email = email
                //    });
                //}

                this.db.SaveChanges();

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxNewsByWebModuleUID(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(3).ToList();
            }
            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxNghienCuu(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(2).ToList();
            }
            return PartialView(webContent);
        }
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxVideo(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(1).ToList();
            }
            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxThuVienAnh(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(5).ToList();
            }
            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxSuKienHoatDong(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(5).ToList();
            }
            return PartialView(webContent);
        }
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxTopDocNhieu()
        {
            var webContent = db.WebContents.Where(x => x.WebModule.ContentTypeID.Equals("Article") && x.Status == (int)Status.Public).OrderByDescending(x => x.CountView).Take(5);

            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _TieuDeHot(int take)
        {
            var webContent = db.WebContents.Where(x => x.WebModule.ContentTypeID.Equals("Article") && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(take);

            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _BoxAnPhamXuatBan()
        {
            var module = db.WebModules.Where(x => x.UID.Equals("an-pham-xuat-ban")).FirstOrDefault();
            ViewBag.WebModule = module;
            List<WebContent> webContent = new List<WebContent>();
            GetListContents(module.ID, webContent);
            return PartialView(webContent);
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult _SlideSuKienHoatDong(string uid)
        {
            List<WebContent> webContent = new List<WebContent>();

            var module = db.WebModules.Where(x => x.UID.Equals(uid)).FirstOrDefault();

            if (module != null)
            {
                ViewBag.WebModule = module;
                webContent = db.WebContents.Where(x => x.WebModuleID == module.ID && x.Status == (int)Status.Public).OrderByDescending(x => x.PublishDate).Take(5).ToList();
            }
            return PartialView(webContent);
        }
    }
}
