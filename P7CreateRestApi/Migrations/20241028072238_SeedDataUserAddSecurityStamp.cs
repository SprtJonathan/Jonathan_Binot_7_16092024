using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P7CreateRestApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataUserAddSecurityStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2416413-25a3-41d9-bb7a-279dfa821f5b", "AQAAAAIAAYagAAAAEFSmoYAQNGLTorTJ6F9OXwlNGj0eVlPQw8SedpNgoVi5FGAx/Zcy1gHEUK7AsipNfw==", "5e1bcfed-6d2e-4a4f-9a46-cc5cd1b1a028" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7df573d4-7131-4974-babc-a28bc6e3b71d", "AQAAAAIAAYagAAAAEM/7qnf63JOKzP880qsEyfBg/U2CVG7M/ccI5Ql+o0L3Fkml3tcSK+NRSr0cG5+PRQ==", "2807cd58-72cf-428f-aca0-346a412b2b77" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f609c986-4aaf-45da-ba8e-5e8b87c2407c", "AQAAAAIAAYagAAAAEG03XLKcaRS30b9Toug8EVL38aBWJJ/kh2+x3AhalE4IKyfQehj6dq9/TDU+OSzzIw==", null });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1aa90a97-e019-4bea-aa1d-b435047bc78b", "AQAAAAIAAYagAAAAEGfJ0WdW8qrEVDuncL5wvcnnTdtfx1XqqlDJAHd/J4+NO98+A/JT7NUIcbrdpjP9YQ==", null });
        }
    }
}
