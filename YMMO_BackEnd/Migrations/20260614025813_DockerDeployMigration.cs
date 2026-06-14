using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YMMO.Backend.Migrations
{
    /// <inheritdoc />
    public partial class DockerDeployMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Locations_LocationID",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Agencies_AgencyID",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Contacts_AgentID",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "Locations",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "AgentID",
                table: "Contacts",
                newName: "AgentId");

            migrationBuilder.RenameColumn(
                name: "AgencyID",
                table: "Contacts",
                newName: "AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_AgentID",
                table: "Contacts",
                newName: "IX_Contacts_AgentId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_AgencyID",
                table: "Contacts",
                newName: "IX_Contacts_AgencyId");

            migrationBuilder.RenameColumn(
                name: "LocationID",
                table: "Agencies",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "AgencyID",
                table: "Agencies",
                newName: "AgencyId");

            migrationBuilder.RenameIndex(
                name: "IX_Agencies_LocationID",
                table: "Agencies",
                newName: "IX_Agencies_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Locations_LocationId",
                table: "Agencies",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Agencies_AgencyId",
                table: "Contacts",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "AgencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Contacts_AgentId",
                table: "Contacts",
                column: "AgentId",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agencies_Locations_LocationId",
                table: "Agencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Agencies_AgencyId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Contacts_AgentId",
                table: "Contacts");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Locations",
                newName: "LocationID");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "Contacts",
                newName: "AgentID");

            migrationBuilder.RenameColumn(
                name: "AgencyId",
                table: "Contacts",
                newName: "AgencyID");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_AgentId",
                table: "Contacts",
                newName: "IX_Contacts_AgentID");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_AgencyId",
                table: "Contacts",
                newName: "IX_Contacts_AgencyID");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Agencies",
                newName: "LocationID");

            migrationBuilder.RenameColumn(
                name: "AgencyId",
                table: "Agencies",
                newName: "AgencyID");

            migrationBuilder.RenameIndex(
                name: "IX_Agencies_LocationId",
                table: "Agencies",
                newName: "IX_Agencies_LocationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Agencies_Locations_LocationID",
                table: "Agencies",
                column: "LocationID",
                principalTable: "Locations",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Agencies_AgencyID",
                table: "Contacts",
                column: "AgencyID",
                principalTable: "Agencies",
                principalColumn: "AgencyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Contacts_AgentID",
                table: "Contacts",
                column: "AgentID",
                principalTable: "Contacts",
                principalColumn: "ContactId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
