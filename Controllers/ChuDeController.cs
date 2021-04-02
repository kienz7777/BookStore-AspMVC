using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStoreASP.Models;

namespace BookStoreASP.Controllers
{
    public class ChuDeController : Controller
    {
        // GET: ChuDe
        WEBBANSACH db = new WEBBANSACH();
        public PartialViewResult ChuDePartial()
        {
            return PartialView(db.ChuDes.Take(4).OrderBy(n => n.TenChuDe).ToList());
        }
    }
}