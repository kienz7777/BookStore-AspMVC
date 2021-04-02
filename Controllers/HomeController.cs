using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;
using BookStoreASP.Models;

namespace BookStoreASP.Controllers
{
    public class HomeController : Controller
    {
        WEBBANSACH db = new WEBBANSACH();
        public ActionResult Index()
        {
            //truy vấn từ csdl
            var lstSachMoi = db.Saches.Take(8).ToList();
            return View(lstSachMoi);

        }

        public ViewResult XemChiTiet(int MaSach = 0)
        {
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == MaSach);
            if (sach == null)
            {
                //Trả về trang báo lỗi 
                Response.StatusCode = 404;
                return null;
            }
            ChuDe chuDe = db.ChuDes.Single(n => n.MaChuDe == sach.MaChuDe);
            ViewBag.TenChuDe = chuDe.TenChuDe;
            //ChuDe cd = db.ChuDes.Single(n => n.MaChuDe == sach.MaChuDe);
            //ViewBag.TenCD = cd.TenChuDe;
            //ViewBag.TenChuDe = db.ChuDes.Single(n => n.MaChuDe == sach.MaChuDe).TenChuDe;
            NXB nXB = db.NXBs.Single(n => n.MaNXB == sach.MaNXB);
            ViewBag.TenNhaXuatBan = nXB.TenNXB;

            var list = (from s in db.SoHuus
                        where s.MaSach == MaSach
                        select s).ToList();

            foreach (var item in list)
            {
                ViewBag.TenTacGia = db.TacGias.Single(n => n.MaTacGia == item.MaTacGia).TenTacGia;
            }

            return View(sach);
        }

    }
}