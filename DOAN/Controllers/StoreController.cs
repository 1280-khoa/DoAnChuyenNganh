using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DOAN.Models;
using PagedList;
using PagedList.Mvc;

namespace DOAN.Controllers
{
    public class StoreController : Controller
    {
        // GET: Store
        DBSHOPDataContext data = new DBSHOPDataContext();
        private List<SanPham> laySanPhamMoi(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.Gia_SanPham).Take(count).ToList();
        }
        public ActionResult Index(int? page, string Text , string currentFilter)
        {
            var lstProduct = new List<SanPham>();
            if (Text != null)
            {
                page = 1;
            }
            else
            {
                Text = currentFilter;
            }
            if (!string.IsNullOrEmpty(Text))
            {
                //lay ds theo tu khoa tim kiem
                lstProduct = data.SanPhams.Where(n => n.TenSanPham.Contains(Text)).ToList();
            }
            else
            {
               
                lstProduct = data.SanPhams.ToList();
            }
            ViewBag.CurrentFilter = Text;
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            var sanphammoi = laySanPhamMoi(30000);
            //sap xep sp theo id sp, sp moi dua len dau
            return View(lstProduct.ToPagedList(pageNumber, pageSize));
           
    
        }
     
        public ActionResult CPU(int? page)
        {
            int pageSize = 20;
            int pagenum = (page ?? 1);


            var sanphammoi = laySanPhamMoi(30000);

            return View(sanphammoi.ToPagedList(pagenum, pageSize));
        }
        public ActionResult ManHinh(int? page)
        {
            int pageSize = 20;
            int pagenum = (page ?? 1);


            var sanphammoi = laySanPhamMoi(30000);

            return View(sanphammoi.ToPagedList(pagenum, pageSize));
        }
        public ActionResult LapTop(int? page)
        {
            int pageSize = 20;
            int pagenum = (page ?? 1);


            var sanphammoi = laySanPhamMoi(30000);

            return View(sanphammoi.ToPagedList(pagenum, pageSize));
        }
        public ActionResult IndexLogin(int? page)
        {

            int pageSize = 20;
            int pagenum = (page ?? 1);


            var sanphammoi = laySanPhamMoi(30000);

            return View(sanphammoi.ToPagedList(pagenum, pageSize));
        }
        public ActionResult Details1(int id)
        {
            var deck = from s in data.SanPhams
                       where s.ID_SanPham == id
                       select s;
            return View(deck.Single());
        }
        public ActionResult Details(int id)
        {
            var deck = from s in data.SanPhams
                       where s.ID_SanPham == id
                       select s;
            
            return View(deck.Single());
        }
        [HttpGet]
        public ActionResult ThemBinhLuanSP()
        {
         

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemBinhLuanSP(FormCollection collection)
        {
            BinhLuan binhluan = new BinhLuan();
           
            var noidungbl = collection["NoiDung"];
            var hoten = collection["TenKH"];
            binhluan.ID_SanPham = 35;
            binhluan.NoiDung = noidungbl
            binhluan.TenKH = hoten;
            binhluan.NgayCmt = DateTime.Now;
            data.BinhLuans.InsertOnSubmit(binhluan);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
        //Hiển thị bình luận sản phẩm
        public ActionResult BinhLuanSP(int id)
        {
            //Lay binh luan
            var binhluan = (from dg in data.BinhLuans orderby dg.NgayCmt descending where dg.ID_SanPham == id select dg).Take(5);
            return View(binhluan);
        }

    }
}