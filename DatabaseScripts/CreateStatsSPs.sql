CREATE PROCEDURE sp_Istatistik_BolumSayisi AS BEGIN SELECT COUNT(*) FROM Bolumler; END;
GO
CREATE PROCEDURE sp_Istatistik_RandevuSayisi AS BEGIN SELECT COUNT(*) FROM Randevular; END;
GO
CREATE PROCEDURE sp_Istatistik_BugunRandevu AS BEGIN SELECT COUNT(*) FROM Randevular WHERE CAST(randevu_tarihi AS DATE) = CAST(GETDATE() AS DATE); END;
GO
CREATE PROCEDURE sp_Istatistik_BolumYogunluklari AS BEGIN SELECT TOP 5 bolum_adi, COUNT(randevu_id) as RandevuSayisi FROM Bolumler b LEFT JOIN DoktorBilgileri dbil ON b.bolum_id = dbil.bolum_id LEFT JOIN Randevular r ON dbil.kullanici_id = r.doktor_ID GROUP BY bolum_adi ORDER BY RandevuSayisi DESC; END;
GO
CREATE PROCEDURE sp_Istatistik_SonHareketler AS BEGIN SELECT TOP 5 islem_tipi, islem_tarihi, aciklama FROM Loglar ORDER BY islem_tarihi DESC; END;
GO
