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
    public class MatchController : BaseController
    {
        private WebContext db = new WebContext();

        //
        // GET: /Admin/Match/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Match_Read([DataSourceRequest] DataSourceRequest request)
        {
            var contents = (from m in db.Matchs
                            join u1 in db.UserProfiles on m.Player1 equals u1.UserId
                            join u2 in db.UserProfiles on m.Player2 equals u2.UserId
                           select new MatchDto
                           {
                               ID = m.ID,
                               Date = m.Date,
                               Point = m.Point,
                               NamePlayer1 = u1.FullName,
                               NamePlayer2 = u2.FullName,
                               Player1Score = m.Player1Score,
                               Player2Score = m.Player2Score,
                           });

            return Json(contents.ToDataSourceResult(request));
        }

        //
        // GET: /Admin/Match/Create

        public ActionResult Add()
        {
            var model = new Match();

            return View(model);
        }

        // POST: /Admin/Match/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Add(Match model)
        {
            if (ModelState.IsValid)
            {
                model.Date = DateTime.Now;
                db.Matchs.Add(model);
                db.SaveChanges();
                ViewBag.StartupScript = "create_success();";
                return View(model);
            }
            return View(model);
        }

        //
        // GET: /Admin/Match/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Match model = db.Matchs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // POST: /Admin/Match/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Match input)
        {
            var model = db.Matchs.Find(input.ID);
            model.Date = input.Date;
            model.Point = input.Point;
            model.Image = input.Image;
            model.Player1 = input.Player1;
            model.Player2 = input.Player2;
            model.Player1Score = input.Player1Score;
            model.Player2Score = input.Player2Score;

            db.SaveChanges();
            ViewBag.StartupScript = "edit_success();";
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var role = db.Set<Match>().Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View("Delete", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Match model)
        {

            try
            {

                var role = db.Matchs.Attach(model);
                db.Set<Match>().Remove(role);
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

            var temp = from p in db.Set<Match>()
                       where lstSiteID.Contains(p.ID)
                       select p;

            return View(temp.ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletes(List<Match> model)
        {
            var temp = new List<Match>();
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
