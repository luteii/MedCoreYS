USE HASTANEYONETIM_DB;
GO

-- 1. sp_BekleyenTahlilleriGetir
IF OBJECT_ID('sp_BekleyenTahlilleriGetir', 'P') IS NOT NULL DROP PROCEDURE sp_BekleyenTahlilleriGetir;
GO
CREATE PROCEDURE sp_BekleyenTahlilleriGetir
AS
BEGIN
    SELECT 
        ts.sonuc_id,
        k.ad_soyad AS HastaAdi,
        t.tahlil_adi AS TahlilAdi,
        ts.tahlil_sonuclari_tarih AS IstenmeTarihi,
        d_k.ad_soyad AS DoktorAdi
    FROM TahlilSonuclari ts
    JOIN Kullanicilar k ON ts.hasta_ID = k.kullanici_id
    JOIN Tahliller t ON ts.tahlil_ID = t.tahlil_id
    JOIN Doktorlar d ON ts.doktor_ID = d.doktor_id
    JOIN Kullanicilar d_k ON d.kullanici_ID = d_k.kullanici_id
    WHERE ts.sonuc_degeri = 'Sonuç Bekleniyor...'
    ORDER BY ts.tahlil_sonuclari_tarih ASC;
END
GO

-- 2. sp_TahlilSonucuGuncelle
IF OBJECT_ID('sp_TahlilSonucuGuncelle', 'P') IS NOT NULL DROP PROCEDURE sp_TahlilSonucuGuncelle;
GO
CREATE PROCEDURE sp_TahlilSonucuGuncelle
    @sonuc_id INT,
    @sonuc_degeri VARCHAR(MAX)
AS
BEGIN
    UPDATE TahlilSonuclari
    SET sonuc_degeri = @sonuc_degeri,
        tahlil_sonuclari_tarih = GETDATE()
    WHERE sonuc_id = @sonuc_id;
END
GO

-- 3. Insert Test Laborant User
IF NOT EXISTS (SELECT 1 FROM Kullanicilar WHERE tc_no = '11111111111')
BEGIN
    INSERT INTO Kullanicilar (ad_soyad, tc_no, sifre, rol_ID, son_giris_tarihi, hesap_aktif_mi)
    VALUES ('Test Laborant', '11111111111', '0000', 5, GETDATE(), 1);
END
GO
