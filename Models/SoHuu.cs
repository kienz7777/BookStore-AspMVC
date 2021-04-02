namespace BookStoreASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SoHuu")]
    public partial class SoHuu
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaTacGia { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaSach { get; set; }

        [StringLength(50)]
        public string ViTri { get; set; }

        [StringLength(50)]
        public string VaiTro { get; set; }

        public virtual Sach Sach { get; set; }

        public virtual TacGia TacGia { get; set; }
    }
}
