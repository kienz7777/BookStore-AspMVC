namespace BookStoreASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sach")]
    public partial class Sach
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sach()
        {
            Muas = new HashSet<Mua>();
            SoHuus = new HashSet<SoHuu>();
        }

        [Key]
        public int MaSach { get; set; }

        [StringLength(50)]
        public string TenSach { get; set; }

        [Column(TypeName = "money")]
        public decimal? GiaBan { get; set; }

        [StringLength(50)]
        public string MoTa { get; set; }

        [StringLength(50)]
        public string AnhBia { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public int? SoLuong { get; set; }

        public int? MaNXB { get; set; }

        public int? MaChuDe { get; set; }

        public virtual ChuDe ChuDe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mua> Muas { get; set; }

        public virtual NXB NXB { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SoHuu> SoHuus { get; set; }
    }
}
