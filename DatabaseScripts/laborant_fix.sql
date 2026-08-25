USE HASTANEYONETIM_DB;
GO

-- 1. Restore Ahmet Yilmaz's password (by copying from user 2)
DECLARE @defaultHash VARCHAR(255) = (SELECT sifre FROM Kullanicilar WHERE kullanici_id = 2);
UPDATE Kullanicilar SET sifre = @defaultHash WHERE tc_no = '11111111111';

-- 2. Create the Laborant user properly with TC 12121212121
IF NOT EXISTS (SELECT 1 FROM Kullanicilar WHERE tc_no = '12121212121')
BEGIN
    INSERT INTO Kullanicilar (ad_soyad, tc_no, sifre, rol_ID, son_giris_tarihi, hesap_aktif_mi)
    VALUES ('Test Laborant', '12121212121', 'mvFbM25qlhmShTffMLLmojdlafz51+dz7M7eZWBlKaA=', 5, GETDATE(), 1);
END
ELSE
BEGIN
    UPDATE Kullanicilar 
    SET sifre = 'mvFbM25qlhmShTffMLLmojdlafz51+dz7M7eZWBlKaA=', rol_ID = 5, hesap_aktif_mi = 1
    WHERE tc_no = '12121212121';
END
GO
