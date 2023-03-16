using DOAN.Models;
using PagedList;
using System;
using System.Collections.Generic;


using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        DBSHOPDataContext data = new DBSHOPDataContext();
        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(FormCollection collection)
        {
            var user = collection["Account_NV"];
            var pass = collection["Password_NV"];
            if (String.IsNullOrEmpty(user))
            {
                ViewData["Loi1"] = "Phải Nhập Username";
            }
            else if (String.IsNullOrEmpty(pass))
            {
                ViewData["Loi2"] = "Phải Nhập Password";
            }
            else
            {
                NhanVien NV = data.NhanViens.SingleOrDefault(n => n.Account_NV == user && n.Password_NV == pass);
                if (NV != null)
                {
                    Session["NhanVien"] = NV;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.ThongBao = "Wrong account or password";
            }
            return View();
        }
        public ActionResult SanPham(int? page)
        {
            int PageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.SanPhams.ToList().OrderBy(n => n.ID_SanPham).ToPagedList(PageNumber, pageSize));
        }
        public ActionResult KhachHang(int? page)
        {
            int PageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.KhachHangs.ToList().OrderBy(n => n.ID_KH).ToPagedList(PageNumber, pageSize));
        }

        //[HttpGet]
        //public ActionResult EditKH(int id)
        //{
        //    KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.ID_KH == id);
        //    ViewBag.ID_SanPham = kh.ID_KH;
        //    if (kh == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    return View(kh);
        //}

        [HttpGet]
        public ActionResult CreateNew()
        {
            ViewBag.ID_Loai = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "ID_Loai", "TenLoai");
            ViewBag.ID_ThuongHieu = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "ID_ThuongHieu", "TenThuongHieu");
            return View();
        }

        [HttpPost]
        public ActionResult CreateNew(SanPham SP, HttpPostedFileBase fileupload)
        {
            ViewBag.ID_Loai = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "ID_Loai", "TenLoai");
            ViewBag.ID_ThuongHieu = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "ID_ThuongHieu", "TenThuongHieu");

            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui Lòng Chọn Ảnh ";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/asset/new/"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình Ảnh Đã Tồn Tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    SP.Image = fileName;
                    data.SanPhams.InsertOnSubmit(SP);
                    data.SubmitChanges();
                }
            }
            return RedirectToAction("SanPham");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(a => a.ID_SanPham == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.ID_Loai = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "ID_Loai", "TenLoai");
            ViewBag.ID_ThuongHieu = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "ID_ThuongHieu", "TenThuongHieu");
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, HttpPostedFileBase fileUpload)
        {

            var sp = data.SanPhams.FirstOrDefault(a => a.ID_SanPham == id);
            sp.ID_SanPham = id;
            ViewBag.ID_Loai = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "ID_Loai", "TenLoai");
            ViewBag.ID_ThuongHieu = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "ID_ThuongHieu", "TenThuongHieu");
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View(sp);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/asset/new/"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.ThongBao = "Ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sp.Image = fileName;
                    sp.ID_SanPham = id;

                    UpdateModel(sp);
                    data.SubmitChanges();
                    return RedirectToAction("SanPham");
                }
                return this.Edit(id);
            }

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var deck = from s in data.SanPhams
                       where s.ID_SanPham == id
                       select s;
            return View(deck.Single());
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult XacNhanDelete(int id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.ID_SanPham == id);
            ViewBag.ID_SanPham = sp.ID_SanPham;
            data.SanPhams.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        
        public ActionResult DonHang(int? page)
        {

            int PageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.SanPhams.ToList());
            return View(data.DonHangs.ToList().OrderBy(n => n.ID_DonHang).ToPagedList(PageNumber, pageSize));
        }
        
        //Chuyển trạng thái sang đang xử lý (đang được giao)
        public ActionResult TrangThaiDangGiao(int id)
        {
            var donhang = data.DonHangs.SingleOrDefault(n => n.ID_DonHang == id);
            if (donhang != null)
            {
                donhang.TrangThai = 3;
               
            }
            UpdateModel(donhang);
            data.SubmitChanges();
            return RedirectToAction("DonHang");

        }
        //Chuyển trạng thái đơn hàng sang hoàn thành
        public ActionResult TrangThaiHoanThanh(int id)
        {
            var donhang = data.DonHangs.SingleOrDefault(n => n.ID_DonHang == id);
            if (donhang != null)
            {
                donhang.TrangThai = 4;
                donhang.TrangThaiThanhToan = 2;
                
            }
            UpdateModel(donhang);
            data.SubmitChanges();
            return RedirectToAction("DonHang");
        }

       
        public ActionResult DatLich(int? page)
        {
            int PageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.DatLiches.ToList().OrderBy(n => n.ID_DatLich).ToPagedList(PageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult EditDatLich(int id)
        {
            DatLich datLich = data.DatLiches.SingleOrDefault(n => n.ID_DatLich == id);
            ViewBag.ID_DatLich = datLich.ID_DatLich;
            if (datLich == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.BuoiDatLich = new SelectList(data.Buois.ToList().OrderBy(n => n.BuoiDatLich), "ID_Buoi", "BuoiDatLich");
            ViewBag.TrangThai = new SelectList(data.TrangThaiDonHangs.ToList().OrderBy(n => n.TrangThaiDH), "TrangThai", "TrangThaiDH");
            ViewBag.NguyenNhan = new SelectList(data.NguyenNhans.ToList().OrderBy(n => n.NguyenNhanDatLich), "ID_NguyenNhan", "NguyenNhanDatLich");
            ViewBag.DiaDiem = new SelectList(data.DiaDiems.ToList().OrderBy(n => n.DiaDiemDatLich), "ID_DiaDiem", "DiaDiemDatLich");
            return View(datLich);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDatLich(DatLich datLich, int id, FormCollection collection)
        {
            datLich = data.DatLiches.First(n => n.ID_DatLich == id);
            ViewBag.BuoiDatLich = new SelectList(data.Buois.ToList().OrderBy(n => n.BuoiDatLich), "ID_Buoi", "BuoiDatLich");
            ViewBag.TrangThai = new SelectList(data.TrangThaiDonHangs.ToList().OrderBy(n => n.TrangThaiDH), "TrangThai", "TrangThaiDH");
            ViewBag.NguyenNhan = new SelectList(data.NguyenNhans.ToList().OrderBy(n => n.NguyenNhanDatLich), "ID_NguyenNhan", "NguyenNhanDatLich");
            ViewBag.DiaDiem = new SelectList(data.DiaDiems.ToList().OrderBy(n => n.DiaDiemDatLich), "ID_DiaDiem", "DiaDiemDatLich");
            var kh = collection["ID_KH"];
            var ghichu = collection["GhiChu"];
            var diachi = collection["DiaChi"];
            var hoten = collection["TenNguoiDat"];
            var sdt = collection["SDT"];
            var NgayDat = String.Format("{0:dd/mm/yyyy}", collection["NgayDat"]);
            var NgayHen = /*String.Format("{0:dd/mm/yyyy}", */collection["NgayHen"];
            var lydohuy = collection["LyDoHuy"];

            datLich.ID_DatLich = id;
            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Chưa Chọn Trạng Thái!";
            }
            else
            {
                datLich.TenNguoiDat = hoten;
                datLich.SDT = sdt;
                datLich.DiaChi = diachi;
                datLich.GhiChu = ghichu;
                datLich.NgayDat = DateTime.Parse(NgayDat);
                datLich.NgayHen = DateTime.Parse(NgayHen);
                datLich.LyDoHuy = lydohuy;
                ViewBag.BuoiDatLich = new SelectList(data.Buois.ToList().OrderBy(n => n.ID_Buoi), "ID_Buoi", "BuoiDatLich");
                ViewBag.TrangThai = new SelectList(data.TrangThaiDonHangs.ToList().OrderBy(n => n.TrangThai), "TrangThai", "TrangThaiDH");
                ViewBag.NguyenNhan = new SelectList(data.NguyenNhans.ToList().OrderBy(n => n.ID_NguyenNhan), "ID_NguyenNhan", "NguyenNhanDatLich");
                ViewBag.DiaDiem = new SelectList(data.DiaDiems.ToList().OrderBy(n => n.ID_DiaDiem), "ID_DiaDiem", "DiaDiemDatLich");
                //Update trong CSDL
                UpdateModel(datLich);
                data.SubmitChanges();
                return RedirectToAction("DatLich");
            }
            return this.EditDatLich(id);
        }
    }
}