using Microsoft.EntityFrameworkCore.Migrations;

namespace DrugData.Migrations
{
    public partial class measurename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeasureName",
                table: "Package",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "Manufacturer",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasureName",
                table: "Package");

            migrationBuilder.AlterColumn<string>(
                name: "ManufacturerName",
                table: "Manufacturer",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
