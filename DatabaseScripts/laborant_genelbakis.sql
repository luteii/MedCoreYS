USE HASTANEYONETIM_DB;
GO

-- 1. Ozet Bilgiler
CREATE PROCEDURE sp_LaborantGenelBakisOzet
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM TahlilSonuclari WHERE sonuc_degeri = 'Sonuç Bekleniyor...') AS BekleyenSayisi,
        (SELECT COUNT(*) FROM TahlilSonuclari WHERE sonuc_degeri != 'Sonuç Bekleniyor...' AND CAST(tahlil_sonuclari_tarih AS DATE) = CAST(GETDATE() AS DATE)) AS BugunTamamlanan,
        (SELECT COUNT(*) FROM TahlilSonuclari WHERE sonuc_degeri != 'Sonuç Bekleniyor...') AS ToplamTamamlanan
END
GO

-- 2. Tahlil Dagilimi (Pasta Grafik)
CREATE PROCEDURE sp_LaborantGenelBakisTahlilDagilimi
AS
BEGIN
    SELECT 
        t.tahlil_adi AS TahlilTuru,
        COUNT(ts.sonuc_id) AS Miktar
    FROM TahlilSonuclari ts
    JOIN Tahliller t ON ts.tahlil_ID = t.tahlil_id
    GROUP BY t.tahlil_adi
END
GO

-- 3. Haftalik Gidisat (Sütun Grafik)
CREATE PROCEDURE sp_LaborantGenelBakisHaftalikGidisat
AS
BEGIN
    SELECT TOP 7
        CAST(tahlil_sonuclari_tarih AS DATE) AS Tarih,
        COUNT(sonuc_id) AS IslemSayisi
    FROM TahlilSonuclari
    GROUP BY CAST(tahlil_sonuclari_tarih AS DATE)
    ORDER BY Tarih DESC
END
GO
