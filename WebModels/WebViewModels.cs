﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Web;
using System.Configuration;
using Kendo.Mvc.UI;
using System.Web.Mvc;
namespace WebModels
{
    public class SelectListTemp
    {
        public string Text { get; set; }
        public int? Value { get; set; }
    }
    public class PlayerDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }
    public class MatchDto
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; } 
        public double? Point { get; set; }
        public string Image { get; set; }
        public int? Player1 { get; set; }
        public int? Player2 { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }
        public string NamePlayer1 { get; set; }
        public string NamePlayer2 { get; set; }
    }

    public class ClubDto
    {
        public int ID { get; set; }
        public string ClubName { get; set; }
        public double? Amount { get; set; }
        public int? Type { get; set; }
        public string QRCode { get; set; }
        public string ShareLink { get; set; }
        public string FacebookRef { get; set; }
        public string TypeName { get; set; }

        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class WebModuleTree
     {
         public int ID { get; set; }
         public string Title { get; set; }
         public string MetaTitle { get; set; }
         public string Description { get; set; }
         public int? Order { get; set; }
         public int? ParentID { get; set; }
         public string ContentTypeID { get; set; } 
     }
    public class WebCategoryNode
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string MetaTitle { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }
        public int? ParentID { get; set; }
    }
    //public partial class WebContext
    // {
    //    public IEnumerable<WebModuleTree> WebModuleInline()
    //     {
    //         var items = HttpContext.Current.Cache.Data("ListWebModuleInlineTree", 60, () =>
    //         {

                 
                                    
    //                 var webmodules = from e in this.WebModules.AsNoTracking() orderby e.Order where e.ParentID == null select e;
    //                 List<WebModuleTree> inlineDefault = new List<WebModuleTree> { };
    //                 foreach (var item in webmodules)
    //                 {
    //                     var model = new WebModuleTree();
    //                     model.Title = item.Title; model.ID = item.ID; model.Order = item.Order; model.ParentID = item.ParentID; model.ContentTypeID = item.ContentTypeID;
    //                     model.MetaTitle = item.MetaTitle; model.Description = item.Description;
    //                     inlineDefault.Add(model);
    //                     var subs = item.SubWebModules.Where(x => x.ShowOnAdmin);
    //                     if (subs.Any())
    //                     {
    //                         WebModuleSubInline(ref inlineDefault, subs.OrderBy(x => x.Order), model);
    //                     }
    //                 }
    //                 return inlineDefault;
                  
    //         });

    //         return items;

    //     }
    //    public void WebModuleSubInline(ref  List<WebModuleTree> inlineDefault, IEnumerable<WebModule> data, WebModuleTree parent)
    //     {

    //         foreach (var item in data)
    //         {
    //             var model = new WebModuleTree();
    //             model.Title = parent.Title + " >> " + item.Title; model.ID = item.ID; model.Order = item.Order; model.ParentID = item.ParentID; model.ContentTypeID = item.ContentTypeID;
    //             model.MetaTitle = item.MetaTitle; model.Description = item.Description; 
    //             inlineDefault.Add(model);
    //             var subs = item.SubWebModules.Where(x => x.ShowOnAdmin);
    //             if (subs.Any())  
    //             {
    //                 WebModuleSubInline(ref inlineDefault, subs.OrderBy(x => x.Order), model);
    //             }
    //         }
    //     }



    //    public List<TreeViewItemModel> WebModuleTreeLeft()
    //    {
    //        UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
    //        List<TreeViewItemModel> inlineDefault = new List<TreeViewItemModel>();
    //        var items = HttpContext.Current.Cache.Data("WebModuleTreeLeft", 180, () =>
    //        {

    //            var webmodules = from e in WebModules.AsNoTracking() orderby e.Order where e.ParentID == null && e.ShowOnAdmin select e;
    //            foreach (var item in webmodules)
    //            {
    //                var model = new TreeViewItemModel() { Text = item.Title, Id = item.ID.ToString(), Expanded = true, Url = helper.Action("Index", "WebContent", new { id = item.ID, area = "admin" }) };
    //                var subs = item.SubWebModules.Where(x => x.ShowOnAdmin);
    //                if (subs.Any())
    //                {
    //                    WebModuleTreeLeftSub(ref model, subs.OrderBy(x => x.Order));
    //                }
    //                inlineDefault.Add(model);
    //            }
    //            return inlineDefault;
    //        });

    //        return items;

    //    }
    //    public void WebModuleTreeLeftSub(ref TreeViewItemModel treeitem, IEnumerable<WebModule> data)
    //    {
    //        UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
    //        foreach (var item in data)
    //        {
    //            var model = new TreeViewItemModel() { Text = item.Title, Expanded = true, Id = item.ID.ToString(), Url = helper.Action("Index", "WebContent", new { id = item.ID, area = "admin" }) };
    //            var subs = item.SubWebModules.Where(x => x.ShowOnAdmin);
    //            if (subs.Any())
    //            {
    //                WebModuleTreeLeftSub(ref model, subs.OrderBy(x => x.Order));
    //            }    
    //            treeitem.Items.Add(model);
    //        }
    //    }

         
    // }
}
