using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationDemo.API.Migrations
{
    /// <inheritdoc />
    public partial class FixDeviceTokenConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eski UNIQUE index'i kaldır
            migrationBuilder.DropIndex(
                name: "IX_TrustedDevices_DeviceToken",
                table: "TrustedDevices");

            // Yeni composite UNIQUE index ekle (DeviceToken + UserId)
            migrationBuilder.CreateIndex(
                name: "IX_TrustedDevices_DeviceToken_UserId",
                table: "TrustedDevices",
                columns: new[] { "DeviceToken", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
        name: "IX_TrustedDevices_DeviceToken_UserId",
        table: "TrustedDevices");

            migrationBuilder.CreateIndex(
                name: "IX_TrustedDevices_DeviceToken",
                table: "TrustedDevices",
                column: "DeviceToken",
                unique: true);
        }
    }
}
