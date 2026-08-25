ALTER PROC [dbo].[sp_KullaniciSil] @kullanici_id INT AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @doktor_id INT;
    SELECT @doktor_id = doktor_id FROM Doktorlar WHERE kullanici_ID = @kullanici_id;

    DECLARE @hasta_id INT;
    SELECT @hasta_id = hasta_id FROM Hastalar WHERE kullanici_ID = @kullanici_id;

    BEGIN TRY
        BEGIN TRAN;

        -- 1. Temel loglar ve kayitlar
        DELETE FROM Loglar WHERE kullanici_id = @kullanici_id;
        DELETE FROM IslemKayitlari WHERE kullanici_ID = @kullanici_id;
        DELETE FROM DoktorBilgileri WHERE kullanici_id = @kullanici_id;

        -- 2. Hasta ise bagli tablolari temizle
        IF @hasta_id IS NOT NULL
        BEGIN
            DELETE FROM Odemeler WHERE fatura_ID IN (SELECT fatura_id FROM Faturalar WHERE hasta_ID = @hasta_id);
            DELETE FROM Faturalar WHERE hasta_ID = @hasta_id;
            
            DELETE FROM ReceteDetaylari WHERE recete_ID IN (SELECT recete_id FROM Receteler WHERE hasta_ID = @hasta_id);
            DELETE FROM ReceteIlaclari WHERE recete_id IN (SELECT recete_id FROM Receteler WHERE hasta_ID = @hasta_id);
            DELETE FROM Receteler WHERE hasta_ID = @hasta_id;
            
            DELETE FROM TahlilSonuclari WHERE hasta_ID = @hasta_id;
            DELETE FROM Yatislar WHERE hasta_ID = @hasta_id;
            DELETE FROM Randevular WHERE hasta_ID = @hasta_id;
            
            DELETE FROM Hastalar WHERE hasta_id = @hasta_id;
        END

        -- 3. Doktor ise bagli tablolari temizle
        IF @doktor_id IS NOT NULL
        BEGIN
            DELETE FROM ReceteDetaylari WHERE recete_ID IN (SELECT recete_id FROM Receteler WHERE doktor_ID = @doktor_id);
            DELETE FROM ReceteIlaclari WHERE recete_id IN (SELECT recete_id FROM Receteler WHERE doktor_ID = @doktor_id);
            DELETE FROM Receteler WHERE doktor_ID = @doktor_id;
            
            DELETE FROM TahlilSonuclari WHERE doktor_ID = @doktor_id;
            DELETE FROM Randevular WHERE doktor_ID = @doktor_id;
            
            DELETE FROM Doktorlar WHERE doktor_id = @doktor_id;
        END

        -- 4. En son kullaniciyi sil
        DELETE FROM Kullanicilar WHERE kullanici_id = @kullanici_id;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
