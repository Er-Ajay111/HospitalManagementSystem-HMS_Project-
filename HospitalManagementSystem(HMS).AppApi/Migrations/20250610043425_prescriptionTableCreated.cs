using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class prescriptionTableCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prescription_tbl",
                columns: table => new
                {
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescribeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription_tbl", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_Prescription_tbl_Appointment_tbl_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment_tbl",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionDetails_tbl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionDetails_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionDetails_tbl_Prescription_tbl_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescription_tbl",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_tbl_AppointmentId",
                table: "Prescription_tbl",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionDetails_tbl_PrescriptionId",
                table: "PrescriptionDetails_tbl",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionDetails_tbl");

            migrationBuilder.DropTable(
                name: "Prescription_tbl");
        }
    }
}
