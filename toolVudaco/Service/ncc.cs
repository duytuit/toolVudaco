using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quản_lý_vudaco.Service
{
    public class ncc : IDisposable
    {
        //  <add name="project" connectionString="server=103.226.249.227\sqlexpress;database=vua45987_vudaco;uid=vua45987_vudaco;pwd=0l7w7fJ*7" providerName="System.Data.SqlClient" />
        //vudaco_model context = new vudaco_model();
        clsKetNoi cls = new clsKetNoi();
        public DataTable CongNoChiTietNhaCungCap(DateTime TuNgay, DateTime DenNgay, string MaNhaCungCap)
        {
            DataTable dt = new DataTable("CongNo");
            dt.Columns.Add("Ngay", typeof(DateTime));
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("SoFile");
            dt.Columns.Add("MaDieuXe");
            dt.Columns.Add("TuyenVC");
            dt.Columns.Add("SoHoaDon");
            dt.Columns.Add("No_VanChuyen", typeof(double));
            dt.Columns.Add("No_NangHa", typeof(double));
            dt.Columns.Add("No_Tong", typeof(double));
            dt.Columns.Add("Thu_VanChuyen", typeof(double));
            dt.Columns.Add("Thu_NangHa", typeof(double));
            dt.Columns.Add("Thu_Tong", typeof(double));
            dt.Columns.Add("NoCuoiKi_VanChuyen", typeof(double));
            dt.Columns.Add("NoCuoiKi_NangHa", typeof(double));
            dt.Columns.Add("NoCuoiKi", typeof(double));
            dt.Columns.Add("SoThu_VanChuyen", typeof(double));
            dt.Columns.Add("SoThu_NangHa", typeof(double));
            //// dt.Columns.Add("LoaiChungTu");
            ////  dt.Columns.Add("Loai");
            double SoTien = 0, KhongFile_CuocBan = 0, KhongFile_ThanhTien = 0, ThuTien = 0, BangPhoiNangHa_ChiHo = 0, KhongFile_CuocMua = 0;
            double DauKi_DichVu = 0, DauKi_ChiHo = 0;
            double Tong_DauKi_DichVu = 0, Tong_DauKi_ChiHo = 0, TraTien = 0, PhiCom = 0;

            string _MaDieuXe = "";
            //theo so file
            DataTable dtF = cls.LoadTable("select distinct IDLoHang,SoFile from FileGia A left join FileGiaChiTiet B on A.IdGia=B.IDGia where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)<=0");
            string sqlFile = "select distinct IDLoHang,(select top 1 SoFile from ThongTinFile where IDLoHang=BangPhoiNangHa.IDLoHang)as SoFile from BangPhoiNangHa where IDLoHang in(select IDLoHang from BangPhoiNangHa_ChiTiet where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "') and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTaoBangKe)<=0";
            DataTable dtFile = cls.LoadTable(sqlFile);
            dtFile.Merge(dtF);
            for (int i = 0; i < dtFile.Rows.Count; i++)
            {
                SoTien = 0; KhongFile_CuocBan = 0; KhongFile_ThanhTien = 0; ThuTien = 0; BangPhoiNangHa_ChiHo = 0; KhongFile_CuocMua = 0;
                TraTien = 0;
                DataRow row1 = dtFile.Rows[i];
                DataRow row = dt.NewRow();
                row["Chon"] = false;
                string tk = row1["SoFile"].ToString();
                row["SoFile"] = tk;
                // row["Ngay"] = Convert.ToDateTime(row1["NgayTaoBangKe"].ToString());
                //ngay lay theo file debit (27/11/2024)
                DataTable t_ngay = cls.LoadTable("SELECT ThoiGianLap FROM FileDebit WHERE SoFile = '" + tk + "'");

                foreach (DataRow item_t_ngay in t_ngay.Rows)
                {
                    row["Ngay"] = item_t_ngay["ThoiGianLap"]; // hoặc item.Field<DateTime>("ThoiGianLap")
                }

                row["SoHoaDon"] = "";//để code sau
                KhongFile_CuocMua = 0;
                KhongFile_ThanhTien = 0;
                double CuocMua = 0;
                string s = "select isnull(sum(GiaMua),0) from FileGiaChiTiet A left join FileGia B on A.IDGia=B.IDGia  where MaNhaCungCap=N'" + MaNhaCungCap.Trim() + "' and  SoFile ='" + tk + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)<=0";
                DataTable dts = cls.LoadTable(s);
                if (dts.Rows.Count > 0)
                    CuocMua = double.Parse(dts.Rows[0][0].ToString());
                else
                    CuocMua = 0;
                row["No_VanChuyen"] = KhongFile_CuocBan + KhongFile_ThanhTien + CuocMua;
                #region no nang ha
                string sql1 = @"select isnull(sum(SoTien_ChiHo),0) from BangPhoiNangHa_ChiTiet A left
                             join BangPhoiNangHa B on A.IDLoHang = B.IDLoHang  where A.MaChiHo in ('CH06','CH07','CH08','CH09','CH12','CH15') and  B.IDLoHang='" + row1["IDLoHang"].ToString().Trim() + "' and A.MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayTaoBangKe)<=0";
                DataTable dt1 = cls.LoadTable(sql1);
                if (dt1.Rows.Count > 0)
                    SoTien = double.Parse(dt1.Rows[0][0].ToString());
                else
                    SoTien = 0;
                row["No_NangHa"] = SoTien;
                row["No_Tong"] = double.Parse(row["No_VanChuyen"].ToString()) + double.Parse(row["No_NangHa"].ToString());
                #endregion
                #region chi tien nha cung cap VanChuyen

                string sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.SoFile='" + row1["SoFile"].ToString().Trim() + "' and A.LaVanChuyen=1 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0";
                DataTable dt2 = cls.LoadTable(sql2);
                if (dt2.Rows.Count > 0)
                    TraTien = double.Parse(dt2.Rows[0][0].ToString());
                else
                    TraTien = 0;
                row["Thu_VanChuyen"] = TraTien;
                #endregion
                #region chi tien nha cung cap NangHa
                sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                         join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where    A.SoFile='" + row1["SoFile"].ToString().Trim() + "' and  A.LaVanChuyen=0 and B.MaChi in ('006','007','008','009','012') and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0";
                dt2 = cls.LoadTable(sql2);
                if (dt2.Rows.Count > 0)
                    TraTien = double.Parse(dt2.Rows[0][0].ToString());
                else
                    TraTien = 0;
                row["Thu_NangHa"] = TraTien;
                row["Thu_Tong"] = double.Parse(row["Thu_VanChuyen"].ToString()) + double.Parse(row["Thu_NangHa"].ToString());
                #endregion
                row["NoCuoiKi_VanChuyen"] = double.Parse(row["No_VanChuyen"].ToString()) - double.Parse(row["Thu_VanChuyen"].ToString());
                row["NoCuoiKi_NangHa"] = double.Parse(row["No_NangHa"].ToString()) - double.Parse(row["Thu_NangHa"].ToString());
                row["NoCuoiKi"] = double.Parse(row["No_Tong"].ToString()) - double.Parse(row["Thu_Tong"].ToString());
                dt.Rows.Add(row);
            }

            //theo mã điều xe(MaDieuXe)
            string sqlFile2 = "select Ngay,MaDieuXe,TuyenVC from BangDieuXe where  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',Ngay)<=0";
            string sql_NgayKhac = @"select NgayTao as Ngay,MaDieuXe,TenDichVu as TuyenVC from FileDebit_KoHoaDon_KH  where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
            DataTable dtFile2 = cls.LoadTable(sqlFile2);
            DataTable dtFile2_Khac = cls.LoadTable(sql_NgayKhac);
            dtFile2.Merge(dtFile2_Khac);
            for (int i = 0; i < dtFile2.Rows.Count; i++)
            {
                DataRow row1 = dtFile2.Rows[i];
                DataRow row = dt.NewRow();
                row["Chon"] = false;
                row["Ngay"] = Convert.ToDateTime(dtFile2.Rows[i]["Ngay"].ToString());
                row["MaDieuXe"] = dtFile2.Rows[i]["MaDieuXe"].ToString();
                row["TuyenVC"] = dtFile2.Rows[i]["TuyenVC"].ToString();
                row["SoFile"] = "";//dtFile2.Rows[i]["SoFile"].ToString();
                DataTable t_sohd = cls.LoadTable("SELECT SoHoaDon FROM FileDebit_KoHoaDon_NCC WHERE MaDieuXe = '" + dtFile2.Rows[i]["MaDieuXe"].ToString() + "'");
                foreach (DataRow item_t_sohd in t_sohd.Rows)
                {
                    row["SoHoaDon"] = item_t_sohd["SoHoaDon"];//để code sau
                }

                #region no van chuyen
                string sql1 = "";
                sql1 = @"select isnull(sum(CuocMua),0) from BangDieuXe  where MaDieuXe not in(select MaDieuXe from FileDebit_KoHoaDon_NCC where MaNhaCungCap=N'" + MaNhaCungCap.Trim() + "') and  MaDieuXe ='" + row1["MaDieuXe"].ToString().Trim() + "' and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',Ngay)<=0";
                DataTable dt11 = cls.LoadTable(sql1);
                if (dt11.Rows.Count > 0)
                    KhongFile_CuocMua = double.Parse(dt11.Rows[0][0].ToString());
                else
                    KhongFile_CuocMua = 0;

                sql1 = @"select isnull(sum(ThanhTien),0) from FileDebit_KoHoaDon_NCC  where MaDieuXe='" + row1["MaDieuXe"].ToString().Trim() + "' and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
                DataTable dt12 = cls.LoadTable(sql1);
                if (dt12.Rows.Count > 0)
                    KhongFile_ThanhTien = double.Parse(dt12.Rows[0][0].ToString());
                else
                    KhongFile_ThanhTien = 0;
                //
                sql1 = @"select isnull(sum(PhiCom),0) from FileDebit_KoHoaDon_KH  where MaDieuXe='" + row1["MaDieuXe"].ToString().Trim() + "' and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
                dt12 = cls.LoadTable(sql1);
                if (dt12.Rows.Count > 0)
                    PhiCom = double.Parse(dt12.Rows[0][0].ToString());
                else
                    PhiCom = 0;
                //
                row["No_VanChuyen"] = KhongFile_CuocMua + KhongFile_ThanhTien + PhiCom;
                #endregion
                #region no nang ha

                SoTien = 0;
                row["No_NangHa"] = SoTien;
                row["No_Tong"] = double.Parse(row["No_VanChuyen"].ToString()) + double.Parse(row["No_NangHa"].ToString());
                #endregion
                #region chi tien nha cung cap VanChuyen
                string sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.MaDieuXe='" + row1["MaDieuXe"].ToString().Trim() + "' and A.LaVanChuyen=1 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0";
                DataTable dt2 = cls.LoadTable(sql2);
                if (dt2.Rows.Count > 0)
                    TraTien = double.Parse(dt2.Rows[0][0].ToString());
                else
                    TraTien = 0;
                row["Thu_VanChuyen"] = TraTien;
                #endregion
                #region chi tien nha cung cap NangHa
                sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where   A.MaDieuXe='" + row1["MaDieuXe"].ToString().Trim() + "' and  A.LaVanChuyen=0 and B.MaChi in('006','007','008','009','012') and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0";
                dt2 = cls.LoadTable(sql2);
                if (dt2.Rows.Count > 0)
                    TraTien = double.Parse(dt2.Rows[0][0].ToString());
                else
                    TraTien = 0;
                row["Thu_NangHa"] = TraTien;
                row["Thu_Tong"] = double.Parse(row["Thu_VanChuyen"].ToString()) + double.Parse(row["Thu_NangHa"].ToString());
                #endregion
                row["NoCuoiKi_VanChuyen"] = double.Parse(row["No_VanChuyen"].ToString()) - double.Parse(row["Thu_VanChuyen"].ToString());
                row["NoCuoiKi_NangHa"] = double.Parse(row["No_NangHa"].ToString()) - double.Parse(row["Thu_NangHa"].ToString());
                row["NoCuoiKi"] = double.Parse(row["No_Tong"].ToString()) - double.Parse(row["Thu_Tong"].ToString());
                dt.Rows.Add(row);
            }
            DataView view = dt.DefaultView;
            //// view.RowFilter = "NoCuoiKi>0";
            DataView view1 = view.ToTable().Copy().DefaultView;
            view1.Sort = "Ngay asc";
            dt = view1.ToTable();
            dt.TableName = "CongNo";

            return dt;
        }
        public DataTable CongNoChiTietNhaCungCap_In(DateTime TuNgay, DateTime DenNgay, string MaNhaCungCap)
        {
            DataTable dt = new DataTable("CongNo");
            dt.Columns.Add("Ngay", typeof(DateTime));
            dt.Columns.Add("LoaiXe_NCC");
            dt.Columns.Add("TuyenVC");
            dt.Columns.Add("SoFile");
            dt.Columns.Add("NoiDung");
            dt.Columns.Add("SoTien", typeof(double));
            dt.Columns.Add("TienVAT", typeof(double));
            dt.Columns.Add("ThanhTien", typeof(double));
            dt.Columns.Add("BienSoXe");
            dt.Columns.Add("SoCont");
            dt.Columns.Add("SoHoaDon");
            dt.Columns.Add("PhiNang", typeof(double));
            dt.Columns.Add("PhiHa", typeof(double));
            dt.Columns.Add("PhiNangHa", typeof(double));
            dt.Columns.Add("PhiCSHT", typeof(double));
            dt.Columns.Add("PhiKhac", typeof(double));
            dt.Columns.Add("PhieuTamThu", typeof(double));
            double SoTien = 0, KhongFile_CuocBan = 0, KhongFile_ThanhTien = 0, ThuTien = 0, BangPhoiNangHa_ChiHo = 0, KhongFile_CuocMua = 0;
            double DauKi_DichVu = 0, DauKi_ChiHo = 0;
            double Tong_DauKi_DichVu = 0, Tong_DauKi_ChiHo = 0, TraTien = 0, PhiCom = 0;
            //theo so file
            DataTable dtF = cls.LoadTable("select distinct IDLoHang,SoFile from FileGia A left join FileGiaChiTiet B on A.IdGia=B.IDGia where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "'  and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)<=0");
            // string sqlFile = "select distinct IDLoHang,(select top 1 SoFile from ThongTinFile where IDLoHang=BangPhoiNangHa.IDLoHang)as SoFile from BangPhoiNangHa where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "'  and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',NgayTaoBangKe)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTaoBangKe)<=0";
            string sqlFile = "select distinct IDLoHang,(select top 1 SoFile from ThongTinFile where IDLoHang=BangPhoiNangHa.IDLoHang)as SoFile from BangPhoiNangHa where IDLoHang in(select IDLoHang from BangPhoiNangHa_ChiTiet where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "') and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTaoBangKe)<=0";
            DataTable dtFile = cls.LoadTable(sqlFile);
            DataView viewTong = dtFile.Copy().DefaultView;
            for (int i = 0; i < dtF.Rows.Count; i++)
            {
                viewTong.RowFilter = "IDLoHang='" + dtF.Rows[i]["IDLoHang"].ToString().Trim() + "'";
                if (viewTong.ToTable().Rows.Count == 0)
                {
                    DataRow row = dtFile.NewRow();
                    row["IDLoHang"] = dtF.Rows[i]["IDLoHang"].ToString().Trim();
                    row["SoFile"] = dtF.Rows[i]["SoFile"].ToString().Trim();
                    dtFile.Rows.Add(row);
                }
            }
            dtFile.Merge(dtF);
            for (int i = 0; i < dtFile.Rows.Count; i++)
            {
                SoTien = 0; KhongFile_CuocBan = 0; KhongFile_ThanhTien = 0; ThuTien = 0; BangPhoiNangHa_ChiHo = 0; KhongFile_CuocMua = 0;
                TraTien = 0;
                DataRow row1 = dtFile.Rows[i];
                string tk = row1["SoFile"].ToString();

                // row["Ngay"] = Convert.ToDateTime(row1["NgayTaoBangKe"].ToString());
                //ngay lay theo file debit (27/11/2024)
                string _LoaiXe_NCC = "", _TuyenVC = "", _BienSoXe = "", _SoCont = "";

                KhongFile_CuocMua = 0;
                KhongFile_ThanhTien = 0;
                double CuocMua = 0;
                string s = "select * from FileGiaChiTiet A,FileGia B where B.SoFile='" + tk + "' and A.IDGia=B.IDGia and MaNhaCungCap=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)<=0";
                DataTable dts = cls.LoadTable(s);
                for (int k = 0; k < dts.Rows.Count; k++)
                {
                    DataTable t = cls.LoadTable(
                         "SELECT TOP 1 LoaiXe_NCC, TuyenVC, BienSoXe FROM BangDieuXe WHERE SoFile = @sofile",
                         new[] { "@sofile" },
                         new object[] { tk },
                         1
                     );

                    if (t.Rows.Count > 0)
                    {
                        DataRow row_t = t.Rows[0];
                        _LoaiXe_NCC = row_t["LoaiXe_NCC"].ToString();
                        _TuyenVC = row_t["TuyenVC"].ToString();
                        _BienSoXe = row_t["BienSoXe"].ToString();
                    }
                    DataTable t1 = cls.LoadTable(
                         "SELECT SoCont FROM ThongTinFile WHERE SoFile = @sofile",
                         new[] { "@sofile" },
                         new object[] { tk },
                         1
                     );

                    foreach (DataRow row_t1 in t1.Rows)
                    {
                        _SoCont = row_t1["SoCont"].ToString();
                    }
                    DataRow row = dt.NewRow();
                    row["Ngay"] = DateTime.Parse(dts.Rows[k]["ThoiGianLap"].ToString());
                    row["SoFile"] = tk;
                    row["LoaiXe_NCC"] = _LoaiXe_NCC;
                    row["TuyenVC"] = _TuyenVC;
                    row["NoiDung"] = dts.Rows[k]["TenDichVu"].ToString();
                    row["SoTien"] = double.Parse(dts.Rows[k]["GiaMua"].ToString());
                    row["TienVAT"] = 0;
                    row["ThanhTien"] = double.Parse(dts.Rows[k]["GiaMua"].ToString());
                    row["BienSoXe"] = _BienSoXe;
                    row["SoCont"] = _SoCont;
                    row["PhiNang"] = 0;
                    row["PhiHa"] = 0;
                    row["PhiNangHa"] = 0;
                    row["PhiCSHT"] = 0;
                    row["PhiKhac"] = 0;
                    row["PhieuTamThu"] = 0;
                    #region show cac phi o bang phoi nang ha

                    #endregion
                    dt.Rows.Add(row);

                }
                #region no nang ha
                s = @"select B.*,A.SoFile from  BangPhoiNangHa B
                      left join ThongTinFile A on A.IDLoHang=B.IDLoHang
                     where A.SoFile='" + tk + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayTaoBangKe)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayTaoBangKe)<=0";
                dts = cls.LoadTable(s);
                // DataView viewCon = dts.Copy().DefaultView;
                for (int k = 0; k < dts.Rows.Count; k++)
                {
                    int _IDLoHang = int.Parse(dts.Rows[k]["IDLoHang"].ToString());
                    // 1. Kiểm tra tồn tại dữ liệu BangPhoiNangHa_ChiTiet
                    DataTable tCheck = cls.LoadTable(
                        "SELECT TOP 1 * FROM BangPhoiNangHa_ChiTiet WHERE IDLoHang = @idLoHang AND MaNhaCungCap = @maNCC",
                        new[] { "@idLoHang", "@maNCC" },
                        new object[] { _IDLoHang, MaNhaCungCap },
                        2
                    );

                    if (tCheck.Rows.Count > 0)
                    {
                        // 2. Lấy dữ liệu từ BangDieuXe
                        DataTable t = cls.LoadTable(
                            "SELECT TOP 1 LoaiXe_NCC, TuyenVC, BienSoXe FROM BangDieuXe WHERE SoFile = @sofile",
                            new[] { "@sofile" },
                            new object[] { tk },
                            1
                        );

                        if (t.Rows.Count > 0)
                        {
                            _LoaiXe_NCC = t.Rows[0]["LoaiXe_NCC"].ToString();
                            _TuyenVC = t.Rows[0]["TuyenVC"].ToString();
                            _BienSoXe = t.Rows[0]["BienSoXe"].ToString();
                        }

                        // 3. Lấy dữ liệu từ ThongTinFile
                        DataTable t1 = cls.LoadTable(
                            "SELECT TOP 1 SoCont FROM ThongTinFile WHERE SoFile = @sofile",
                            new[] { "@sofile" },
                            new object[] { tk },
                            1
                        );

                        if (t1.Rows.Count > 0)
                        {
                            _SoCont = t1.Rows[0]["SoCont"].ToString();
                        }

                        // 4. Tạo dòng mới trong DataTable dt
                        DataRow row = dt.NewRow();
                        row["Ngay"] = DateTime.Parse(dts.Rows[k]["NgayTaoBangKe"].ToString());
                        row["SoFile"] = tk;
                        row["LoaiXe_NCC"] = _LoaiXe_NCC;
                        row["TuyenVC"] = _TuyenVC;
                        row["NoiDung"] = _TuyenVC;
                        row["SoTien"] = 0;
                        row["TienVAT"] = 0;
                        row["ThanhTien"] = 0;
                        row["BienSoXe"] = _BienSoXe;
                        row["SoCont"] = _SoCont;

                        string idLoHangStr = dts.Rows[k]["IDLoHang"].ToString().Trim();

                        // 5. Các phí liên quan
                        row["PhiNang"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH06' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        row["PhiHa"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH07' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        row["PhiNangHa"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH08' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        row["PhiCSHT"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH09' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        row["PhiKhac"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH12' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        row["PhieuTamThu"] = cls.LoadTable("SELECT ISNULL(SUM(SoTien_ChiHo), 0) FROM BangPhoiNangHa_ChiTiet WHERE MaChiHo='CH15' AND IDLoHang=@id AND MaNhaCungCap=@ncc",
                            new[] { "@id", "@ncc" }, new object[] { idLoHangStr, MaNhaCungCap }, 2).Rows[0][0].ToString();

                        dt.Rows.Add(row);
                    }


                }
                #endregion

            }

            //theo mã điều xe(MaDieuXe)
            string sqlFile2 = "select Ngay,MaDieuXe,TuyenVC from BangDieuXe where  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "'  and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',Ngay)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',Ngay)<=0";
            string sql_NgayKhac = @"select NgayTao as Ngay,MaDieuXe,TenDichVu as TuyenVC from FileDebit_KoHoaDon_KH  where MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',NgayTao)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
            DataTable dtFile2 = cls.LoadTable(sqlFile2);
            DataTable dtFile2_Khac = cls.LoadTable(sql_NgayKhac);
            dtFile2.Merge(dtFile2_Khac);
            for (int i = 0; i < dtFile2.Rows.Count; i++)
            {
                DataRow row1 = dtFile2.Rows[i];
                string tk = row1["MaDieuXe"].ToString();

                #region no van chuyen
                string sql1 = @"SELECT * 
                FROM BangDieuXe 
                WHERE MaDieuXe NOT IN (
                    SELECT MaDieuXe FROM FileDebit_KoHoaDon_NCC WHERE MaNhaCungCap = N'" + MaNhaCungCap.Trim() + @"'
                ) 
                AND MaDieuXe = '" + tk.Trim() + @"' 
                AND MaNhaCungCap = N'" + MaNhaCungCap.Trim() + @"' 
                AND DATEDIFF(day, '" + TuNgay.ToString("yyyy-MM-dd") + @"', Ngay) >= 0 
                AND DATEDIFF(day, '" + DenNgay.ToString("yyyy-MM-dd") + @"', Ngay) <= 0";

                DataTable dts = cls.LoadTable(sql1);

                for (int k = 0; k < dts.Rows.Count; k++)
                {
                    string _f = dts.Rows[k]["SoFile"].ToString().Trim();
                    string file = _f;
                    string _LoaiXe_NCC = "", _TuyenVC = "", _BienSoXe = "", _SoCont = "", _SoHoaDon = "";

                    if (!string.IsNullOrEmpty(_f))
                    {
                        // Lấy thông tin từ BangDieuXe
                        string sql_dieuxe = "SELECT TOP 1 LoaiXe_NCC, TuyenVC, BienSoXe FROM BangDieuXe WHERE SoFile = '" + _f + "'";
                        DataTable t = cls.LoadTable(sql_dieuxe);
                        if (t.Rows.Count > 0)
                        {
                            _LoaiXe_NCC = t.Rows[0]["LoaiXe_NCC"].ToString();
                            _TuyenVC = t.Rows[0]["TuyenVC"].ToString();
                            _BienSoXe = t.Rows[0]["BienSoXe"].ToString();
                        }

                        // Lấy SoCont từ ThongTinFiles
                        string sql_ttfile = "SELECT TOP 1 SoCont FROM ThongTinFiles WHERE SoFile = '" + _f + "'";
                        DataTable t1 = cls.LoadTable(sql_ttfile);
                        if (t1.Rows.Count > 0)
                        {
                            _SoCont = t1.Rows[0]["SoCont"].ToString();
                        }
                    }

                    // Lấy SoHoaDon từ FileDebit_KoHoaDon_NCC
                    string sql_sohd = "SELECT TOP 1 SoHoaDon FROM FileDebit_KoHoaDon_NCC WHERE MaDieuXe = '" + tk + "'";
                    DataTable t_sohd = cls.LoadTable(sql_sohd);
                    if (t_sohd.Rows.Count > 0)
                    {
                        _SoHoaDon = t_sohd.Rows[0]["SoHoaDon"].ToString();
                    }

                    // Thêm vào datatable
                    DataRow row = dt.NewRow();
                    row["Ngay"] = DateTime.Parse(dts.Rows[k]["Ngay"].ToString());
                    row["SoFile"] = (!string.IsNullOrEmpty(file)) ? file : tk;
                    row["SoHoaDon"] = _SoHoaDon;
                    row["LoaiXe_NCC"] = _LoaiXe_NCC;
                    row["TuyenVC"] = dts.Rows[k]["TuyenVC"].ToString();
                    row["NoiDung"] = dts.Rows[k]["TuyenVC"].ToString();
                    row["SoTien"] = double.Parse(dts.Rows[k]["CuocMua"].ToString());
                    row["TienVAT"] = 0;
                    row["ThanhTien"] = double.Parse(dts.Rows[k]["CuocMua"].ToString());
                    row["BienSoXe"] = _BienSoXe;
                    row["SoCont"] = _SoCont;
                    row["PhiNang"] = 0;
                    row["PhiHa"] = 0;
                    row["PhiCSHT"] = 0;
                    row["PhiKhac"] = 0;
                    row["PhieuTamThu"] = 0;

                    dt.Rows.Add(row);
                }

                sql1 = @"select * from FileDebit_KoHoaDon_NCC  where MaDieuXe='" + tk.Trim() + "' and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "'  and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',NgayTao)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
                dts = cls.LoadTable(sql1);
                for (int k = 0; k < dts.Rows.Count; k++)
                {
                    string _LoaiXe_NCC = "", _TuyenVC = "", _BienSoXe = "", _SoCont = "";

                    DataRow row = dt.NewRow();
                    row["Ngay"] = DateTime.Parse(dts.Rows[k]["NgayTao"].ToString());
                    row["SoFile"] = tk;
                    row["LoaiXe_NCC"] = _LoaiXe_NCC;
                    row["TuyenVC"] = dts.Rows[k]["TuyenVC"].ToString();
                    row["NoiDung"] = dts.Rows[k]["TuyenVC"].ToString();
                    row["SoTien"] = double.Parse(dts.Rows[k]["SoTien"].ToString());
                    row["TienVAT"] = (double.Parse(dts.Rows[k]["VAT"].ToString()) * double.Parse(dts.Rows[k]["SoTien"].ToString())) / 100;
                    row["ThanhTien"] = double.Parse(dts.Rows[k]["ThanhTien"].ToString());
                    row["BienSoXe"] = _BienSoXe;
                    row["SoCont"] = _SoCont;
                    row["PhiNang"] = 0;
                    row["PhiHa"] = 0;
                    row["PhiCSHT"] = 0;
                    row["PhiKhac"] = 0;
                    row["PhieuTamThu"] = 0;
                    DataTable dtSoHoaDon = cls.LoadTable("SELECT TOP 1 SoHoaDon FROM FileDebit_KoHoaDon_NCC WHERE MaDieuXe = '" + tk + "'");
                    if (dtSoHoaDon.Rows.Count > 0)
                    {
                        row["SoHoaDon"] = dtSoHoaDon.Rows[0]["SoHoaDon"].ToString();
                    }
                    dt.Rows.Add(row);
                }
                //
                sql1 = @"select * from FileDebit_KoHoaDon_KH  where MaDieuXe='" + row1["MaDieuXe"].ToString().Trim() + "' and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',NgayTao)<=0";
                dts = cls.LoadTable(sql1);
                for (int k = 0; k < dts.Rows.Count; k++)
                {
                    string _LoaiXe_NCC = "", _TuyenVC = "", _BienSoXe = "", _SoCont = "";
                    DataRow row = dt.NewRow();
                    row["Ngay"] = DateTime.Parse(dts.Rows[k]["NgayTao"].ToString());
                    row["SoFile"] = tk;
                    row["LoaiXe_NCC"] = dts.Rows[k]["LoaiXe_KH"].ToString();
                    row["TuyenVC"] = dts.Rows[k]["TuyenVC"].ToString();
                    row["NoiDung"] = dts.Rows[k]["TenDichVu"].ToString() + "_Phí com";
                    row["SoTien"] = double.Parse(dts.Rows[k]["PhiCom"].ToString());
                    row["TienVAT"] = 0;
                    row["ThanhTien"] = double.Parse(dts.Rows[k]["PhiCom"].ToString());
                    row["BienSoXe"] = _BienSoXe;
                    row["SoCont"] = _SoCont;
                    row["PhiNang"] = 0;
                    row["PhiHa"] = 0;
                    row["PhiCSHT"] = 0;
                    row["PhiKhac"] = 0;
                    row["PhieuTamThu"] = 0;
                    dt.Rows.Add(row);
                }

                #endregion

            }

            DataView view = dt.DefaultView;
            DataView view1 = view.ToTable().Copy().DefaultView;
            view1.Sort = "SoFile asc";
            dt = view1.ToTable();
            dt.TableName = "CongNo";

            return dt;
        }
        public double DauKy_NCC(string MaNhaCungCap, DateTime TuNgay, DateTime DenNgay)
        {
            double DauKy = 0;
            #region dau ki
            double KhongFile_CuocMua = 0, KhongFile_ThanhTien = 0, PhiCom = 0, CuocMua = 0, TraTien = 0, SoTien = 0, DauKi_VanChuyen = 0, DauKi_NangHa = 0;
            string sql1 = "";
            sql1 = @"select isnull(sum(CuocMua),0) from BangDieuXe  where MaDieuXe not in(select MaDieuXe from FileDebit_KoHoaDon_NCC where MaNhaCungCap=N'" + MaNhaCungCap + "') and  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',Ngay)<0";
            DataTable dt11 = cls.LoadTable(sql1);
            if (dt11.Rows.Count > 0)
                KhongFile_CuocMua = double.Parse(dt11.Rows[0][0].ToString());
            else
                KhongFile_CuocMua = 0;
            sql1 = @"select isnull(sum(ThanhTien),0) from FileDebit_KoHoaDon_NCC  where  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',NgayTao)<0";
            DataTable dt12 = cls.LoadTable(sql1);
            if (dt12.Rows.Count > 0)
                KhongFile_ThanhTien = double.Parse(dt12.Rows[0][0].ToString());
            else
                KhongFile_ThanhTien = 0;
            //
            sql1 = @"select isnull(sum(PhiCom),0) from FileDebit_KoHoaDon_KH  where  MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',NgayTao)<0";
            DataTable dt112 = cls.LoadTable(sql1);
            if (dt112.Rows.Count > 0)
                PhiCom = double.Parse(dt112.Rows[0][0].ToString());
            else
                PhiCom = 0;
            //file gia
            sql1 = @"select isnull(sum(A.GiaMua),0) from FileGiaChiTiet A left join FileGia B on A.IDGia=B.IDGia  where  A.MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',ThoiGianLap)<0";
            DataTable dt122 = cls.LoadTable(sql1);
            if (dt122.Rows.Count > 0)
                CuocMua = double.Parse(dt122.Rows[0][0].ToString());
            else
                CuocMua = 0;
            //
            string sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.LaVanChuyen=1 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<0";
            DataTable dt2 = cls.LoadTable(sql2);
            if (dt2.Rows.Count > 0)
                TraTien = double.Parse(dt2.Rows[0][0].ToString());
            else
                TraTien = 0;
            DauKi_VanChuyen = KhongFile_CuocMua + KhongFile_ThanhTien + CuocMua + PhiCom - TraTien;
            //
            sql1 = @"select isnull(sum(SoTien_ChiHo),0) from BangPhoiNangHa_ChiTiet A left
                             join BangPhoiNangHa B on A.IDLoHang = B.IDLoHang  where A.MaNhaCungCap =N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayTaoBangKe)<0";
            DataTable dt1 = cls.LoadTable(sql1);
            if (dt1.Rows.Count > 0)
                SoTien = double.Parse(dt1.Rows[0][0].ToString());
            else
                SoTien = 0;
            sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.LaVanChuyen=0 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<0";
            dt2 = cls.LoadTable(sql2);
            if (dt2.Rows.Count > 0)
                TraTien = double.Parse(dt2.Rows[0][0].ToString());
            else
                TraTien = 0;

            DauKi_NangHa = SoTien - TraTien;
            #endregion
            DauKy = DauKi_VanChuyen + DauKi_NangHa;
            return DauKy;
        }
        public double ThanhToan_NCC(string MaNhaCungCap, DateTime TuNgay, DateTime DenNgay)
        {
            double TraTien = 0, ThanhToan_VanChuyen = 0, ThanhToan_NangHa = 0;
            #region thanh toan
            string sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.LaVanChuyen=1 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and (DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0)";
            DataTable dt2 = cls.LoadTable(sql2);
            if (dt2.Rows.Count > 0)
                TraTien = double.Parse(dt2.Rows[0][0].ToString());
            else
                TraTien = 0;
            ThanhToan_VanChuyen = TraTien;
            sql2 = @"select isnull(sum(ThanhTien),0) from PhieuChi_NCC_CT A left
                             join PhieuChi_NCC B on A.SoChungTu = B.SoChungTu where  A.LaVanChuyen=0 and B.MaChi='006' and  A.MaDoituong=N'" + MaNhaCungCap.Trim() + "' and (DATEDIFF(day,'" + TuNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)>=0 and DATEDIFF(day,'" + DenNgay.ToString("yyyy-MM-dd") + "',B.NgayHachToan)<=0)";
            dt2 = cls.LoadTable(sql2);
            if (dt2.Rows.Count > 0)
                TraTien = double.Parse(dt2.Rows[0][0].ToString());
            else
                TraTien = 0;
            ThanhToan_NangHa = TraTien;
            #endregion
            return ThanhToan_VanChuyen + ThanhToan_NangHa;
        }
        public void Dispose()
        {
            cls.Dispose(); // nếu clsKetNoi implements IDisposable
        }
    }
}
