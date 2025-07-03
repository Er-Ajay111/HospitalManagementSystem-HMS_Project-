using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem_HMS_.AppApi.Migrations
{
    /// <inheritdoc />
    public partial class labTestPaymentMapping_tblColumnIssueSolved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestPaymentMapping_tbl_LabTestPayment_tbl_LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl");

            migrationBuilder.DropIndex(
                name: "IX_LabTestPaymentMapping_tbl_LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl");

            migrationBuilder.DropColumn(
                name: "LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestPaymentMapping_tbl_PaymentId",
                table: "LabTestPaymentMapping_tbl",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestPaymentMapping_tbl_LabTestPayment_tbl_PaymentId",
                table: "LabTestPaymentMapping_tbl",
                column: "PaymentId",
                principalTable: "LabTestPayment_tbl",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestPaymentMapping_tbl_LabTestPayment_tbl_PaymentId",
                table: "LabTestPaymentMapping_tbl");

            migrationBuilder.DropIndex(
                name: "IX_LabTestPaymentMapping_tbl_PaymentId",
                table: "LabTestPaymentMapping_tbl");

            migrationBuilder.AddColumn<Guid>(
                name: "LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LabTestPaymentMapping_tbl_LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl",
                column: "LabTestPaymentPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestPaymentMapping_tbl_LabTestPayment_tbl_LabTestPaymentPaymentId",
                table: "LabTestPaymentMapping_tbl",
                column: "LabTestPaymentPaymentId",
                principalTable: "LabTestPayment_tbl",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
