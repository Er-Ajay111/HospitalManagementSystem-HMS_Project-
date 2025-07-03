using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class columnRenamedToPatientLabTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "OrderedById",
                table: "PatientLabTests_tbl");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "PatientLabTests_tbl",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "PatientLabTests_tbl",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "OrderedById",
                table: "PatientLabTests_tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
