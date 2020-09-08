using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using WebMatrix.WebData;

namespace WebModels
{
    [Table("UploadFiles")]
    public partial class UploadFile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(ResourceType = typeof(WebResources), Name = "ID")]
        public int ID { get; set; }

        public string Title { get; set; }
        public string Link { get; set; }
    }
    [Table("WebContentUploads")]
    public class WebContentUpload
    {
        public WebContentUpload()
        {
            this.SubWebContentUploads = new HashSet<WebContentUpload>();

        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string MetaTitle { get; set; }
        public string FullPath { get; set; }
        public string UID { get; set; }
        //   public int RefID { get; set; }
        public int? FolderID { get; set; }
        public string MimeType { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "CreatedBy")]
        public string CreatedBy { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        // public string Module { get; set; }

        [ForeignKey("FolderID")]
        public virtual ICollection<WebContentUpload> SubWebContentUploads { get; set; }
    }
    public partial class WebSlide : BaseEntity
    {
        public WebSlide()
        {


        }
        [Required(ErrorMessageResourceType = typeof(WebResources), ErrorMessageResourceName = "RequiredTitle")]
        [Display(ResourceType = typeof(WebResources), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Link")]
        public string Link { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Image")]
        public string Image { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Target")]
        public string Target { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Order")]
        public int? Order { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Status")]
        public int Status { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "ModifiedBy")]
        public string ModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(WebResources), Name = "ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }


        public string Culture { get; set; }
    }
  
    [Table("WebSimpleContents")]
    public partial class WebSimpleContent : BaseEntity
    {
        public WebSimpleContent()
        {

        }
        [Required(ErrorMessageResourceType = typeof(WebResources), ErrorMessageResourceName = "RequiredTitle")]
        [Display(ResourceType = typeof(WebResources), Name = "Title")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Image")]
        public string Image { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "Link")]
        public string Link { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Key")]
        public string Key { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "Body")]
        public string Body { get; set; }

        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(WebResources), Name = "CreatedBy")]
        public string CreatedBy { get; set; }
        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(WebResources), Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(WebResources), Name = "ModifiedBy")]
        public string ModifiedBy { get; set; }
        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(WebResources), Name = "ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [Display(ResourceType = typeof(WebResources), Name = "MetaTitle")]
        public string MetaTitle { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaKeywords")]
        public string MetaKeywords { get; set; }
        [Display(ResourceType = typeof(WebResources), Name = "MetaDescription")]
        public string MetaDescription { get; set; }

        public string Culture { get; set; }
    }
}
