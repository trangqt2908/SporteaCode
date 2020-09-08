using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebModels;
using Common;
using WEB.Models;

namespace WEB.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ClubController : BaseController
    {
        private WebContext db = new WebContext();

        //
        // GET: /Admin/Club/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Club_Read([DataSourceRequest] DataSourceRequest request)
        {
            var contents = db.Clubs.Select(x => new ClubDto
            {
                ID = x.ID,
                ClubName = x.ClubName,
                CreatedDate = x.CreatedDate,
                Amount = x.Amount,
                Type = x.Type
            }).ToList();

            foreach (var item in contents)
            {
                if (item.Type == (int)TypeClub.Tennis)
                {
                    item.TypeName = TypeClub.Tennis.ToString();
                }
                else
                {
                    item.TypeName = TypeClub.Badminton.ToString();
                }
            }
            return Json(contents.ToDataSourceResult(request));
        }

        public JsonResult GetClubs()
        {
            var Clubs = db.Clubs.Select(x => new { x.ID, x.ClubName }).ToList();
            return Json(Clubs, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Admin/Club/Create

        public ActionResult Add()
        {
            var model = new Club();

            return View(model);
        }

        // POST: /Admin/Club/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(Club model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                db.Clubs.Add(model);
                db.SaveChanges();
                ViewBag.StartupScript = "create_success();";
                return View(model);
            }
            return View(model);
        }

        //
        // GET: /Admin/Club/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Club model = db.Clubs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // POST: /Admin/Club/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Club input)
        {
            if (ModelState.IsValid)
            {
                var model = db.Clubs.Find(input.ID);
                model.ClubName = input.ClubName;
                model.Amount = input.Amount;
                model.Type = input.Type;
                model.QRCode = input.QRCode;
                model.ShareLink = input.ShareLink;
                model.FacebookRef = input.FacebookRef;

                db.SaveChanges();
                ViewBag.StartupScript = "edit_success();";
                return View(model);
            }
            return View(input);
        }
        public ActionResult Delete(int id)
        {
            var role = db.Set<Club>().Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View("Delete", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Club model)
        {

            try
            {

                var role = db.Clubs.Attach(model);
                db.Set<Club>().Remove(role);
                db.SaveChanges();
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
            var lstSiteID = new List<int>();
            foreach (var obj in objects)
            {
                try
                {
                    lstSiteID.Add(int.Parse(obj.ToString()));
                }
                catch (Exception)
                { }
            }

            var temp = from p in db.Set<Club>()
                       where lstSiteID.Contains(p.ID)
                       select p;

            return View(temp.ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletes(List<Club> model)
        {
            var temp = new List<Club>();
            foreach (var item in model)
            {
                try
                {

                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();


                }
                catch (Exception)
                {
                    db.Entry(item).State = EntityState.Unchanged;
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
                ViewBag.StartupScript = "top.$('#grid').data('kendoGrid').dataSource.read(); top.treeview.dataSource.read();";
                ModelState.AddModelError("", Resources.Common.ErrorDeleteItems);
                return View(temp);
            }
            else
            {
                ViewBag.StartupScript = "deletes_success();";
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
