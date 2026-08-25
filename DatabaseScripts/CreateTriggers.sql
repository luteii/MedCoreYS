-- Receteler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_ReceteEkle ON Receteler AFTER INSERT AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM inserted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RECETE_YAZILDI', GETDATE(), 'Hasta için yeni bir reçete oluşturuldu.');
END
GO

CREATE OR ALTER TRIGGER trg_ReceteSil ON Receteler AFTER DELETE AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM deleted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RECETE_SILINDI', GETDATE(), 'Sistemden bir reçete kaydı silindi.');
END
GO

-- Randevular tablosu icin trigger
CREATE OR ALTER TRIGGER trg_RandevuEkle ON Randevular AFTER INSERT AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM inserted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RANDEVU_OLUSTURULDU', GETDATE(), 'Sisteme yeni bir randevu eklendi.');
END
GO

CREATE OR ALTER TRIGGER trg_RandevuSil ON Randevular AFTER DELETE AS
BEGIN
    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_ID FROM deleted;
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (@doktor_id, 'RANDEVU_IPTALI', GETDATE(), 'Bir randevu iptal edildi veya silindi.');
END
GO

-- Hastalar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_HastaEkle ON Hastalar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_KAYDI', GETDATE(), 'Sisteme yeni bir hasta kaydı yapıldı.');
END
GO

CREATE OR ALTER TRIGGER trg_HastaSil ON Hastalar AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'HASTA_SILINDI', GETDATE(), 'Sistemden bir hasta kaydı silindi.');
END
GO

-- Bolumler tablosu icin trigger
CREATE OR ALTER TRIGGER trg_BolumEkle ON Bolumler AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'BOLUM_EKLENDI', GETDATE(), 'Hastaneye yeni bir bölüm / klinik eklendi.');
END
GO

-- Kullanicilar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_KullaniciEkle ON Kullanicilar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'PERSONEL_EKLENDI', GETDATE(), 'Sisteme yeni bir personel (kullanıcı) eklendi.');
END
GO

CREATE OR ALTER TRIGGER trg_KullaniciSil ON Kullanicilar AFTER DELETE AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'PERSONEL_SILINDI', GETDATE(), 'Sistemden bir personel kaydı silindi.');
END
GO

-- Yatislar tablosu icin trigger
CREATE OR ALTER TRIGGER trg_YatisEkle ON Yatislar AFTER INSERT AS
BEGIN
    INSERT INTO IslemKayitlari (kullanici_ID, islem_tipi, islem_tarihi, islem_kayitlari_aciklama)
    VALUES (NULL, 'YATIS_ISLEMI', GETDATE(), 'Bir hasta için yatış işlemi gerçekleştirildi.');
END
GO
