using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CitasMedicas.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Appointments]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Appointments] (
                        [Id] int NOT NULL IDENTITY,
                        [DoctorId] int NOT NULL,
                        [PatientName] nvarchar(200) NOT NULL,
                        [Date] date NOT NULL,
                        [StartTime] time NOT NULL,
                        [EndTime] time NOT NULL,
                        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Appointments_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION
                    );
                END

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Appointments_DoctorId_Date_StartTime_EndTime'
                      AND object_id = OBJECT_ID(N'[Appointments]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Appointments_DoctorId_Date_StartTime_EndTime]
                    ON [Appointments] ([DoctorId], [Date], [StartTime], [EndTime]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
