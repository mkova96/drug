using Microsoft.EntityFrameworkCore.Migrations;

namespace DrugData.Migrations
{
    public partial class subs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicationDrugId",
                table: "Drug",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drug_MedicationDrugId",
                table: "Drug",
                column: "MedicationDrugId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drug_Drug_MedicationDrugId",
                table: "Drug",
                column: "MedicationDrugId",
                principalTable: "Drug",
                principalColumn: "DrugId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drug_Drug_MedicationDrugId",
                table: "Drug");

            migrationBuilder.DropIndex(
                name: "IX_Drug_MedicationDrugId",
                table: "Drug");

            migrationBuilder.DropColumn(
                name: "MedicationDrugId",
                table: "Drug");
        }
    }
}
