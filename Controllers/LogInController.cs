using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using BookStoreASP.Models;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BookStoreASP.Controllers
{
    public class LogInController : Controller
    {
        // GET: LogIn

        //WEBBANSACHEntities db = new WEBBANSACHEntities();
        WEBBANSACH db = new WEBBANSACH();
        [HttpGet]
        public ActionResult Log()
        {
            //var returnData = db.KhachHangs();
            return View();
        }

        [HttpPost]
        public ActionResult Log(FormCollection f)
        {

            string sTaiKhoan = f["email"].ToString();
            string sMatKhau = f.Get("password").ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[UserLogin]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@Email", sTaiKhoan);
            sqlcomm.Parameters.AddWithValue("@Matkhau", sMatKhau);
            SqlDataReader sdr = sqlcomm.ExecuteReader();

            if (sdr.Read())
            {
                // Random
                Random random = new Random();
                int rd = random.Next(100000, 999999);
                Session["OTP"] = rd;

                ViewBag.message = "Chúc Mừng Bạn Đăng Nhập Thành Công";
                KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.Email == sTaiKhoan && n.MatKhau == sMatKhau);
                Session["TaiKhoan"] = kh;

                MailMessage mm = new MailMessage("kienvu729@gmail.com", sTaiKhoan);
                mm.Subject = "Your Password ! ";
                // mm.Body = string.Format("Hello : <h1>{0}</h1> is your email <br/> your password is <h1>{1}</h1>", kh.Email, kh.MatKhau);
                mm.Body = string.Format("Hello : <h1>{0}</h1> is your OTP", rd);
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential();
                nc.UserName = "kienvu729@gmail.com";
                nc.Password = "kien01286843931";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                smtp.Port = 587;
                smtp.Send(mm);

                //return RedirectToAction("Index", "Home");
                return RedirectToAction("ForgetPass", "Login");
            }

            else
            {
                ViewBag.message = "Đăng Nhập Thất Bại";
            }

            sqlconn.Close();

            return View();


            //string sTaiKhoan = f["email"].ToString();
            //string sMatKhau = f.Get("password").ToString();
            //KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.Email == sTaiKhoan && n.MatKhau == sMatKhau);
            //if (kh != null)
            //{
            //    ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công ";
            //    Session["TaiKhoan"] = kh;
            //    return View();
            //}
            //ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
            //return View();
        }

        public ActionResult LogPartial()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {

                ViewBag.hello = "";
            }
            else
            {
                KhachHang kh = (KhachHang)Session["TaiKhoan"];
                ViewBag.hello = kh.TenKH;
            }

            return PartialView();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection f)
        {
            string sTenKH = f["TenKH"].ToString();
            string sSDT = f["SDT"].ToString();
            string sDiaChi = f["DiaChi"].ToString();
            string sGioiTinh = f["GioiTinh"].ToString();
            string sSTK = f["STK"].ToString();
            string sTaiKhoan = f["email"].ToString();
            string sMatKhau = f.Get("password").ToString();
            string sMatKhau1 = f.Get("password1").ToString();

            string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[Register]");
            sqlconn.Open();
            sqlcomm.Connection = sqlconn;

            if (sMatKhau == sMatKhau1)
            {
                sqlcomm.CommandType = CommandType.StoredProcedure;
                sqlcomm.Parameters.AddWithValue("@TenKH", sTenKH);
                sqlcomm.Parameters.AddWithValue("@SDT", sSDT);
                sqlcomm.Parameters.AddWithValue("@DiaChi", sDiaChi);
                sqlcomm.Parameters.AddWithValue("@GioiTinh", sGioiTinh);
                sqlcomm.Parameters.AddWithValue("@STK", sSTK);
                sqlcomm.Parameters.AddWithValue("@Email", sTaiKhoan);
                sqlcomm.Parameters.AddWithValue("@Matkhau", sMatKhau);
                SqlDataReader sdr = sqlcomm.ExecuteReader();

                ViewBag.message = "Chúc Mừng Bạn Đăng Ký Thành Công";
            }

            else
            {
                ViewBag.message = "Đăng Ký Thất Bại";
            }

            sqlconn.Close();
            return View();
        }
        [HttpGet]
        public ActionResult MapPartial()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                ViewBag.DN = "Bạn cần đăng nhập";
                return View();
            }
            else
            {
                KhachHang kh = (KhachHang)Session["TaiKhoan"];
                string mainconn = ConfigurationManager.ConnectionStrings["WEBBANSACH"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(mainconn);
                SqlCommand sqlcomm = new SqlCommand("[dbo].[map_proc]");
                sqlconn.Open();
                sqlcomm.Connection = sqlconn;
                sqlcomm.CommandType = CommandType.StoredProcedure;
                sqlcomm.Parameters.AddWithValue("@MaKH", kh.MaKH);

                SqlDataAdapter sd = new SqlDataAdapter(sqlcomm);
                object a = sqlcomm.ExecuteScalar();
                DataTable dt = new DataTable();
                KhachHang xem = new KhachHang();
                xem.DiaChi = sqlcomm.ExecuteScalar().ToString();
                if (a == DBNull.Value)
                {
                    ViewBag.DN = "Không tìm thấy địa chỉ";
                }
                return View(xem);
            }
        }
        //-----------Quen MK------------
        [HttpGet]
        public ActionResult ForgetPass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPass(FormCollection f)
        {
            string sRandom = f["email"].ToString();
            int rd2 = int.Parse(sRandom);

            int rd = (int)Session["OTP"];

            if (rd2 == rd)
            {
                Random random = new Random();
                int wa = random.Next(100000, 999999);
                Session["OTP"] = wa;

                //Gửi WhatsApp
                //ACaef3c4650eb58f93d19b0b1fe155442e
                //a3a016697de7c6103bed8148ce63346f

                const string accountSid = "AC56ba7cb3f429acb5ee52e5b9cbb42f01";
                const string authToken = "cd18b3e78c56658736a9f898664bbdd2";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "Your OTP is: " + wa,
                    from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                    to: new Twilio.Types.PhoneNumber("whatsapp:+84564730914")//
                );
                return RedirectToAction("WhatsApp", "LogIn");
            }
            else
            {
                ViewBag.message = "Mã OTP không chính xác";
                return View();
            }


        }
        [HttpGet]
        public ActionResult WhatsApp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WhatsApp(FormCollection f)
        {
            string sRandom2 = f["otp"].ToString();

            int rd3 = int.Parse(sRandom2);
            int rd4 = (int)Session["OTP"];

            if (rd3 == rd4)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewBag.message = "Mã OTP không chính xác";
                return View();
            }
        }
    }

}