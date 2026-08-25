-- Receteler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_ReceteEkle ON Receteler AFTER INSERT AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM inserted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RECETE_YAZILDI', GETDATE(), N'Hasta için yeni bir reçete oluşturuldu.');
END
GO

CREATE OR ALTER TRIGGER trg_ReceteSil ON Receteler AFTER DELETE AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM deleted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RECETE_SILINDI', GETDATE(), N'Sistemden bir reçete kaydı silindi.');
END
GO

-- Randevular tablosu icin trigger
CREATE OR ALTER TRIGGER trg_RandevuEkle ON Randevular AFTER INSERT AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM inserted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RANDEVU_OLUSTURULDU', GETDATE(), N'Sisteme yeni bir randevu eklendi.');
END
GO

CREATE OR ALTER TRIGGER trg_RandevuSil ON Randevular AFTER DELETE AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM deleted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RANDEVU_IPTALI', GETDATE(), N'Bir randevu iptal edildi veya silindi.');
END
GO

-- Hastalar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_HastaEkle ON Hastalar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_KAYDI', GETDATE(), N'Sisteme yeni bir hasta kaydı yapıldı.');
END
GO

CREATE OR ALTER TRIGGER trg_HastaSil ON Hastalar AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_SILINDI', GETDATE(), N'Sistemden bir hasta kaydı silindi.');
END
GO

-- Bolumler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_BolumEkle ON Bolumler AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'BOLUM_EKLENDI', GETDATE(), N'Hastaneye yeni bir bölüm / klinik eklendi.');
END
GO

-- Kullanicilar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_KullaniciEkle ON Kullanicilar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'PERSONEL_EKLENDI', GETDATE(), N'Sisteme yeni bir personel (kullanıcı) eklendi.');
END
GO

CREATE OR ALTER TRIGGER trg_KullaniciSil ON Kullanicilar AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'PERSONEL_SILINDI', GETDATE(), N'Sistemden bir personel kaydı silindi.');
END
GO

-- Yatislar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_YatisEkle ON Yatislar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'YATIS_ISLEMI', GETDATE(), N'Bir hasta için yatış işlemi gerçekleştirildi.');
END
GO

-- Tahliller tablosu icin trigger
CREATE OR ALTER TRIGGER trg_TahlilEkle ON Tahliller AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_ISTENDI', GETDATE(), N'Hasta için yeni bir tahlil istendi.');
END
GO

CREATE OR ALTER TRIGGER trg_TahlilSil ON Tahliller AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_IPTALI', GETDATE(), N'Sistemden bir tahlil kaydı silindi.');
END
GO

-- TahlilSonuclari tablosu icin trigger
CREATE OR ALTER TRIGGER trg_TahlilSonucEkle ON TahlilSonuclari AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'TAHLIL_SONUCU', GETDATE(), N'Hasta tahlil sonuçları sisteme girildi.');
END
GO

-- Odemeler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_OdemeEkle ON Odemeler AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'ODEME_ALINDI', GETDATE(), N'Hasta için yeni bir ödeme kaydı oluşturuldu.');
END
GO

-- Faturalar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_FaturaEkle ON Faturalar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'FATURA_KESILDI', GETDATE(), N'Hasta için fatura kesildi.');
END
GO

-- Hastalar tablosu guncelleme icin trigger (UPDATE)
CREATE OR ALTER TRIGGER trg_HastaGuncelle ON Hastalar AFTER UPDATE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_GUNCELLEME', GETDATE(), N'Sistemdeki bir hastanın kayıt bilgileri güncellendi.');
END
GO

-- Randevular tablosu guncelleme icin trigger (UPDATE)
CREATE OR ALTER TRIGGER trg_RandevuGuncelle ON Randevular AFTER UPDATE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'RANDEVU_GUNCELLEME', GETDATE(), N'Sistemdeki bir randevu güncellendi.');
END
GO
