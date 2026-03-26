/****** Object:  Table [dbo].[BpjsRujukanSatuSehat]    Script Date: 3/26/2026 10:08:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BpjsRujukanSatuSehat](
	[noSep] [varchar](50) NOT NULL,
	[NoRujukan] [varchar](50) NOT NULL,
	[tglRujukan] [smalldatetime] NULL,
	[tglRencana] [smalldatetime] NULL,
	[ppkDirujuk] [varchar](50) NULL,
	[namaPpkDirujuk] [varchar](255) NULL,
	[jnsPelayanan] [char](1) NULL,
	[catatan] [varchar](max) NULL,
	[diagRujukan] [varchar](20) NULL,
	[tipeRujukan] [char](1) NULL,
	[poliRujukan] [varchar](10) NULL,
	[namaPoliRujukan] [varchar](255) NULL,
	[user] [varchar](40) NULL,
	[kodeFaskesSatuSehat] [varchar](50) NULL,
	[idPasienSatuSehat] [varchar](50) NULL,
	[kdppkSatuSehatTujuanRujukan] [varchar](50) NULL,
	[kdDokterSatuSehat] [varchar](50) NULL,
	[EncounterReference] [varchar](100) NULL,
	[patientInstruction] [varchar](255) NULL,
	[keteranganRujukan] [varchar](255) NULL,
	[kodePropinsi] [varchar](10) NULL,
	[namaPropinsi] [varchar](100) NULL,
	[kodeKabupaten] [varchar](10) NULL,
	[namaKabupaten] [varchar](100) NULL,
	[KriteriaRujukanJson] [varchar](max) NULL,
	[noRujukanSatuSehat] [varchar](50) NULL,
	[serviceRequestId] [varchar](100) NULL,
	[asalRujukanKode] [varchar](20) NULL,
	[asalRujukanNama] [varchar](255) NULL,
	[diagnosaKode] [varchar](20) NULL,
	[diagnosaNama] [varchar](255) NULL,
	[pesertaAsuransi] [varchar](50) NULL,
	[pesertaHakKelas] [varchar](20) NULL,
	[pesertaJenis] [varchar](50) NULL,
	[pesertaKelamin] [varchar](20) NULL,
	[pesertaNama] [varchar](255) NULL,
	[pesertaNoKartu] [varchar](50) NULL,
	[pesertaNoMR] [varchar](50) NULL,
	[pesertaTglLahir] [date] NULL,
	[poliTujuanKode] [varchar](20) NULL,
	[poliTujuanNama] [varchar](255) NULL,
	[tujuanRujukanKode] [varchar](20) NULL,
	[tujuanRujukanNama] [varchar](255) NULL,
	[bpjsResponseCode] [varchar](10) NULL,
	[bpjsResponseMessage] [varchar](255) NULL,
	[RequestJson] [varchar](max) NULL,
	[ResponseJson] [varchar](max) NULL,
	[LastUpdateDateTime] [datetime] NULL,
	[LastUpdateByUserID] [varchar](40) NULL,
 CONSTRAINT [PK_BpjsRujukanSatuSehat] PRIMARY KEY CLUSTERED 
(
	[noSep] ASC,
	[NoRujukan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

