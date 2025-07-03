using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class columnAddedToAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_OrderedById",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropIndex(
                name: "IX_PatientLabTests_tbl_OrderedById",
                table: "PatientLabTests_tbl");

            migrationBuilder.AlterColumn<string>(
                name: "OrderedById",
                table: "PatientLabTests_tbl",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "PatientLabTests_tbl",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Appointment_tbl",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Appointment_tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientLabTests_tbl_DoctorId",
                table: "PatientLabTests_tbl",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_DoctorId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropIndex(
                name: "IX_PatientLabTests_tbl_DoctorId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "PatientLabTests_tbl");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Appointment_tbl");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Appointment_tbl");

            migrationBuilder.AlterColumn<string>(
                name: "OrderedById",
                table: "PatientLabTests_tbl",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientLabTests_tbl_OrderedById",
                table: "PatientLabTests_tbl",
                column: "OrderedById");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientLabTests_tbl_AspNetUsers_OrderedById",
                table: "PatientLabTests_tbl",
                column: "OrderedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
