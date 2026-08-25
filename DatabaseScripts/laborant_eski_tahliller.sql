USE HASTANEYONETIM_DB;
GO

CREATE PROCEDURE sp_TamamlanmisTahlilleriGetir
AS
BEGIN
    SELECT 
        ts.sonuc_id,
        k.ad_soyad AS HastaAdi,
        t.tahlil_adi AS TahlilAdi,
        ts.tahlil_sonuclari_tarih AS IstenmeTarihi,
        d_k.ad_soyad AS DoktorAdi,
        t.referans_araligi AS ReferansAraligi,
        ts.sonuc_degeri AS MevcutSonuc
    FROM TahlilSonuclari ts
    JOIN Kullanicilar k ON ts.hasta_ID = k.kullanici_id
    JOIN Tahliller t ON ts.tahlil_ID = t.tahlil_id
    JOIN Doktorlar d ON ts.doktor_ID = d.doktor_id
    JOIN Kullanicilar d_k ON d.kullanici_ID = d_k.kullanici_id
    WHERE ts.sonuc_degeri != 'Sonuç Bekleniyor...'
    ORDER BY ts.tahlil_sonuclari_tarih DESC;
END
GO
