using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YMMO.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyDetailsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Agencies_AgencyID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_AgentID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_BuyerID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_SellerID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Locations_LocationID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPictures_Properties_PropertyId",
                table: "PropertyPictures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPictures",
                table: "PropertyPictures");

            migrationBuilder.RenameTable(
                name: "PropertyPictures",
                newName: "PropertyPicture");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Properties",
                newName: "SellerId");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "Properties",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "BuyerID",
                table: "Properties",
                newName: "BuyerId");

            migrationBuilder.RenameColumn(
                name: "AgentID",
                table: "Properties",
                newName: "AgentId");

            migrationBuilder.RenameColumn(
                name: "AgencyID",
                table: "Properties",
                newName: "AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_SellerID",
                table: "Properties",
                newName: "IX_Properties_SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_LocationID",
                table: "Properties",
                newName: "IX_Properties_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_BuyerID",
                table: "Properties",
                newName: "IX_Properties_BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_AgentID",
                table: "Properties",
                newName: "IX_Properties_AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_AgencyID",
                table: "Properties",
                newName: "IX_Properties_AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPictures_PropertyId",
                table: "PropertyPicture",
                newName: "IX_PropertyPicture_PropertyId");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyName",
                table: "Properties",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyDescription",
                table: "Properties",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPicture",
                table: "PropertyPicture",
                column: "PropertyPictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                table: "Properties",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "AgencyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_AgentId",
                table: "Properties",
                column: "AgentId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_BuyerId",
                table: "Properties",
                column: "BuyerId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_SellerId",
                table: "Properties",
                column: "SellerId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Locations_LocationId",
                table: "Properties",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPicture_Properties_PropertyId",
                table: "PropertyPicture",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_AgentId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_BuyerId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Contacts_SellerId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Locations_LocationId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPicture_Properties_PropertyId",
                table: "PropertyPicture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPicture",
                table: "PropertyPicture");

            migrationBuilder.RenameTable(
                name: "PropertyPicture",
                newName: "PropertyPictures");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Properties",
                newName: "SellerID");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Properties",
                newName: "LocationID");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Properties",
                newName: "BuyerID");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "Properties",
                newName: "AgentID");

            migrationBuilder.RenameColumn(
                name: "AgencyId",
                table: "Properties",
                newName: "AgencyID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_SellerId",
                table: "Properties",
                newName: "IX_Properties_SellerID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_LocationId",
                table: "Properties",
                newName: "IX_Properties_LocationID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_BuyerId",
                table: "Properties",
                newName: "IX_Properties_BuyerID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_AgentId",
                table: "Properties",
                newName: "IX_Properties_AgentID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_AgencyId",
                table: "Properties",
                newName: "IX_Properties_AgencyID");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPicture_PropertyId",
                table: "PropertyPictures",
                newName: "IX_PropertyPictures_PropertyId");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyName",
                table: "Properties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "PropertyDescription",
                table: "Properties",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPictures",
                table: "PropertyPictures",
                column: "PropertyPictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Agencies_AgencyID",
                table: "Properties",
                column: "AgencyID",
                principalTable: "Agencies",
                principalColumn: "AgencyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_AgentID",
                table: "Properties",
                column: "AgentID",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_BuyerID",
                table: "Properties",
                column: "BuyerID",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Contacts_SellerID",
                table: "Properties",
                column: "SellerID",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Locations_LocationID",
                table: "Properties",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPictures_Properties_PropertyId",
                table: "PropertyPictures",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
