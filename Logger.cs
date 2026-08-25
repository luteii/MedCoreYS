using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HastaneYonetim
{
    public static class Logger
    {
        public static void Log(string islemTipi, string aciklama, int? kullaniciId = null)
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@islem_tipi", islemTipi },
                    { "@aciklama", aciklama }
                };

                if (kullaniciId.HasValue)
                {
                    parameters.Add("@kullanici_id", kullaniciId.Value);
                }
                else
                {
                    parameters.Add("@kullanici_id", DBNull.Value);
                }

                db.ExecuteNonQuery("sp_LogEkle", parameters);
            }
            catch (Exception ex)
            {
                // Hata durumunda uygulamanın çökmesini önlemek için hata yutulabilir 
                // veya event viewer / yerel dosyaya yazdırılabilir.
                Console.WriteLine("Loglama hatası: " + ex.Message);
            }
        }
    }
}
