CREATE OR ALTER PROCEDURE AntrianSetCanceled
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @CurrentStatus VARCHAR(50),
        @QueueDate     DATE,
        @Stage         VARCHAR(50),
		@StageID       VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================
        -- Ambil data + LOCK
        -- =========================
        SELECT
            @CurrentStatus = Status,
            @QueueDate     = QueueDate,
            @Stage         = CurrentStage,
			@StageID       = StageID
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE VisitQueueNo = @VisitQueueNo;

        IF @CurrentStatus IS NULL
        BEGIN
            THROW 50001, 'Data antrian tidak ditemukan', 1;
        END

		-- =========================
		-- Validasi hanya LOKET
		-- =========================
		IF @StageID <> 'LOKET'
		BEGIN
			THROW 50003,
				'Cancel hanya diperbolehkan pada antrian pendaftaran (LOKET)',
				1;
		END

        -- =========================
        -- Validasi status
        -- =========================
        IF @CurrentStatus NOT IN ('WAITING','PENDING')
        BEGIN
            THROW 50002,
                'Hanya antrian WAITING atau PENDING yang bisa di-CANCELED',
                1;
        END

        -- =========================
        -- Update ke CANCELED
        -- =========================
        UPDATE VisitQueue
        SET
            Status           = 'CANCELED',
            QueueSequence    = NULL,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE VisitQueueNo = @VisitQueueNo;

        COMMIT;

        -- =========================
        -- Return Data
        -- =========================
        SELECT
            VisitQueueNo,
            VisitNo,
            Status,
            StageID,
            CurrentStage,
            CategoryID,
            ServiceUnitID,
            ParamedicID,
            QueueSequence,
            CalledTime,
            LastUpdated,
            UpdatedBy
        FROM VisitQueue
        WHERE VisitQueueNo = @VisitQueueNo;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH
END
GO

EXEC AntrianSetCanceled
    @VisitQueueNo = 'VQUE-260602-0003',
    @UserID = 'admin';