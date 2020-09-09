using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using WebMatrix.WebData;

namespace WebModels
{
    public partial class WebModule : BaseWebContent
    {
        public WebModule()
        {
            this.AccessWebModules = new HashSet<AccessWebModule>();
            this.AccessWebModuleRoles = new HashSet<AccessWebModuleRole>();
            this.WebContents = new HashSet<WebContent>();
            this.WebContacts = new HashSet<WebContact>();
            this.ModuleNavigations = new HashSet<ModuleNavigation>();
        }
        public virtual ICollection<ModuleNavigation> ModuleNavigations { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Body")]
        public string Body { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Image")]
        public string Image { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Icon")]
        public string Icon { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "CodeColor")]
        public string CodeColor { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "ParentID")]
        public int? ParentID { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "ContentTypeID")]
        public string ContentTypeID { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "URL")]
        public string URL { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Target")]
        public string Target { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "SeoTitle")]
        public string SeoTitle { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "MetaTitle")]
        public string MetaTitle { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaKeywords")]
        public string MetaKeywords { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaDescription")]
        public string MetaDescription { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Order")]
        public int? Order { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "UID")]
        public string UID { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "IndexLayout")]
        public string IndexLayout { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "IndexView")]
        public string IndexView { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "PublishIndexView")]
        public string PublishIndexView { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "PublishDetailView")]
        public string PublishDetailView { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "PublishIndexLayout")]
        public string PublishIndexLayout { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "PublishDetailLayout")]
        public string PublishDetailLayout { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Status")]
        public int? Status { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "SubQuerys")]
        public string SubQuerys { get; set; }
        public string ActiveArticle { get; set; }

        public virtual ICollection<AccessWebModule> AccessWebModules { get; set; }
        public virtual ICollection<AccessWebModuleRole> AccessWebModuleRoles { get; set; }
        public virtual ContentType ContentType { get; set; }
        public virtual ICollection<WebContent> WebContents { get; set; }
        [ForeignKey("ParentID")]
        public virtual ICollection<WebModule> SubWebModules { get; set; }
        public virtual ICollection<WebContact> WebContacts { get; set; }
        public virtual WebModule Parent { get; set; }

        public Boolean ShowOnMenu { get; set; }
        public Boolean ShowOnAdmin { get; set; }
        public string Culture { get; set; }

    }
    [Table("ContentRelateds")]
    public partial class ContentRelated
    {
        [Key]
        [ForeignKey("MainContent")]
        [Column(Order = 1)]
        [Display(ResourceType = typeof(WebResources), Name = "ID")]
        public int MainID { get; set; }
        [Key]
        [ForeignKey("RelateContent")]
        [Column(Order = 2)]
        [Display(ResourceType = typeof(WebResources), Name = "ID")]
        public int RelatedID { get; set; }
        [DataType(DataType.Text)]
        public string Type { get; set; }
        [DataType(DataType.Text)]
        public int Order { get; set; }
        public virtual WebContent MainContent { get; set; }
        public virtual WebContent RelateContent { get; set; }
    }
    public partial class WebContent : BaseWebContent
    {
        public WebContent()
        {
            //if (WebContentCategories == null)
            // this.WebContentCategories = new HashSet<WebContentCategory>();
            //if(ProductInfo==null)     this.ProductInfo = new ProductInfo();
        }
        public string UID { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Image")]
        public string Image { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Body")]
        public string Body { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Link")]
        public string Link { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "MetaTitle")]
        public string MetaTitle { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaKeywords")]
        public string MetaKeywords { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaDescription")]
        public string MetaDescription { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Status")]
        public Nullable<int> Status { get; set; }
        public string StatusText
        {
            get
            {
                if (Status == null)
                {
                    return string.Empty;
                }
                if (Status.Value == (int)StatusEnum.Internal)
                {
                    return "Chờ duyệt";
                }
                if (Status.Value == (int)StatusEnum.Public)
                {
                    return "Đã phát hành";
                }

                return string.Empty;
            }
        }
        [Display(ResourceType = typeof(WebResources), Name = "Order")]
        public Nullable<int> Order { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "LinkVideo")]
        public string LinkVideo { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Event")]
        public DateTime? Event { get; set; }

        public string SeoTitle { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "WebModuleID")]
        public Nullable<int> WebModuleID { get; set; }
        public decimal? CountView { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "PublishDate")]
        public DateTime? PublishDate { get; set; }
        public virtual WebModule WebModule { get; set; }
        public virtual ICollection<ContentImage> ContentImages { get; set; }
    }
    public enum StatusEnum { Private = -1, Internal = 0, Public = 1 }


    [Table("WebContacts")]
    public partial class WebContact : BaseEntity
    {
        [Required(ErrorMessageResourceType = typeof(WebResources), ErrorMessageResourceName = "RequiredTitle")]
        [Display(ResourceType = typeof(WebResources), Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [Display(ResourceType = typeof(WebResources), Name = "Body")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(ResourceType = typeof(WebResources), Name = "FullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessageResourceType = typeof(WebResources), ErrorMessageResourceName = "EmailNotValid")]
        [Display(ResourceType = typeof(WebResources), Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số điện thoại")]
        [Display(ResourceType = typeof(WebResources), Name = "Mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(ResourceType = typeof(WebResources), Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại đơn hàng")]
        public int LoaiDonHang { get; set; }

        [NotMapped]
        public string LoaiDonHangText
        {
            get
            {
                return LoaiDonHang == 1 ? "Điện tử" : "Giấy";
            }
        }
        public int LoaiLienHe { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }



    [Table("ContentImages")]
    public class ContentImage : BaseEntity
    {
        [Display(ResourceType = typeof(WebResources), Name = "Title")]
        public string Title { get; set; }

        public string Image { get; set; }

        [DefaultValue(0)]
        public int Order { get; set; }

        public int WebContentID { get; set; }
        public virtual WebContent WebContent { get; set; }
    }


    [Table("ModuleNavigations")]
    public class ModuleNavigation
    {
        [Key]
        [Column(Order = 0)]
        [Display(ResourceType = typeof(WebResources), Name = "WebModuleID")]
        public int WebModuleID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NavigationID { get; set; }
        public virtual Navigation Navigation { get; set; }
        public virtual WebModule WebModule { get; set; }
    }

    [Table("Navigations")]
    public class Navigation : BaseEntity
    {
        public Navigation()
        {
            this.ModuleNavigations = new HashSet<ModuleNavigation>();
        }
        public virtual ICollection<ModuleNavigation> ModuleNavigations { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Key")]
        public string Key { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebResources), ErrorMessageResourceName = "RequiredTitle")]
        [Display(ResourceType = typeof(WebResources), Name = "Title")]
        public string Title { get; set; }

        [DefaultValue(0)]
        public int Order { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Description")]
        public string Description { get; set; }
    }

    [Table("Club")]
    public class Club : BaseEntity
    {
        public string ClubName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double? Amount { get; set; }
        public int? Type { get; set; }
        public string QRCode { get; set; }
        public string ShareLink { get; set; }
        public string FacebookRef { get; set; }
    }

    [Table("ClubConfiguration")]
    public class ClubConfiguration : BaseEntity
    {
        public string ConfigName { get; set; }
        public string ConfigValue { get; set; }
        public int? ClubId { get; set; }
    }
    [Table("ClubPlayerPayment")]
    public class ClubPlayerPayment : BaseEntity
    {
        public int? ClubId { get; set; }
        public int? PlayerId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? Amount { get; set; }
    }
    [Table("ClubPurchase")]
    public class ClubPurchase : BaseEntity
    {
        public int? ClubId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? Amount { get; set; }
    }
    [Table("ClubSpend")]
    public class ClubSpend : BaseEntity
    {
        public int? ClubId { get; set; }
        public DateTime? SpendDate { get; set; }
        public string SpendNote { get; set; }
        public decimal? Amount { get; set; }
    }
    [Table("Countries")]
    public partial class Country : BaseEntity
    {
        public Country()
        {
            this.Provinces = new List<Province>();
        }
        public string Title { get; set; }
        public string IsoCode { get; set; }
        public virtual ICollection<Province> Provinces { get; set; }
    }


    [Table("Match")]
    public class Match : BaseEntity
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public int? Point { get; set; }
        public string Image { get; set; }
        public int? Player1 { get; set; }
        public int? Player2 { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }
    }
    [Table("Provinces")]
    public partial class Province : BaseEntity
    {
        public string Title { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Area { get; set; }
        public virtual Country Country { get; set; }

        [NotMapped]
        public string CountryName { get; set; }
    }

    [Table("Rights")]
    public class Rights : BaseEntity
    {
        public string RightName { get; set; }
        public string Description { get; set; }
    } 
    [Table("RightsInRoles")]
    public class RightsInRole
    {
        [Key]
        [Column(Order = 0)]
        public int? RightId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int? RoleId { get; set; }
    }
    [Table("TennisDetails")]
    public class TennisDetail : BaseEntity
    {
        public int? MatchId { get; set; }
        public int? SetNumber { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }
    }
}
