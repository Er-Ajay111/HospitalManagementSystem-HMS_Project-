using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class labTestPaymentMapping_tblCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestPayment_tbl_PatientLabTests_tbl_PatientLabTestId",
                table: "LabTestPayment_tbl");

            migrationBuilder.DropIndex(
                name: "IX_LabTestPayment_tbl_PatientLabTestId",
                table: "LabTestPayment_tbl");

            migrationBuilder.DropColumn(
                name: "PatientLabTestId",
                table: "LabTestPayment_tbl");

            migrationBuilder.CreateTable(
                name: "LabTestPaymentMapping_tbl",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabTestPaymentPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientLabTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestPaymentMapping_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestPaymentMapping_tbl_LabTestPayment_tbl_LabTestPaymentPaymentId",
                        column: x => x.LabTestPaymentPaymentId,
                        principalTable: "LabTestPayment_tbl",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTestPaymentMapping_tbl_PatientLabTests_tbl_PatientLabTestId",
                        column: x => x.PatientLabTestId,
                        principalTable: "PatientLabTests_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestPaymentMapping_tbl_LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl",
                column: "LabTestPaymentPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestPaymentMapping_tbl_PatientLabTestId",
                table: "LabTestPaymentMapping_tbl",
                column: "PatientLabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestPaymentMapping_tbl");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientLabTestId",
                table: "LabTestPayment_tbl",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LabTestPayment_tbl_PatientLabTestId",
                table: "LabTestPayment_tbl",
                column: "PatientLabTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestPayment_tbl_PatientLabTests_tbl_PatientLabTestId",
                table: "LabTestPayment_tbl",
                column: "PatientLabTestId",
                principalTable: "PatientLabTests_tbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
