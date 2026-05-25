CREATE OR ALTER PROCEDURE AntrianCallNextQueueAllServiceUnit
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @QueueDate          DATE,
        @CurrentStage       VARCHAR(50),
        @CounterID          VARCHAR(50),
        @CurrentStatus      VARCHAR(50),
        @RegistrationNo     VARCHAR(50),
        @ParamedicID        VARCHAR(50),
        @ServiceUnitID      VARCHAR(50),
		@StageID			VARCHAR(50),
		@CategoryID			VARCHAR(50),

        -- 🔥 IMPORTANT: pisahkan variable
        @NextVisitQueueNo   VARCHAR(50),
        @NextVisitNo        VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- AMBIL DATA CURRENT (FIXED CONTEXT)
        -- =========================================
        SELECT
            @QueueDate       = CAST(vq.QueueDate AS DATE),
            @CurrentStage    = vq.CurrentStage,
            @CounterID       = vq.CalledByCounterID,
            @CurrentStatus   = vq.Status,
            @RegistrationNo  = vq.RegistrationNo,
            @ServiceUnitID   = vq.ServiceUnitID,
            @ParamedicID     = vq.ParamedicID,
			@StageID		 = vq.StageID,
			@CategoryID      = vq.CategoryID
        FROM VisitQueue vq WITH (UPDLOCK, HOLDLOCK)
        WHERE vq.VisitQueueNo = @VisitQueueNo;

        IF @CurrentStatus IS NULL
            THROW 50001, 'Data antrian tidak ditemukan', 1;

        IF @CurrentStatus <> 'CALLED'
            THROW 50002, 'Hanya antrian CALLED yang bisa di-next', 1;

        -- =========================================
        -- FINISH CURRENT
        -- =========================================
        UPDATE VisitQueue
        SET
            Status      = 'FINISHED',
            UpdatedBy   = @UserID,
            LastUpdated = GETDATE()
        WHERE VisitQueueNo = @VisitQueueNo;

        -- =========================================
        -- NEXT QUEUE (JANGAN OVERWRITE INPUT)
        -- =========================================
        SELECT TOP 1
            @NextVisitQueueNo = VisitQueueNo,
            @NextVisitNo      = VisitNo
        FROM VisitQueue WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE 
            CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @CurrentStage
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID,'') = ISNULL(@ParamedicID,'')
			AND ISNULL(CategoryID,'') = ISNULL(@CategoryID,'')
            AND Status = 'WAITING'
        ORDER BY 
            Priority ASC,
            QueueSequence ASC,
            CreatedDate ASC;

		-- =========================================
		-- SET NEXT → CALLED (INI YANG KAMU LUPA)
		-- =========================================
		IF @NextVisitQueueNo IS NOT NULL
		BEGIN
			UPDATE VisitQueue
			SET
				Status      = 'CALLED',
				CalledTime  = GETDATE(),
				UpdatedBy   = @UserID,
				LastUpdated = GETDATE(),
				CalledByCounterID = @CounterID
			WHERE VisitQueueNo = @NextVisitQueueNo;
		END

		-- =========================================
		-- CREATE NEXT STEP PENUNJANG
		-- =========================================
		IF @CurrentStage IN ('USG', 'LAB', 'RADIOLOGI', 'ENDOSCOPY', 'CT SCAN')
		BEGIN

			EXEC TakeQueueVisitNumberForPenunjang
				@RegistrationNo = @RegistrationNo,
				@VisitQueueNo   = @VisitQueueNo,
				@ServiceUnitID  = @ServiceUnitID,
				@UserID         = @UserID;

		END

        COMMIT;

        -- optional return NEXT
        SELECT 
            @NextVisitQueueNo AS NextVisitQueueNo,
            @NextVisitNo AS NextVisitNo,
			@CurrentStatus AS Status,
			@ParamedicID AS ParamedicID,
			@ServiceUnitID AS ServiceUnitID,
			@CurrentStage AS CurrentStage,
			@StageID AS StageID;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END


EXEC AntrianCallNextQueueAllServiceUnit
    @VisitQueueNo = 'VQUE-260520-0020',
    @UserID       = '240076';