using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookStoreASP.Models
{
    public class XemChiTietSanPham
    {
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
        public string TenNXB { get; set; }
        public string TenChuDe { get; set; }
        public string TenTacGia { get; set; }
    }
}