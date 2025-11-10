using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Package_PackageId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Package_PackageId1",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Users_UserId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PackageId1",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Package",
                table: "Package");

            migrationBuilder.DropColumn(
                name: "PackageId1",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Package",
                newName: "Packages");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_UserId",
                table: "Payments",
                newName: "IX_Payments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PackageId",
                table: "Payments",
                newName: "IX_Payments_PackageId");

            migrationBuilder.AddColumn<int>(
                name: "CoinBalance",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Packages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Packages",
                table: "Packages",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "CoinAmount", "Description", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 100, "Perfect for getting started with lesson creation", true, "Starter Pack", 9900m },
                    { 2, 500, "For frequent lesson creators", true, "Pro Creator", 39900m },
                    { 3, 1000, "Maximum value for power users", true, "Power User", 69900m }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Packages_PackageId",
                table: "Payments",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Packages_PackageId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Packages",
                table: "Packages");

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "CoinBalance",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "Packages",
                newName: "Package");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_UserId",
                table: "Payment",
                newName: "IX_Payment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PackageId",
                table: "Payment",
                newName: "IX_Payment_PackageId");

            migrationBuilder.AddColumn<int>(
                name: "PackageId1",
                table: "Payment",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Package",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Package",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Package",
                table: "Package",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PackageId1",
                table: "Payment",
                column: "PackageId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Package_PackageId",
                table: "Payment",
                column: "PackageId",
                principalTable: "Package",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Package_PackageId1",
                table: "Payment",
                column: "PackageId1",
                principalTable: "Package",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Users_UserId",
                table: "Payment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
