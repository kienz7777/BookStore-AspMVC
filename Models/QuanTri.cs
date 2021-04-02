namespace BookStoreASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuanTri")]
    public partial class QuanTri
    {
        [Key]
        public int MaQuanTri { get; set; }

        [StringLength(30)]
        public string TaiKhoan { get; set; }

        [StringLength(10)]
        public string MatKhau { get; set; }
    }
}
