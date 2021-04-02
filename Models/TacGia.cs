namespace BookStoreASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TacGia")]
    public partial class TacGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TacGia()
        {
            SoHuus = new HashSet<SoHuu>();
        }

        [Key]
        public int MaTacGia { get; set; }

        [StringLength(50)]
        public string TenTacGia { get; set; }

        [StringLength(50)]
        public string DiaChi { get; set; }

        public string TieuSu { get; set; }

        [StringLength(11)]
        public string SDT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SoHuu> SoHuus { get; set; }
    }
}
