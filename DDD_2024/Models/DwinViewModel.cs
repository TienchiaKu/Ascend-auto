using System.ComponentModel.DataAnnotations;

namespace DDD_2024.Models
{
    public class DwinViewModel
    {
        [Key]
        [StringLength(12)]
        [Display(Name = "專案編號")]
        public string? ProjectID { get; set; }

        //Project_DIDW
        [StringLength(10)]
        [Display(Name = "Dwin建立時間")]
        public string? DwinDate { get; set; }

        [StringLength(1)]
        [Display(Name = "DWin狀態")]
        public string? DwinStatus { get; set; }

        //ProjectM
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

        [Display(Name = "DWin狀態")]
        public string? StatusName { get; set; }

        [Display(Name = "供應商")]
        public string? VendorName { get; set; }

        [Display(Name = "客戶")]
        public string? CusName { get; set; }

        [Display(Name = "上傳狀態")]
        public string? UploadStatus { get; set; }
    }
}
