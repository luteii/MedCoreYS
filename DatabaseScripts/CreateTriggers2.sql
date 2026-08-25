-- Tahliller tablosu icin trigger
CREATE OR ALTER TRIGGER trg_TahlilEkle ON Tahliller AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_ISTENDI', GETDATE(), 'Hasta için yeni bir tahlil istendi.');
END
GO

CREATE OR ALTER TRIGGER trg_TahlilSil ON Tahliller AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_IPTALI', GETDATE(), 'Sistemden bir tahlil kaydı silindi.');
END
GO

-- TahlilSonuclari tablosu icin trigger
CREATE OR ALTER TRIGGER trg_TahlilSonucEkle ON TahlilSonuclari AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_SONUCU', GETDATE(), 'Hasta tahlil sonuçları sisteme girildi.');
END
GO

-- Odemeler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_OdemeEkle ON Odemeler AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'ODEME_ALINDI', GETDATE(), 'Hasta için yeni bir ödeme kaydı oluşturuldu.');
END
GO

-- Faturalar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_FaturaEkle ON Faturalar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'FATURA_KESILDI', GETDATE(), 'Hasta için fatura kesildi.');
END
GO

-- Hastalar tablosu guncelleme icin trigger (UPDATE)
CREATE OR ALTER TRIGGER trg_HastaGuncelle ON Hastalar AFTER UPDATE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_GUNCELLEME', GETDATE(), 'Sistemdeki bir hastanın kayıt bilgileri güncellendi.');
END
GO

-- Randevular tablosu guncelleme icin trigger (UPDATE)
CREATE OR ALTER TRIGGER trg_RandevuGuncelle ON Randevular AFTER UPDATE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'RANDEVU_GUNCELLEME', GETDATE(), 'Sistemdeki bir randevu güncellendi.');
END
GO
