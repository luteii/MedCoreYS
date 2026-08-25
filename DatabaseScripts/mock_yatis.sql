DECLARE @hasta_id INT;
DECLARE @oda_id INT;
DECLARE @yatis_tarihi DATETIME;
DECLARE @cikis_tarihi DATETIME;
DECLARE @max_oda INT = ISNULL((SELECT MAX(oda_id) FROM Odalar), 1);
DECLARE @min_oda INT = ISNULL((SELECT MIN(oda_id) FROM Odalar), 1);

DECLARE cur CURSOR FOR SELECT hasta_id FROM Hastalar;
OPEN cur;
FETCH NEXT FROM cur INTO @hasta_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @oda_id = @min_oda + (ABS(CHECKSUM(NEWID())) % (@max_oda - @min_oda + 1));
    -- Make sure oda_id exists
    IF NOT EXISTS(SELECT 1 FROM Odalar WHERE oda_id = @oda_id)
        SET @oda_id = @min_oda;

    SET @yatis_tarihi = DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 60 + 5), GETDATE());
    
    -- 50% chance of being discharged
    IF (ABS(CHECKSUM(NEWID())) % 2 = 0)
        SET @cikis_tarihi = DATEADD(day, (ABS(CHECKSUM(NEWID())) % 10 + 1), @yatis_tarihi);
    ELSE
        SET @cikis_tarihi = NULL; -- Still in hospital

    INSERT INTO Yatislar (hasta_ID, oda_ID, yatis_tarihi, cikis_tarihi)
    VALUES (@hasta_id, @oda_id, @yatis_tarihi, @cikis_tarihi);

    FETCH NEXT FROM cur INTO @hasta_id;
END
CLOSE cur;
DEALLOCATE cur;
