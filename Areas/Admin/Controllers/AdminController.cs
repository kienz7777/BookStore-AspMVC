using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BookStoreASP.Models;
using Newtonsoft.Json;

using System.Net.Mail;
using System.Net;

namespace BookStoreASP.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        WEBBANSACH db = new WEBBANSACH();
        [HttpGet]
        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(FormCollection f)
        {
            string sEmail = f["email"].ToString();
            string sMatKhau = f.Get("password").ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[AdminLogin]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@EmailAdmin", sEmail);
            sqlcomm.Parameters.AddWithValue("@MatkhauAdmin", sMatKhau);
            SqlDataReader sdr = sqlcomm.ExecuteReader();
            if (sdr.Read())
            {
                ViewBag.message = "Đăng Nhập Thành Công";
                return RedirectToAction("XemSanPham", "Admin");
            }

            else
            {
                ViewBag.message = "Đăng Nhập Thất Bại";
            }

            sqlconn.Close();

            return View();
        }


        [HttpGet]
        public ActionResult Manage()
        {
            //List<ChuDe> listchude = db.ChuDes.ToList();
            //SelectList l = new SelectList(listchude, "MaChuDe", "TenChuDe");
            //ViewBag.ChuDe = l;
            ViewBag.MaNXB = new SelectList(db.NXBs.ToList(), "MaNXB", "TenNXB");
            ViewBag.MaChuDe = new SelectList(db.ChuDes.ToList(), "MaChuDe", "TenChuDe");
            return View();
        }
        [HttpPost]
        public ActionResult Manage(FormCollection f2, Sach sach, HttpPostedFileBase fileUpLoad)
        {
            ViewBag.MaNXB = new SelectList(db.NXBs.ToList(), "MaNXB", "TenNXB");
            ViewBag.MaChuDe = new SelectList(db.ChuDes.ToList(), "MaChuDe", "TenChuDe");
            // Lưu tên file
            var fileName = Path.GetFileName(fileUpLoad.FileName);
            // Lưu đường dẫn
            var path = Path.Combine(Server.MapPath("~/Assets"), fileName);
            if (System.IO.File.Exists(path))
            {
                ViewBag.ThongBao = "Thêm không thành công, Hình ảnh đã tồn tại!";
            }
            else
            {
                fileUpLoad.SaveAs(path);
                string sNgayCapNhat = f2.Get("NgayCapNhat").ToString();
                string sTenSach = f2["TenSach"].ToString();
                string sMaNXB = f2["MaNXB"].ToString();
                string sMaChuDe = f2["MaChuDe"].ToString();
                string sSoLuong = f2["SoLuong"].ToString();
                string sMoTa = f2["MoTa"].ToString();
                string sGiaBan = f2["GiaBan"].ToString();


                string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(mainconn);
                SqlCommand sqlcomm = new SqlCommand("[dbo].[ThemSach]");
                sqlconn.Open();
                sqlcomm.Connection = sqlconn;
                sqlcomm.CommandType = CommandType.StoredProcedure;
                sqlcomm.Parameters.AddWithValue("@TenSach", sTenSach);
                sqlcomm.Parameters.AddWithValue("@GiaBan", sGiaBan);
                sqlcomm.Parameters.AddWithValue("@MoTa", sMoTa);
                sqlcomm.Parameters.AddWithValue("@AnhBia", fileName.ToString());
                sqlcomm.Parameters.AddWithValue("@NgayCapNhat", sNgayCapNhat);
                sqlcomm.Parameters.AddWithValue("@SoLuong", sSoLuong);
                sqlcomm.Parameters.AddWithValue("@MaNXB", sMaNXB);
                sqlcomm.Parameters.AddWithValue("@MaChuDe", sMaChuDe);
                SqlDataReader sdr = sqlcomm.ExecuteReader();

                ViewBag.ThongBao = "Thêm thành công!";

                return View();

            }
            return View();

        }
        [HttpGet]
        public ActionResult XemSanPham()
        {

            return View(db.Saches.ToList());
        }

        [HttpPost]
        public ActionResult XemSanPham(FormCollection f3)
        {
            string sTenSach = f3["TenSach"].ToString();
            if (sTenSach.Length > 0)
            {
                string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(mainconn);
                SqlCommand sqlcomm = new SqlCommand("[dbo].[TimKiemSach_Admin]");

                sqlconn.Open();
                sqlcomm.Connection = sqlconn;
                sqlcomm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

                sqlcomm.Parameters.AddWithValue("@TenSach", sTenSach);

                DataTable dt = new DataTable();
                List<Sach> listSachs = new List<Sach>();
                sd.Fill(dt);
                sqlconn.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    listSachs.Add(
                        new Sach
                        {
                            MaSach = Convert.ToInt32(dr["MaSach"]),
                            TenSach = Convert.ToString(dr["TenSach"]),
                            GiaBan = Convert.ToDecimal(dr["GiaBan"]),
                            MoTa = Convert.ToString(dr["MoTa"]),
                            AnhBia = Convert.ToString(dr["AnhBia"]),
                            NgayCapNhat = Convert.ToDateTime(dr["NgayCapNhat"]),
                            SoLuong = Convert.ToInt32(dr["SoLuong"]),
                            MaNXB = Convert.ToInt32(dr["MaNXB"]),
                            MaChuDe = Convert.ToInt32(dr["MaChuDe"])
                        });
                }
                if (listSachs.Count() > 0)
                    return View(listSachs);
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy sách nào có tên " + "'" + sTenSach.ToString() + "' !";
                    return View(listSachs);
                }
            }
            return View();

        }
        [HttpGet]
        public ActionResult XemChiTietSanPham(int idSach)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChitietSach]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaSach", idSach);

            DataTable dt = new DataTable();
            List<XemChiTietSanPham> infoSachs = new List<XemChiTietSanPham>();
            sd.Fill(dt);
            sqlconn.Close();

            foreach (DataRow dr in dt.Rows)
            {
                infoSachs.Add(
                    new XemChiTietSanPham
                    {
                        MaSach = Convert.ToInt32(dr["MaSach"]),
                        TenSach = Convert.ToString(dr["TenSach"]),
                        GiaBan = Convert.ToDecimal(dr["GiaBan"]),
                        MoTa = Convert.ToString(dr["MoTa"]),
                        AnhBia = Convert.ToString(dr["AnhBia"]),
                        NgayCapNhat = Convert.ToDateTime(dr["NgayCapNhat"]),
                        SoLuong = Convert.ToInt32(dr["SoLuong"]),
                        TenNXB = Convert.ToString(dr["TenNXB"]),
                        TenChuDe = Convert.ToString(dr["TenChuDe"]),
                        TenTacGia = Convert.ToString(dr["TenTacGia"])

                    });
            }

            return View(infoSachs);
        }
        [HttpGet]
        public ActionResult XoaSach(int idSach)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XoaSach]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaSach", idSach);
            SqlDataReader sdr = sqlcomm.ExecuteReader();
            if (sdr.Read())
            {
                return RedirectToAction("XemSanPham", "Admin");
            }
            return RedirectToAction("XemSanPham", "Admin");
        }
        [HttpGet]
        public ActionResult ChinhSuaSach(int idSach)
        {
            ViewBag.MaNXB = new SelectList(db.NXBs.ToList(), "MaNXB", "TenNXB");
            ViewBag.MaChuDe = new SelectList(db.ChuDes.ToList(), "MaChuDe", "TenChuDe");

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChitietSach]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaSach", idSach);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();
            Sach xem = new Sach();
            foreach (DataRow dr in dt.Rows)
            {
                xem.MaSach = Convert.ToInt32(dr["MaSach"]);
                xem.TenSach = Convert.ToString(dr["TenSach"]);
                xem.GiaBan = Convert.ToDecimal(dr["GiaBan"]);
                xem.MoTa = Convert.ToString(dr["MoTa"]);
                xem.AnhBia = Convert.ToString(dr["AnhBia"]);
                xem.NgayCapNhat = Convert.ToDateTime(dr["NgayCapNhat"]);
                xem.SoLuong = Convert.ToInt32(dr["SoLuong"]);
            }
            return View(xem);
        }
        [HttpPost]
        public ActionResult ChinhSuaSach(FormCollection f, int idSach, Sach sach)
        {
            ViewBag.MaNXB = new SelectList(db.NXBs.ToList(), "MaNXB", "TenNXB");
            ViewBag.MaChuDe = new SelectList(db.ChuDes.ToList(), "MaChuDe", "TenChuDe");

            string sNgayCapNhat = f.Get("NgayCapNhat").ToString();
            string sTenSach = f["TenSach"].ToString();
            string sMaNXB = f["MaNXB"].ToString();
            string sMaChuDe = f["MaChuDe"].ToString();
            string sSoLuong = f["SoLuong"].ToString();
            string sMoTa = f["MoTa"].ToString();
            string sGiaBan = f["GiaBan"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ChinhSuaSach]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaSach", idSach);
            sqlcomm.Parameters.AddWithValue("@TenSach", sTenSach);
            sqlcomm.Parameters.AddWithValue("@GiaBan", sGiaBan);
            sqlcomm.Parameters.AddWithValue("@MoTa", sMoTa);
            sqlcomm.Parameters.AddWithValue("@NgayCapNhat", sNgayCapNhat);
            sqlcomm.Parameters.AddWithValue("@SoLuong", sSoLuong);
            sqlcomm.Parameters.AddWithValue("@MaNXB", sMaNXB);
            sqlcomm.Parameters.AddWithValue("@MaChuDe", sMaChuDe);
            SqlDataReader sdr = sqlcomm.ExecuteReader();



            return RedirectToAction("XemSanPham", "Admin");
        }
        [HttpGet]

        /*-------------TÁC GIẢ-----------*/
        public ActionResult ThemTacGia()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemTacGia(FormCollection f)
        {
            string sTenTacGia = f["TenTacGia"].ToString();
            string sDiaChi = f["DiaChi"].ToString();
            string sTieuSu = f["TieuSu"].ToString();
            string sSDT = f["SDT"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ThemTacGia]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@TenTacGia", sTenTacGia);
            sqlcomm.Parameters.AddWithValue("@DiaChi", sDiaChi);
            sqlcomm.Parameters.AddWithValue("@TieuSu", sTieuSu);
            sqlcomm.Parameters.AddWithValue("@SDT", sSDT);
            SqlDataReader sdr = sqlcomm.ExecuteReader();

            ViewBag.ThongBao = "Thêm thành công!";

            return View();
        }
        [HttpGet]
        public ActionResult XemTacGia()
        {
            return View(db.TacGias.ToList());
        }
        [HttpPost]
        public ActionResult XemTacGia(FormCollection f)
        {
            string sTenTacGia = f["TenTacGia"].ToString();
            if (sTenTacGia.Length > 0)
            {
                string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(mainconn);
                SqlCommand sqlcomm = new SqlCommand("[dbo].[TimKiemTacGia]");

                sqlconn.Open();
                sqlcomm.Connection = sqlconn;
                sqlcomm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

                sqlcomm.Parameters.AddWithValue("@TenTacGia", sTenTacGia);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                sqlconn.Close();

                List<TacGia> listTacGia = new List<TacGia>();

                foreach (DataRow dr in dt.Rows)
                {
                    listTacGia.Add(
                        new TacGia
                        {
                            MaTacGia = Convert.ToInt32(dr["MaTacGia"]),
                            TenTacGia = Convert.ToString(dr["TenTacGia"]),
                            DiaChi = Convert.ToString(dr["DiaChi"]),
                            TieuSu = Convert.ToString(dr["TieuSu"]),
                            SDT = Convert.ToString(dr["SDT"]),
                        });
                }
                if (listTacGia.Count() > 0)
                    return View(listTacGia);
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy sách nào có tên " + "'" + sTenTacGia.ToString() + "' !";

                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public ActionResult XemChiTietTacGia(int idTacGia)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChiTietTacGia]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaTacGia", idTacGia);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();
            TacGia xem = new TacGia();
            foreach (DataRow dr in dt.Rows)
            {
                xem.MaTacGia = Convert.ToInt32(dr["MaTacGia"]);
                xem.TenTacGia = Convert.ToString(dr["TenTacGia"]);
                xem.DiaChi = Convert.ToString(dr["DiaChi"]);
                xem.TieuSu = Convert.ToString(dr["TieuSu"]);
                xem.SDT = Convert.ToString(dr["SDT"]);
            }
            return View(xem);
        }
        [HttpGet]
        public ActionResult XoaTacGia(int idTacGia)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XoaTacGia]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaTacGia", idTacGia);
            SqlDataReader sdr = sqlcomm.ExecuteReader();
            if (sdr.Read())
            {
                return RedirectToAction("XemTacGia", "Admin");
            }
            return RedirectToAction("XemTacGia", "Admin");
        }
        public ActionResult ChinhSuaTacGia(int idTacGia)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChiTietTacGia]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaTacGia", idTacGia);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();
            TacGia xem = new TacGia();
            foreach (DataRow dr in dt.Rows)
            {
                xem.MaTacGia = Convert.ToInt32(dr["MaTacGia"]);
                xem.TenTacGia = Convert.ToString(dr["TenTacGia"]);
                xem.DiaChi = Convert.ToString(dr["DiaChi"]);
                xem.TieuSu = Convert.ToString(dr["TieuSu"]);
                xem.SDT = Convert.ToString(dr["SDT"]);
            }
            return View(xem);
        }
        [HttpPost]
        public ActionResult ChinhSuaTacGia(FormCollection f, int idTacGia, TacGia tacGia)
        {

            string sTenTacGia = f.Get("TenTacGia").ToString();
            string sDiaChi = f["DiaChi"].ToString();
            string sSDT = f["SDT"].ToString();
            string sTieuSu = f["TieuSu"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ChinhSuaTacGia]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaTacGia", idTacGia);
            sqlcomm.Parameters.AddWithValue("@TenTacGia", sTenTacGia);
            sqlcomm.Parameters.AddWithValue("@DiaChi", sDiaChi);
            sqlcomm.Parameters.AddWithValue("@TieuSu", sTieuSu);
            sqlcomm.Parameters.AddWithValue("@SDT", sSDT);
            SqlDataReader sdr = sqlcomm.ExecuteReader();



            return RedirectToAction("XemTacGia", "Admin");
        }

        /*-------------NHÀ XUẤT BẢN-----------*/
        [HttpGet]
        public ActionResult ThemNXB()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemNXB(FormCollection f)
        {
            string sTenNXB = f["TenNXB"].ToString();
            string sDiaChi = f["DiaChi"].ToString();
            string sSDT = f["SDT"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ThemNXB]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@TenNXB", sTenNXB);
            sqlcomm.Parameters.AddWithValue("@DiaChi", sDiaChi);
            sqlcomm.Parameters.AddWithValue("@SDT", sSDT);
            SqlDataReader sdr = sqlcomm.ExecuteReader();

            ViewBag.ThongBao = "Thêm thành công!";

            return View();
        }
        [HttpGet]
        public ActionResult XemNXB()
        {
            return View(db.NXBs.ToList());
        }
        [HttpPost]
        public ActionResult XemNXB(FormCollection f)
        {
            string sTenNXB = f["TenNXB"].ToString();
            if (sTenNXB.Length > 0)
            {
                string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(mainconn);
                SqlCommand sqlcomm = new SqlCommand("[dbo].[TimKiemNXB]");

                sqlconn.Open();
                sqlcomm.Connection = sqlconn;
                sqlcomm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

                sqlcomm.Parameters.AddWithValue("@TenNXB", sTenNXB);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                sqlconn.Close();

                List<NXB> listNXB = new List<NXB>();

                foreach (DataRow dr in dt.Rows)
                {
                    listNXB.Add(
                        new NXB
                        {
                            MaNXB = Convert.ToInt32(dr["MaNXB"]),
                            TenNXB = Convert.ToString(dr["TenNXB"]),
                            DiaChi = Convert.ToString(dr["DiaChi"]),
                            SDT = Convert.ToString(dr["SDT"]),
                        });
                }
                if (listNXB.Count() > 0)
                    return View(listNXB);
                else
                {
                    ViewBag.ThongBao = "Không tìm thấy sách nào có tên " + "'" + sTenNXB.ToString() + "' !";

                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public ActionResult XemChiTietNXB(int idNXB)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChiTietNXB]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaNXB", idNXB);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();
            NXB xem = new NXB();
            foreach (DataRow dr in dt.Rows)
            {
                xem.MaNXB = Convert.ToInt32(dr["MaNXB"]);
                xem.TenNXB = Convert.ToString(dr["TenNXB"]);
                xem.DiaChi = Convert.ToString(dr["DiaChi"]);
                xem.SDT = Convert.ToString(dr["SDT"]);
            }
            return View(xem);
        }
        [HttpGet]
        public ActionResult XoaNXB(int idNXB)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XoaNXB]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaNXB", idNXB);
            SqlDataReader sdr = sqlcomm.ExecuteReader();
            if (sdr.Read())
            {
                return RedirectToAction("XemNXB", "Admin");
            }
            return RedirectToAction("XemNXB", "Admin");
        }
        [HttpGet]
        public ActionResult ChinhSuaNXB(int idNXB)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[XemChiTietNXB]");

            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);

            sqlcomm.Parameters.AddWithValue("@MaNXB", idNXB);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();
            NXB xem = new NXB();
            foreach (DataRow dr in dt.Rows)
            {
                xem.MaNXB = Convert.ToInt32(dr["MaNXB"]);
                xem.TenNXB = Convert.ToString(dr["TenNXB"]);
                xem.DiaChi = Convert.ToString(dr["DiaChi"]);
                xem.SDT = Convert.ToString(dr["SDT"]);
            }
            return View(xem);
        }
        [HttpPost]
        public ActionResult ChinhSuaNXB(FormCollection f, int idNXB, NXB nXB)
        {
            string sTenNXB = f.Get("TenNXB").ToString();
            string sDiaChi = f["DiaChi"].ToString();
            string sSDT = f["SDT"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ChinhSuaNXB]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaNXB", idNXB);
            sqlcomm.Parameters.AddWithValue("@TenNXB", sTenNXB);
            sqlcomm.Parameters.AddWithValue("@DiaChi", sDiaChi);
            sqlcomm.Parameters.AddWithValue("@SDT", sSDT);
            SqlDataReader sdr = sqlcomm.ExecuteReader();



            return RedirectToAction("XemNXB", "Admin");
        }


        /*-------------SỞ HỮU-----------*/
        [HttpGet]
        public ActionResult ThemSoHuu()
        {
            ViewBag.MaSach = new SelectList(db.Saches.ToList(), "MaSach", "TenSach");
            ViewBag.MaTacGia = new SelectList(db.TacGias.ToList(), "MaTacGia", "TenTacGia");
            return View();
        }
        [HttpPost]
        public ActionResult ThemSoHuu(FormCollection f)
        {
            ViewBag.MaSach = new SelectList(db.Saches.ToList(), "MaSach", "TenSach");
            ViewBag.MaTacGia = new SelectList(db.TacGias.ToList(), "MaTacGia", "TenTacGia");

            string sMaTacGia = f["MaTacGia"].ToString();
            string sMaSach = f["MaSach"].ToString();
            string sViTri = f["ViTri"].ToString();
            string sVaiTro = f["VaiTro"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ThemSoHuu]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@MaTacGia", sMaTacGia);
            sqlcomm.Parameters.AddWithValue("@MaSach", sMaSach);
            sqlcomm.Parameters.AddWithValue("@ViTri", sViTri);
            sqlcomm.Parameters.AddWithValue("@VaiTro", sVaiTro);
            SqlDataReader sdr = sqlcomm.ExecuteReader();

            ViewBag.ThongBao = "Thêm thành công!";

            return View();
        }
        [HttpGet]
        public ActionResult XemSoHuu()
        {
            return View(db.SoHuus.ToList());
        }

        /*-------------BIỂU ĐỒ-----------*/
        [HttpGet]
        public ActionResult ThongKe()
        {
            List<ThongKeDoanhThuThang> listTK = new List<ThongKeDoanhThuThang>();
            return View(listTK);
        }
        [HttpPost]
        public ActionResult ThongKe(FormCollection f)
        {
            string sNam = f["ChonNam"].ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[ThongKeDoanhThuThang]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);
            sqlcomm.Parameters.AddWithValue("@Nam", sNam);

            DataTable dt = new DataTable();

            sd.Fill(dt);
            sqlconn.Close();

            ThongKeDoanhThuThang xem = new ThongKeDoanhThuThang();
            List<ThongKeDoanhThuThang> listTK = new List<ThongKeDoanhThuThang>();
            foreach (DataRow dr in dt.Rows)
            {
                listTK.Add(
                    new ThongKeDoanhThuThang
                    {
                        Thang = Convert.ToInt32(dr["Thang"]),
                        Tong = Convert.ToInt32(dr["Tong"]),

                    });
            }
            ViewBag.Dau = ",";
            return View(listTK);
        }

    }
}