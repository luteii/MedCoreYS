SET NOCOUNT ON;

DECLARE @h_id INT = 21; -- Derya Öz
DECLARE @Dates TABLE (Tarih DATE);
INSERT INTO @Dates VALUES ('2026-08-06'), ('2026-08-07'), ('2026-08-10'), ('2026-08-11'), ('2026-08-12'), ('2026-08-13');

DECLARE @dr_id INT = 1; -- Doctor Ahmet Yılmaz
DECLARE @d INT = 1;
DECLARE @totalDates INT = (SELECT COUNT(*) FROM @Dates);

WHILE @d <= @totalDates
BEGIN
    DECLARE @currentDate DATE;
    SELECT @currentDate = Tarih FROM @Dates ORDER BY Tarih OFFSET (@d - 1) ROWS FETCH NEXT 1 ROWS ONLY;
    
    -- Assign hour based on date
    DECLARE @hour INT = 9 + @d; 
    DECLARE @randevu_datetime DATETIME = DATEADD(hour, @hour, CAST(@currentDate AS DATETIME));
    
    DECLARE @isCompleted BIT = CASE WHEN (@d % 2) = 0 THEN 0 ELSE 1 END;
    
    DECLARE @sikayet NVARCHAR(MAX) = NULL;
    DECLARE @teshis NVARCHAR(MAX) = NULL;
    DECLARE @notlar NVARCHAR(MAX) = NULL;
    
    IF @isCompleted = 1
    BEGIN
        SET @sikayet = N'Boğaz ağrısı, yutkunma güçlüğü ve yüksek ateş.';
        SET @teshis = N'Üst Solunum Yolu Enfeksiyonu';
        SET @notlar = N'Bol dinlenme, sıvı tüketimi önerildi. Reçete yazıldı.';
    END
    ELSE
    BEGIN
        SET @sikayet = N'Genel kontrol ve tahlil randevusu.';
    END

    INSERT INTO Randevular (hasta_ID, doktor_ID, randevu_tarihi, randevu_durum, sikayet, teshis, notlar)
    VALUES (@h_id, @dr_id, @randevu_datetime, @isCompleted, @sikayet, @teshis, @notlar);
    
    DECLARE @randevu_id INT = SCOPE_IDENTITY();
    
    IF @isCompleted = 1
    BEGIN
        -- Reçete
        INSERT INTO Receteler (hasta_ID, doktor_ID, receteler_tarih, tani)
        VALUES (@h_id, @dr_id, @randevu_datetime, @teshis);
        
        DECLARE @recete_id INT = SCOPE_IDENTITY();
        
        INSERT INTO ReceteDetaylari (recete_ID, ilac_ID, kullanim_sekli)
        VALUES (@recete_id, 1, N'Günde 2 kez tok karnına (Sabah-Akşam)');
        
        -- Tahlil Sonuçları (Tamamlanmış)
        INSERT INTO TahlilSonuclari (hasta_ID, tahlil_ID, sonuc_degeri, tahlil_sonuclari_tarih, doktor_ID, doktor_aciklamasi)
        VALUES (@h_id, 27, N'Normal Sınırlar İçinde / 12.5 mg', @randevu_datetime, @dr_id, N'Sonuçlar referans aralığında. Endişe edilecek bir durum yok.');
    END
    ELSE
    BEGIN
        -- Tahlil Bekliyor
        INSERT INTO TahlilSonuclari (hasta_ID, tahlil_ID, sonuc_degeri, tahlil_sonuclari_tarih, doktor_ID, doktor_aciklamasi)
        VALUES (@h_id, 28, N'Bekliyor', @randevu_datetime, @dr_id, N'Hastadan numune alındı, laboratuvardan sonuç bekleniyor.');
    END
    
    SET @d = @d + 1;
END
PRINT 'Derya Oz mock data generation completed successfully.';
