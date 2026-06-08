using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YMMO.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameWishlistToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Contacts_AgentID",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Properties_PropertyID",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "WishlistID",
                table: "Wishlists",
                newName: "WishlistItemID");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Properties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "Features",
                table: "Properties",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Offers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Contacts_AgentID",
                table: "Offers",
                column: "AgentID",
                principalTable: "Contacts",
                principalColumn: "ContactID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Properties_PropertyID",
                table: "Offers",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Contacts_AgentID",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Properties_PropertyID",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Features",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "WishlistItemID",
                table: "Wishlists",
                newName: "WishlistID");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Offers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Contacts_AgentID",
                table: "Offers",
                column: "AgentID",
                principalTable: "Contacts",
                principalColumn: "ContactID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Properties_PropertyID",
                table: "Offers",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
