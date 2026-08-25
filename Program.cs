using System;
using System.Windows.Forms;

namespace HastaneYonetim
{
    public static class Program
    {
        // ==========================================
        // 1. SİSTEM HAFIZASI (GLOBAL DEĞİŞKENLER)
        // ==========================================
        public static int AktifKullaniciID;
        public static string AktifKullaniciAdSoyad;
        public static int AktifDoktorID; // Bütün doktorlarda aynı hastanın çıkmasını engelleyen anahtar değişkenimiz!

        [STAThread]
        static void Main()
        {
            // ==========================================
            // 2. GÖRÜNTÜ VE ÇÖZÜNÜRLÜK (DPI) AYARLARI
            // ==========================================
            // Ekranların bulanıklaşmasını engeller, cam gibi net ve modern görünümü korur
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ==========================================
            // 3. BAŞLANGIÇ FORMU
            // ==========================================
            Application.Run(new LoginForm());
        }
    }
}