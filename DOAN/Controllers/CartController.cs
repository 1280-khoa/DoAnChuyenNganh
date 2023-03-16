using DOAN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOAN.Models;
using System.Configuration;
using Common;
using DOAN.Others;
using System.Net.NetworkInformation;

namespace DOAN.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DBSHOPDataContext data = new DBSHOPDataContext();

        public List<Cart> Laygiohang()
        {
            List<Cart> list = Session["Cart"] as List<Cart>;
            if (list == null)
            {
                list = new List<Cart>();
                Session["Cart"] = list;
            }
            return list;
        }
        public ActionResult AddCart(int IDSP, string strURL)
        {
            List<Cart> list = Laygiohang();
            Cart sp = list.Find(n => n.ID_SanPham == IDSP);
            if (sp == null)
            {
                sp = new Cart(IDSP);
                list.Add(sp);
                return Redirect(strURL);
            }
            else
            {
                sp.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int TongSoLuongs = 0;
            List<Cart> list = Session["Cart"] as List<Cart>;
            if (list != null)
            {
                TongSoLuongs = list.Sum(n => n.iSoluong);
            }
            return TongSoLuongs;
        }
        private int TongTien()
        {
            int iTongTien = 0;
            List<Cart> lstGiohang = Session["Cart"] as List<Cart>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult Cart()
        {
            List<Cart> list = Laygiohang();
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Store");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(list);
        }

        public ActionResult CartPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult DeleteCart(int IDSP)
        {
            List<Cart> list = Laygiohang();
            Cart cart = list.SingleOrDefault(n => n.ID_SanPham == IDSP);
            if (cart != null)
            {
                list.RemoveAll(n => n.ID_SanPham == IDSP);
                return RedirectToAction("Cart");
            }
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Store");

            }
            return RedirectToAction("Cart");
        }
        public ActionResult UpdateCart(int IDSP, FormCollection f)
        {
            List<Cart> list = Laygiohang();
            Cart cart = list.SingleOrDefault(n => n.ID_SanPham == IDSP);
            if (cart != null)
            {
                cart.iSoluong = int.Parse(f["quantity"].ToString());

            }
            return RedirectToAction("Cart");
        }
        public ActionResult DeleteFullCart()
        {
            List<Cart> list = Laygiohang();
            list.Clear();
            return RedirectToAction("Index", "Store");
        }
        [HttpGet]
        public ActionResult Oder()
        {
            if (Session["Account_KH"] == null || Session["Account_KH"].ToString() == "")
            {
                return RedirectToAction("Signin", "Accountuser");

            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Store");
            }
            ViewBag.PT_ThanhToan = new SelectList(data.PhuongThucThanhToans.ToList().OrderBy(n => n.PhuongThuc), "PT_ThanhToan", "PhuongThuc");

            List<Cart> list = Laygiohang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(list);
        }
        public ActionResult Oder(DonHang dh, FormCollection collection)
        {

            KhachHang kh = (KhachHang)Session["Account_KH"];

            ViewBag.PT_ThanhToan = new SelectList(data.PhuongThucThanhToans.ToList().OrderBy(n => n.PhuongThuc), "PT_ThanhToan", "PhuongThuc");
            dh.ID_KH = kh.ID_KH;
            dh.NgayDat = DateTime.Now;
            dh.TrangThai = 1;
            dh.TrangThaiThanhToan = 1;
            dh.TenNguoiNhan = kh.HoTen_KH;
            dh.SDT_NguoiNhan = kh.SDT_KH;
            dh.DiaChiNguoiNhan = kh.DiaChi_KH;
            List<Cart> list = Laygiohang();
            ViewBag.TongTien = TongTien();
            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();
           
            foreach (var item in list)
            {
                CT_DonHang ct = new CT_DonHang();
                ct.ID_DonHang = dh.ID_DonHang;
                ct.ID_SanPham = item.ID_SanPham;
                ct.SoLuong = item.iSoluong;
                ct.DonGia = (long)(decimal)item.dDongia;                
                data.CT_DonHangs.InsertOnSubmit(ct);

            }
          
            var strSanpham = "";
            var thanhtien = decimal.Zero;
            var Tongtien = decimal.Zero;
            foreach (var item in list)
            {
                strSanpham += "Tên sản phẩm : " + item.TenSanPham + "<br />";
                strSanpham += "Số lượng :" + item.iSoluong + "<br />"; ;
                strSanpham += "Giá /1 sản phẩm : " + String.Format("{0:n}", (decimal)item.dDongia) + "<br />";

                thanhtien += (decimal)item.dDongia * item.iSoluong;
            }
            Tongtien = thanhtien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templateSendmail/send.cshtml"));
            //contentCustomer = contentCustomer.Replace("{{MaDon}}",ddh );
            contentCustomer = contentCustomer.Replace("{{TenSP}}", strSanpham);
            contentCustomer = contentCustomer.Replace("{{Tenkhachhang}}", kh.HoTen_KH);
            contentCustomer = contentCustomer.Replace("{{Phone}}", kh.SDT_KH);
            contentCustomer = contentCustomer.Replace("{{Email}}", kh.Account_KH);
            contentCustomer = contentCustomer.Replace("{{Address}}", kh.DiaChi_KH);
            contentCustomer = contentCustomer.Replace("{{Tongtien}}", String.Format("{0:n}", Tongtien));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendMail(kh.Account_KH, "Demo", contentCustomer);
            new MailHelper().SendMail(toEmail, "Demo", contentCustomer);
            data.SubmitChanges();
            Session["Cart"] = null;
                return RedirectToAction("Orderconfirmation", "Cart");

        }
        public ActionResult ThanhToan()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            ViewBag.PT_ThanhToan = new SelectList(data.PhuongThucThanhToans.ToList().OrderBy(n => n.PhuongThuc), "PT_ThanhToan", "PhuongThuc");
            string totals = (TongTien() * 100).ToString(); //total là tổng của session giỏ hàng

            XuLy pay = new XuLy();

            //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Version", "2.1.0");

            //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_Command", "pay");

            //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_TmnCode", tmnCode);

            //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            //totals là value đã ép kiểu sang kiểu chuỗi ở phía trên
            pay.AddRequestData("vnp_Amount", totals);

            //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_BankCode", "");

            //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_CurrCode", "VND");

            //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_IpAddr", ChuyenDoi.GetIpAddress());

            //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_Locale", "vn");

            //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng");

            //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_OrderType", "other");

            //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);

            //mã hóa đơn
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);

        }
        public ActionResult XacNhanThanhToan(DonHang dh, FormCollection collection)
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                XuLy pay = new XuLy();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //mã hóa đơn
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));

                //mã giao dịch tại hệ thống VNPAY
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));

                //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                //hash của dữ liệu trả về
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                //check chữ ký đúng hay không?
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        dh.PT_ThanhToan = 2;
                        dh.TrangThaiThanhToan = 2;

                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        dh.TrangThaiThanhToan = 1;

                    }
                    KhachHang kh = (KhachHang)Session["Account_KH"];

                    dh.ID_KH = kh.ID_KH;
                    dh.NgayDat = DateTime.Now;
                    dh.TrangThai = 1;
                    var NgayGiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);                   
                    dh.TenNguoiNhan = kh.HoTen_KH;
                    dh.SDT_NguoiNhan = kh.SDT_KH;
                    dh.DiaChiNguoiNhan = kh.DiaChi_KH;
                    List<Cart> list = Laygiohang();
                    ViewBag.TongTien = TongTien();
                    data.DonHangs.InsertOnSubmit(dh);
                    data.SubmitChanges();
                        
                    foreach (var item in list)
                    {
                        CT_DonHang ct = new CT_DonHang();
                        ct.ID_DonHang = dh.ID_DonHang;
                        ct.ID_SanPham = item.ID_SanPham;
                        ct.SoLuong = item.iSoluong;
                        ct.DonGia = (long)(decimal)item.dDongia;
                        data.CT_DonHangs.InsertOnSubmit(ct);

                    }
                    var strSanpham = "";
                    var thanhtien = decimal.Zero;
                    var Tongtien = decimal.Zero;
                    foreach (var item in list)
                    {
                        strSanpham += "Tên sản phẩm : " + item.TenSanPham + "<br />";
                        strSanpham += "Số lượng :" + item.iSoluong + "<br />"; ;
                        strSanpham += "Giá /1 sản phẩm : " + String.Format("{0:n}", (decimal)item.dDongia) + "<br />";

                        thanhtien += (decimal)item.dDongia * item.iSoluong;
                    }
                    Tongtien = thanhtien;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templateSendmail/send.cshtml"));
                    //contentCustomer = contentCustomer.Replace("{{MaDon}}",ddh );
                    contentCustomer = contentCustomer.Replace("{{TenSP}}", strSanpham);
                    contentCustomer = contentCustomer.Replace("{{Tenkhachhang}}", kh.HoTen_KH);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", kh.SDT_KH);
                    contentCustomer = contentCustomer.Replace("{{Email}}", kh.Account_KH);
                    contentCustomer = contentCustomer.Replace("{{Address}}", kh.DiaChi_KH);
                    contentCustomer = contentCustomer.Replace("{{Tongtien}}", String.Format("{0:n}", Tongtien));
                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new MailHelper().SendMail(kh.Account_KH, "Demo", contentCustomer);
                    new MailHelper().SendMail(toEmail, "Demo", contentCustomer);
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        public ActionResult Orderconfirmation()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DatLich()
        {
            if (Session["Account_KH"] == null || Session["Account_KH"].ToString() == "")
            {
                return RedirectToAction("Signin", "Accountuser");

            }

            ViewBag.Buoi = new SelectList(data.Buois.ToList().OrderBy(n => n.BuoiDatLich), "ID_Buoi", "BuoiDatLich");
            ViewBag.NguyenNhan = new SelectList(data.NguyenNhans.ToList().OrderBy(n => n.NguyenNhanDatLich), "ID_NguyenNhan", "NguyenNhanDatLich");
            ViewBag.DiaDiem = new SelectList(data.DiaDiems.ToList().OrderBy(n => n.DiaDiemDatLich), "ID_DiaDiem", "DiaDiemDatLich");


            return View();
        }

        public ActionResult DatLich(DatLich dh, FormCollection collection)
        {

            KhachHang kh = (KhachHang)Session["Account_KH"];

            ViewBag.Buoi = new SelectList(data.Buois.ToList().OrderBy(n => n.BuoiDatLich), "ID_Buoi", "BuoiDatLich");
            ViewBag.NguyenNhan = new SelectList(data.NguyenNhans.ToList().OrderBy(n => n.NguyenNhanDatLich), "ID_NguyenNhan", "NguyenNhanDatLich");
            ViewBag.DiaDiem = new SelectList(data.DiaDiems.ToList().OrderBy(n => n.DiaDiemDatLich), "ID_DiaDiem", "DiaDiemDatLich");
            dh.ID_KH = kh.ID_KH;
            
            dh.NgayDat = DateTime.Now;
            var NgayHen = String.Format("{0:MM/dd/yyyy}", collection["NgayHen"]);
            dh.NgayHen = DateTime.Parse(NgayHen);
            dh.TenNguoiDat = kh.HoTen_KH;
            dh.DiaChi = kh.DiaChi_KH;
            dh.SDT = kh.SDT_KH;
            dh.TrangThai = 1;
            data.DatLiches.InsertOnSubmit(dh);
            data.SubmitChanges();
            return RedirectToAction("Orderconfirmation", "Cart");
        }
    }
}