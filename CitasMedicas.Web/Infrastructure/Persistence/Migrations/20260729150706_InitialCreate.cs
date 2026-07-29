using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CitasMedicas.Web.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[Specialties]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Specialties] (
                        [Id] int NOT NULL IDENTITY,
                        [Name] nvarchar(100) NOT NULL,
                        CONSTRAINT [PK_Specialties] PRIMARY KEY ([Id])
                    );
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Specialties_Name' AND object_id = OBJECT_ID(N'[dbo].[Specialties]'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Specialties_Name] ON [dbo].[Specialties] ([Name]);
                END;

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Medicina general'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Medicina general');

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Cardiología'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Cardiología');

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Dermatología'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Dermatología');

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Pediatría'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Pediatría');

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Ginecología'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Ginecología');

                INSERT INTO [dbo].[Specialties] ([Name])
                SELECT N'Odontología'
                WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Specialties] WHERE [Name] = N'Odontología');

                IF OBJECT_ID(N'[dbo].[Doctors]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Doctors] (
                        [Id] int NOT NULL IDENTITY,
                        [FullName] nvarchar(200) NOT NULL,
                        [SpecialtyId] int NOT NULL,
                        CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Doctors_Specialties_SpecialtyId]
                            FOREIGN KEY ([SpecialtyId]) REFERENCES [dbo].[Specialties] ([Id]) ON DELETE NO ACTION
                    );
                END
                ELSE
                BEGIN
                    IF COL_LENGTH(N'[dbo].[Doctors]', N'SpecialtyId') IS NULL
                    BEGIN
                        ALTER TABLE [dbo].[Doctors] ADD [SpecialtyId] int NULL;
                    END;

                    UPDATE [dbo].[Doctors]
                    SET [SpecialtyId] = (SELECT [Id] FROM [dbo].[Specialties] WHERE [Name] = N'Medicina general')
                    WHERE [SpecialtyId] IS NULL
                       OR [SpecialtyId] NOT IN (SELECT [Id] FROM [dbo].[Specialties]);

                    ALTER TABLE [dbo].[Doctors] ALTER COLUMN [SpecialtyId] int NOT NULL;

                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Doctors_Specialties_SpecialtyId')
                    BEGIN
                        ALTER TABLE [dbo].[Doctors] ADD CONSTRAINT [FK_Doctors_Specialties_SpecialtyId]
                            FOREIGN KEY ([SpecialtyId]) REFERENCES [dbo].[Specialties] ([Id]) ON DELETE NO ACTION;
                    END;
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctors_SpecialtyId' AND object_id = OBJECT_ID(N'[dbo].[Doctors]'))
                BEGIN
                    CREATE INDEX [IX_Doctors_SpecialtyId] ON [dbo].[Doctors] ([SpecialtyId]);
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[Doctors]', N'U') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Doctors_Specialties_SpecialtyId')
                    BEGIN
                        ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT [FK_Doctors_Specialties_SpecialtyId];
                    END;

                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Doctors_SpecialtyId' AND object_id = OBJECT_ID(N'[dbo].[Doctors]'))
                    BEGIN
                        DROP INDEX [IX_Doctors_SpecialtyId] ON [dbo].[Doctors];
                    END;

                    IF COL_LENGTH(N'[dbo].[Doctors]', N'SpecialtyId') IS NOT NULL
                    BEGIN
                        ALTER TABLE [dbo].[Doctors] DROP COLUMN [SpecialtyId];
                    END;
                END;

                IF OBJECT_ID(N'[dbo].[Specialties]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [dbo].[Specialties];
                END;
                """);
        }
    }
}
