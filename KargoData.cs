using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace cargo_tracker
{
public static class KargoData
{
    public static bool KodKaydet(string mail, string kod)
    {
        using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
        {
            conn.Open();
            string query = "UPDATE Kullanicilar SET OnayKodu = @kod WHERE mail = @mail";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@kod", kod);
            cmd.Parameters.AddWithValue("@mail", mail);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
        public static bool KodKaydet1(string mail, string kod)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                conn.Open();
                string query = "UPDATE personeller SET onaykodu = @kod WHERE eposta = @mail";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kod", kod);
                cmd.Parameters.AddWithValue("@mail", mail);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool KodDogrula(string mail, string girilenKod)
    {
        using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM Kullanicilar WHERE Mail = @mail AND OnayKodu = @kod";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@mail", mail);
            cmd.Parameters.AddWithValue("@kod", girilenKod);

            int sonuc = Convert.ToInt32(cmd.ExecuteScalar());
            return sonuc > 0;
        }
    }
        public static bool KodDogrula1(string mail, string girilenKod)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM personeller WHERE eposta = @mail AND onaykodu = @kod";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mail", mail);
                cmd.Parameters.AddWithValue("@kod", girilenKod);

                int sonuc = Convert.ToInt32(cmd.ExecuteScalar());
                return sonuc > 0;
            }
        }
        public static string MevcutSifreGetir(string mail)
        {
            string sifre = "";
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            string query = "SELECT sifre FROM kullanicilar WHERE mail = @mail";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mail", mail);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null) sifre = result.ToString();
                }
            }
            return sifre;
        }
        public static string MevcutSifreGetir1(string mail)
        {
            string sifre = "";
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            string query = "SELECT sifre FROM personeller WHERE eposta = @eposta";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@eposta", mail);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null) sifre = result.ToString();
                }
            }
            return sifre;
        }

        public static bool SifreGuncelle(string mail, string yeniSifre)
        {
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            string query = "UPDATE kullanicilar SET Sifre = @sifre, OnayKodu = NULL WHERE mail = @mail";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sifre", yeniSifre);
                    cmd.Parameters.AddWithValue("@mail", mail);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public static bool SifreGuncelle1(string mail, string yeniSifre)
        {
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            string query = "UPDATE personeller SET sifre = @sifre, onaykodu = NULL WHERE eposta = @eposta";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sifre", yeniSifre);
                    cmd.Parameters.AddWithValue("@eposta", mail);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static Dictionary<string, Kargo> Kargolar = new Dictionary<string, Kargo>();
        static Random rnd = new Random();

        public static string RastgeleTakipNoUret()
        {
            string no;
            do
            {
                no = rnd.Next(100000, 999999).ToString();
            }
            while (Kargolar.ContainsKey(no));
            return no;
        }
    }
}