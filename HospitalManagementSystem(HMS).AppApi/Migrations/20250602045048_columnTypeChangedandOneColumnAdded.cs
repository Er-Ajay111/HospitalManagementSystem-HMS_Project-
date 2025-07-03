using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class columnTypeChangedandOneColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Nurse_tbl",
                newName: "Departments");

            migrationBuilder.RenameColumn(
                name: "Qualification",
                table: "Doctor_tbl",
                newName: "Qualifications");

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistory",
                table: "Patient_tbl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalHistory",
                table: "Patient_tbl");

            migrationBuilder.RenameColumn(
                name: "Departments",
                table: "Nurse_tbl",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "Qualifications",
                table: "Doctor_tbl",
                newName: "Qualification");
        }
    }
}
