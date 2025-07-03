using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class navigationPropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_tbl_UserId",
                table: "Patient_tbl");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_tbl_UserId",
                table: "Patient_tbl",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_tbl_UserId",
                table: "Patient_tbl");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_tbl_UserId",
                table: "Patient_tbl",
                column: "UserId");
        }
    }
}
