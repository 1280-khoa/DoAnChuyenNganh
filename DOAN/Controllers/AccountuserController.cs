using DOAN.Models;
using System;
 
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace DOAN.Controllers
{
    public class AccountuserController : Controller
    {
        // GET: Accountuser
        DBSHOPDataContext data = new DBSHOPDataContext();
        [HttpGet]
        public ActionResult Registers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registers(FormCollection Collection, KhachHang KH)
        {
            var hoten = Collection["HoTen_KH"];
            var user = Collection["Account_KH"];
            var pass =MaHoa.GetMD5( Collection["Password_KH"]);
            var nhaplaipass =MaHoa.GetMD5( Collection["matkhaunhaplai"]);
            var diachi = Collection["DiaChi_KH"];
            var SDT = Collection["SDT_KH"];
            var check_username = data.KhachHangs.FirstOrDefault(n => n.Account_KH == user);
            
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Lỗi1"] = "HỌ TÊN KHÔNG ĐƯỢC ĐỂ TRỐNG!";
            }
            else if (String.IsNullOrEmpty(user))
            {
                ViewData["Lỗi2"] = "USER KHÔNG ĐƯỢC ĐỂ TRỐNG!!";
            }
            else if (String.IsNullOrEmpty(pass))
            {
                ViewData["Lỗi3"] = "PASS KHÔNG ĐƯỢC ĐỂ TRỐNG!!";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Lỗi4"] = "ĐỊA CHỈ KHÔNG ĐƯỢC ĐỂ TRỐNG!!";
            }
            else if (String.IsNullOrEmpty(SDT))
            {
                ViewData["Lỗi5"] = "SĐT KHÔNG ĐƯỢC ĐỂ TRỐNG!!";
            }
            else if (String.IsNullOrEmpty(nhaplaipass))
            {
                ViewData["Lỗi6"] = "Phải Nhập Lại Mật Khẩu";
            }
            else if (check_username != null)
            {
                ViewBag.Warning = "Username này đã tồn tại!!";
            }
            else if (pass != nhaplaipass)
            {
                ViewBag.Warning = "Mật Khẩu Nhập lại không trùng khớp!!";
            }
            else
            {
                KH.HoTen_KH = hoten;
                KH.Account_KH = user;
                KH.Password_KH =MaHoa.GetMD5( pass);
                KH.DiaChi_KH = diachi;
                KH.SDT_KH = SDT;
                data.KhachHangs.InsertOnSubmit(KH);
                data.SubmitChanges();
                return RedirectToAction("SignIn");
            }
            return this.Registers();
        }
        [HttpGet]
        public ActionResult Signin()
        {
            return View();
        }
        public ActionResult Signin(FormCollection Collection)
        {
            var account = Collection["ACcount_KH"];
            var pass = MaHoa.GetMD5(Collection["Password_KH"]);
            if (String.IsNullOrEmpty(account))
            {
                ViewData["Lôi1"] = "Phải Nhập Tên Đăng Nhập!";

            }
            else if (String.IsNullOrEmpty(pass))
            {
                ViewData["Lôi2"] = "Phải Nhập Mật Khẩu!";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(k => k.Account_KH == account && k.Password_KH ==MaHoa.GetMD5( pass));
                if (kh != null)
                {

                    Session["Account_KH"] = kh;
                    return RedirectToAction("IndexLogin","Home");

                }
                else
                {
                    ViewBag.Thongbao = "Mật Khẩu Hoặc Tài Khoản Không Đúng!!";
                }
            }
            return View();
        }
        public ActionResult DetailsKH()
        {
            KhachHang kh = new KhachHang();
            
            if (Session["Account_KH"] == null || Session["Account_KH"].ToString() == "")
            {
                return RedirectToAction("Signin", "Accountuser");
            }
            
            
            else 
            {
                 kh = (KhachHang)Session["Account_KH"];


            }
            return View(kh);

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.ID_KH == id);
            ViewBag.ID_KH = kh.ID_KH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(int id, FormCollection collection)
        {
            //Tạo 1 biến khachhang với đối tương id = id truyền vào
            var khachhang = data.KhachHangs.First(n => n.ID_KH == id);
            var hoten = collection["HoTen_KH"];
            var sdt = collection["SDT_KH"];
            var diachi = collection["DiaChi_KH"];
            var taikhoan = collection["Account_KH"];
            var pass = collection["Password_KH"];
            khachhang.ID_KH = id;

            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Chưa nhập họ tên!";
            }
            if (string.IsNullOrEmpty(sdt))
            {
                ViewData["Loi2"] = "Chưa nhập số điện thoại!";
            }
            if (string.IsNullOrEmpty(diachi))
            {
                ViewData["Loi3"] = "Chưa nhập địa chỉ!";
            }
            if (string.IsNullOrEmpty(taikhoan))
            {
                ViewData["Loi4"] = "Chưa nhập tài khoản!";
            }
            if (string.IsNullOrEmpty(pass))
            {
                ViewData["Loi4"] = "Chưa nhập PassWord!";
            }
            else
            {
                khachhang.HoTen_KH = hoten;
                khachhang.SDT_KH = sdt;
                khachhang.DiaChi_KH = diachi;
                khachhang.Account_KH = taikhoan;
                khachhang.Password_KH = pass;
                //Update trong CSDL
                UpdateModel(khachhang);
                data.SubmitChanges();
                return RedirectToAction("Signin");
            }
            return this.Edit(id);
        }
        public ActionResult DonHangCaNhan()
        {
            KhachHang kh = new KhachHang();
            if (Session["Account_KH"] != null)
                kh = (KhachHang)Session["Account_KH"];
            else
                return RedirectToAction("Signin", "Accountuser");

            var dsPDP = data.DonHangs.Where(t => t.ID_KH == kh.ID_KH).ToList();
            return View(dsPDP);
        }
        public ActionResult CTDonHang(int id)
        {

            CT_DonHang ct = data.CT_DonHangs.SingleOrDefault(n => n.ID_DonHang == id);
            return View(ct);
        }
        public ActionResult LichHen()
        {
            KhachHang kh = new KhachHang();
            if (Session["Account_KH"] != null)
                kh = (KhachHang)Session["Account_KH"];
            else
                return RedirectToAction("Signin", "Accountuser");

            var LH = data.DatLiches.Where(t => t.ID_KH == kh.ID_KH).ToList();
            return View(LH);
        }


    }
}