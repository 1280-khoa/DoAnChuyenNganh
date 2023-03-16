using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN.Models
{
    public class Cart
    {
        DBSHOPDataContext data = new DBSHOPDataContext();
        public int ID_SanPham { get; set; }
        public string TenSanPham { get; set; }
        public string Image { get; set; }
        public int dDongia { set; get; }
        public int iSoluong { set; get; }
        public int dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Cart(int Masp)
        {
            ID_SanPham = Masp;
            SanPham giay = data.SanPhams.Single(n => n.ID_SanPham == Masp);
            TenSanPham = giay.TenSanPham;
            Image = giay.Image;

            dDongia = int.Parse(giay.Gia_SanPham.ToString());
            iSoluong = 1;
        }



    }
}