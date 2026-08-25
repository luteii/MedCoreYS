SET NOCOUNT ON;

DECLARE @Dates TABLE (Tarih DATE);
INSERT INTO @Dates VALUES ('2026-08-06');

DECLARE @Doctors TABLE (id INT IDENTITY(1,1), dr_id INT);
INSERT INTO @Doctors (dr_id) VALUES (1), (2), (3), (4), (7);

DECLARE @Patients TABLE (id INT IDENTITY(1,1), h_id INT);
INSERT INTO @Patients (h_id) VALUES (10), (11), (14), (15), (16), (17), (18), (19), (20);

DECLARE @Tests TABLE (id INT IDENTITY(1,1), t_id INT);
INSERT INTO @Tests (t_id) VALUES (27), (28), (29), (30), (31), (32);

DECLARE @Meds TABLE (id INT IDENTITY(1,1), m_id INT);
INSERT INTO @Meds (m_id) VALUES (1), (2), (3), (4), (5), (6), (7), (8);

DECLARE @d INT = 1;
DECLARE @totalDates INT = (SELECT COUNT(*) FROM @Dates);

WHILE @d <= @totalDates
BEGIN
    DECLARE @currentDate DATE;
    SELECT @currentDate = Tarih FROM @Dates ORDER BY Tarih OFFSET (@d - 1) ROWS FETCH NEXT 1 ROWS ONLY;
    
    DECLARE @doc_idx INT = 1;
    WHILE @doc_idx <= 5
    BEGIN
        DECLARE @dr_id INT;
        SELECT @dr_id = dr_id FROM @Doctors WHERE id = @doc_idx;
        
        DECLARE @p INT = 1;
        WHILE @p <= 5
        BEGIN
            DECLARE @patient_index INT = ((@d * @doc_idx * @p) % 9) + 1;
            DECLARE @h_id INT;
            SELECT @h_id = h_id FROM @Patients WHERE id = @patient_index;
            
            DECLARE @isCompleted BIT = CASE WHEN (@p % 3) <> 0 THEN 1 ELSE 0 END;
            
            DECLARE @hour INT = CASE @p WHEN 1 THEN 9 WHEN 2 THEN 10 WHEN 3 THEN 11 WHEN 4 THEN 13 WHEN 5 THEN 14 END;
            DECLARE @randevu_datetime DATETIME = DATEADD(hour, @hour, CAST(@currentDate AS DATETIME));
            
            DECLARE @sikayet NVARCHAR(MAX) = NULL;
            DECLARE @teshis NVARCHAR(MAX) = NULL;
            DECLARE @notlar NVARCHAR(MAX) = NULL;
            
            IF @isCompleted = 1
            BEGIN
                SET @sikayet = CASE (@p % 3) WHEN 1 THEN 'Şiddetli baş ağrısı ve mide bulantısı şikayetiyle geldi.' WHEN 2 THEN 'Boğaz ağrısı, yutkunma güçlüğü ve yüksek ateş.' ELSE 'Bel ağrısı ve aşırı halsizlik.' END;
                SET @teshis = CASE (@p % 3) WHEN 1 THEN 'Akut Migren Atağı' WHEN 2 THEN 'Üst Solunum Yolu Enfeksiyonu' ELSE 'Mekanik Bel Ağrısı / Kas Spazmı' END;
                SET @notlar = 'Bol dinlenme, sıvı tüketimi önerildi. Reçete yazıldı.';
            END
            ELSE
            BEGIN
                SET @sikayet = 'Genel kontrol ve tahlil randevusu.';
            END

            INSERT INTO Randevular (hasta_ID, doktor_ID, randevu_tarihi, randevu_durum, sikayet, teshis, notlar)
            VALUES (@h_id, @dr_id, @randevu_datetime, @isCompleted, @sikayet, @teshis, @notlar);
            
            DECLARE @randevu_id INT = SCOPE_IDENTITY();
            
            IF @isCompleted = 1
            BEGIN
                INSERT INTO Receteler (hasta_ID, doktor_ID, receteler_tarih, tani)
                VALUES (@h_id, @dr_id, @randevu_datetime, @teshis);
                
                DECLARE @recete_id INT = SCOPE_IDENTITY();
                
                DECLARE @med1_idx INT = ((@h_id + @dr_id) % 8) + 1;
                DECLARE @med2_idx INT = ((@h_id + @dr_id + 3) % 8) + 1;
                
                DECLARE @med1 INT, @med2 INT;
                SELECT @med1 = m_id FROM @Meds WHERE id = @med1_idx;
                SELECT @med2 = m_id FROM @Meds WHERE id = @med2_idx;
                
                INSERT INTO ReceteDetaylari (recete_ID, ilac_ID, kullanim_sekli)
                VALUES (@recete_id, @med1, 'Günde 2 kez tok karnına (Sabah-Akşam)');
                
                IF @med1 <> @med2
                BEGIN
                    INSERT INTO ReceteDetaylari (recete_ID, ilac_ID, kullanim_sekli)
                    VALUES (@recete_id, @med2, 'Günde 1 kez yatmadan önce');
                END
                
                DECLARE @test1_idx INT = ((@h_id + @d) % 6) + 1;
                DECLARE @test1 INT;
                SELECT @test1 = t_id FROM @Tests WHERE id = @test1_idx;
                
                INSERT INTO TahlilSonuclari (hasta_ID, tahlil_ID, sonuc_degeri, tahlil_sonuclari_tarih, doktor_ID, doktor_aciklamasi)
                VALUES (@h_id, @test1, 'Normal Sınırlar İçinde / 12.5 mg', @randevu_datetime, @dr_id, 'Sonuçlar referans aralığında. Endişe edilecek bir durum yok.');
            END
            ELSE
            BEGIN
                IF (@p % 2) = 0
                BEGIN
                    DECLARE @test2_idx INT = ((@h_id + @d + 2) % 6) + 1;
                    DECLARE @test2 INT;
                    SELECT @test2 = t_id FROM @Tests WHERE id = @test2_idx;
                    
                    INSERT INTO TahlilSonuclari (hasta_ID, tahlil_ID, sonuc_degeri, tahlil_sonuclari_tarih, doktor_ID, doktor_aciklamasi)
                    VALUES (@h_id, @test2, 'Bekliyor', @randevu_datetime, @dr_id, 'Hastadan numune alındı, laboratuvardan sonuç bekleniyor.');
                END
            END
            
            SET @p = @p + 1;
        END
        
        SET @doc_idx = @doc_idx + 1;
    END
    
    SET @d = @d + 1;
END
PRINT 'Mock data generation completed successfully.';
