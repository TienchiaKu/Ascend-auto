using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DinViewModel
    {
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [StringLength(13)]
        [Display(Name = "DoID")]
        public string? DoID { get; set; }

        //Project_DIDW
        [StringLength(10)]
        [Display(Name = "Din建立時間")]
        public string? DinDate { get; set; }

        [StringLength(1)]
        [Display(Name = "DIn狀態")]
        public string? DinStatus { get; set; }

        //ProjectM
        [StringLength(4)]
        [Display(Name = "專案狀態")]
        public string? ProjectStatus { get; set; }

        [StringLength(8)]
        [Display(Name = "客戶資料庫")]
        public string? Cus_DB { get; set; }

        [StringLength(10)]
        [Display(Name = "客戶代碼")]
        public string? CusID { get; set; }

        [StringLength(20)]
        [Display(Name = "最終客戶名稱")]
        public string? EndCus { get; set; }

        [StringLength(20)]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [StringLength(20)]
        [Display(Name = "產品型號")]
        public string? ProModel { get; set; }

        [Display(Name = "是否整合")]
        public bool IsInte { get; set; }

        [StringLength(6)]
        [Display(Name = "預估量產時間")]
        public string? EProduceYS { get; set; }

        //ProjectD
        [StringLength(10)]
        [Display(Name = "供應商")]
        public string? VendorID { get; set; }

        [StringLength(25)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        [Display(Name = "預估三年銷售額")]
        public double ELTR { get; set; }

        [Display(Name = "預估毛利率")]
        public double EGP { get; set; }

        [Display(Name = "預估三年銷售毛利")]
        public double LTP { get; set; }

        [Display(Name = "預估第一年數量")]
        public int EFirstYQty { get; set; }

        [Display(Name = "預估第二年數量")]
        public int ESecondYQty { get; set; }

        [Display(Name = "預估第三年數量")]
        public int EThirdYQty { get; set; }

        [Display(Name = "第一年單價")]
        public double UFirstYPrice { get; set; }

        [Display(Name = "第二年單價")]
        public double USecondYPrice { get; set; }

        [Display(Name = "第三年單價")]
        public double UThirdYPrice { get; set; }

        //Project_Emp
        [StringLength(5)]
        [Display(Name = "PM")]
        [Required]
        public string? PM_EmpName { get; set; }

        public int PMID { get; set; }

        public double PM_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "Sales")]
        [Required]
        public string? Sales_EmpName { get; set; }

        public int SalesID { get; set; }

        public double Sales_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE1")]
        public string? FAE1_EmpName { get; set; }

        public int FAE1ID { get; set; }

        public double FAE1_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE2")]
        public string? FAE2_EmpName { get; set; }

        public int FAE2ID { get; set; }

        public double FAE2_Bonus { get; set; }

        [StringLength(5)]
        [Display(Name = "FAE3")]
        public string? FAE3_EmpName { get; set; }

        public int FAE3ID { get; set; }

        public double FAE3_Bonus { get; set; }

        //讀取資料用
        [Display(Name = "建立時間")]
        public string? vmCreateDate { get; set; }

        [Display(Name = "DIn狀態")]
        public string? StatusName { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }

    public class DinUploadViewModel
    {
        [Display(Name = "建立時間")]
        public string? CreateDate { get; set; }

        [Display(Name = "Din/Dwin")]
        public string? DIDWType { get; set; }

        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "供應商ID")]
        public string? VendorID { get; set; }

        [Display(Name = "供應商名稱")]
        public string? VendorName { get; set; }

        [Display(Name = "品名料號")]
        public string? PartNo { get; set; }

        [Display(Name = "客戶資料庫")]
        public string? CusDB { get; set; }

        [Display(Name = "客戶ID")]
        public string? CusID { get; set; }

        [Display(Name = "客戶名稱")]
        public string? CusName { get; set; }

        [Display(Name = "最終客戶名稱")]
        public string? EndCusName { get; set; }

        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [Display(Name = "產品型號")]
        public string? ProModel { get; set; }

        [Display(Name = "預估量產(年)")]
        public string? EProduceYear { get; set; }

        [Display(Name = "預估量產(季)")]
        public string? EProduceSeason { get; set; } 

        [Display(Name = "預估第一年數量")]
        public int EFirstYQty { get; set; }

        [Display(Name = "預估第二年數量")]
        public int ESecondYQty { get; set; }

        [Display(Name = "預估第三年數量")]
        public int EThirdYQty { get; set; }

        [Display(Name = "第一年單價")]
        public double UFirstYPrice { get; set; }

        [Display(Name = "第二年單價")]
        public double USecondYPrice { get; set; }

        [Display(Name = "第三年單價")]
        public double UThirdYPrice { get; set; }

        [Display(Name = "預估三年銷售額")]
        public double ELTR { get; set; }

        [Display(Name = "預估毛利率")]
        public double EGP { get; set; }

        [Display(Name = "整合?")]
        public bool IsInte { get; set; }

        [Display(Name = "PMID")]
        public int PMID { get; set; }

        [Display(Name = "PM名字")]
        public string? PM_EmpName { get; set; }

        [Display(Name = "PM獎金%")]
        public double PM_Bonus { get; set; }

        [Display(Name = "SalesID")]
        public int SalesID { get; set; }

        [Display(Name = "Sales名字")]
        public string? Sales_EmpName { get; set; }

        [Display(Name = "Sales獎金%")]
        public double Sales_Bonus { get; set; }

        [Display(Name = "FAE1ID")]
        public int FAE1ID { get; set; }

        [Display(Name = "FAE1名字")]
        public string? FAE1_EmpName { get; set; }

        [Display(Name = "FAE1獎金%")] 
        public double FAE1_Bonus { get; set; }

        [Display(Name = "FAE2ID")]
        public int FAE2ID { get; set; }

        [Display(Name = "FAE2名字")]
        public string? FAE2_EmpName { get; set; }

        [Display(Name = "FAE2獎金%")]
        public double FAE2_Bonus { get; set; }

        [Display(Name = "FAE3ID")]
        public int FAE3ID { get; set; }

        [Display(Name = "FAE3名字")]
        public string? FAE3_EmpName { get; set; }

        [Display(Name = "FAE3獎金%")]
        public double FAE3_Bonus { get; set; }

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }

    public class DinIndexViewModel
    {
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "Din建立時間")]
        public string? DinDate { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "產品代號")]
        public string? PartNo { get; set; }

        [Display(Name = "Din狀態")]
        public string? DinStatus { get; set; }
    }

    public class DinCreateViewModel
    {
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        [Display(Name = "Din建立時間")]
        public string? DinDate { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [StringLength(40)]
        [Display(Name = "最終客戶名稱")]
        public string? EndCus { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [StringLength(50)]
        [Display(Name = "料號")]
        public string? PartNo { get; set; }

        [StringLength(40)]
        [Display(Name = "產品應用")]
        public string? ProApp { get; set; }

        [StringLength(40)]
        [Display(Name = "產品型號")]
        public string? ProModel { get; set; }

        [StringLength(6)]
        [Display(Name = "預估量產時間")]
        public string? EProduceYS { get; set; }

        [Display(Name = "預估三年銷售額")]
        public double ELTR { get; set; }

        [Display(Name = "預估毛利率")]
        public double EGP { get; set; }

        [Display(Name = "預估三年銷售毛利")]
        public double LTP { get; set; }

        [Display(Name = "預估第一年數量")]
        public int EFirstYQty { get; set; }

        [Display(Name = "預估第二年數量")]
        public int ESecondYQty { get; set; }

        [Display(Name = "預估第三年數量")]
        public int EThirdYQty { get; set; }

        [Display(Name = "第一年單價")]
        public double UFirstYPrice { get; set; }

        [Display(Name = "第二年單價")]
        public double USecondYPrice { get; set; }

        [Display(Name = "第三年單價")]
        public double UThirdYPrice { get; set; }

        [Display(Name = "PM")]
        public string? PM_EmpName { get; set; }

        [Required]
        public int PMID { get; set; }

        [Display(Name = "Sales")]
        public string? Sales_EmpName { get; set; }

        [Required]
        public int SalesID { get; set; }

        [Display(Name = "FAE1")]
        public string? FAE1_EmpName { get; set; }

        [Required]
        public int FAE1ID { get; set; }

        [Display(Name = "FAE2")]
        public string? FAE2_EmpName { get; set; }

        [Required]
        public int FAE2ID { get; set; }

        [Display(Name = "FAE3")]
        public string? FAE3_EmpName { get; set; }

        [Required]
        public int FAE3ID { get; set; }
    }
}
