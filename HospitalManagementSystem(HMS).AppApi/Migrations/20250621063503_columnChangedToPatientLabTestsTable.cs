using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class columnChangedToPatientLabTestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId",
                table: "PatientLabTests_tbl",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PatientLabTests_tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportFilePath",
                table: "PatientLabTests_tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "PatientLabTests_tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientLabTests_tbl_AppointmentId",
                table: "PatientLabTests_tbl",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLabTests_tbl_Appointment_tbl_AppointmentId",
                table: "PatientLabTests_tbl",
                column: "AppointmentId",
                principalTable: "Appointment_tbl",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientLabTests_tbl_Appointment_tbl_AppointmentId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropIndex(
                name: "IX_PatientLabTests_tbl_AppointmentId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "ReportFilePath",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "PatientLabTests_tbl");
        }
    }
}
