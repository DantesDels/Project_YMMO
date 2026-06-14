using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YMMO.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PropertyID",
                table: "Properties",
                newName: "PropertyId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Offers",
                newName: "StatusOffer");

            migrationBuilder.RenameColumn(
                name: "ContactID",
                table: "Contacts",
                newName: "ContactId");

            migrationBuilder.AddColumn<string>(
                name: "PropertyDescription",
                table: "Properties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyName",
                table: "Properties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Contacts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Contacts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Contacts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Contacts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PropertyPictures",
                columns: table => new
                {
                    PropertyPictureId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPictures", x => x.PropertyPictureId);
                    table.ForeignKey(
                        name: "FK_PropertyPictures_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPictures_PropertyId",
                table: "PropertyPictures",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyPictures");

            migrationBuilder.DropColumn(
                name: "PropertyDescription",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyName",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "Properties",
                newName: "PropertyID");

            migrationBuilder.RenameColumn(
                name: "StatusOffer",
                table: "Offers",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "Contacts",
                newName: "ContactID");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Contacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Contacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Contacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }
    }
}
