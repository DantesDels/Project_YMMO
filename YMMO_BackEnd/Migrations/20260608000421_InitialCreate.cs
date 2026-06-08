using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YMMO.Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Complement = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    AgencyID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.AgencyID);
                    table.ForeignKey(
                        name: "FK_Agencies_Locations_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ContactID = table.Column<Guid>(type: "uuid", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    ContactRole = table.Column<int>(type: "integer", nullable: false),
                    ContactType = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    AgencyID = table.Column<Guid>(type: "uuid", nullable: true),
                    Criteria = table.Column<int>(type: "integer", nullable: true),
                    AgentID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ContactID);
                    table.ForeignKey(
                        name: "FK_Contacts_Agencies_AgencyID",
                        column: x => x.AgencyID,
                        principalTable: "Agencies",
                        principalColumn: "AgencyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_Contacts_AgentID",
                        column: x => x.AgentID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyID = table.Column<Guid>(type: "uuid", nullable: false),
                    DateListed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateSold = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InitialPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    PropertyType = table.Column<int>(type: "integer", nullable: false),
                    EnergyClass = table.Column<int>(type: "integer", nullable: false),
                    YearBuilt = table.Column<int>(type: "integer", nullable: false),
                    Surface = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AgencyID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentID = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationID = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerID = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_Properties_Agencies_AgencyID",
                        column: x => x.AgencyID,
                        principalTable: "Agencies",
                        principalColumn: "AgencyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_Contacts_AgentID",
                        column: x => x.AgentID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_Contacts_BuyerID",
                        column: x => x.BuyerID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Properties_Contacts_SellerID",
                        column: x => x.SellerID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Properties_Locations_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    OfferID = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DatePriceUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OfferPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClientID = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentID = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.OfferID);
                    table.ForeignKey(
                        name: "FK_Offers_Contacts_AgentID",
                        column: x => x.AgentID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Contacts_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    WishlistID = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientID = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.WishlistID);
                    table.ForeignKey(
                        name: "FK_Wishlists_Contacts_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Contacts",
                        principalColumn: "ContactID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_LocationID",
                table: "Agencies",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_AgencyID",
                table: "Contacts",
                column: "AgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_AgentID",
                table: "Contacts",
                column: "AgentID");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_AgentID",
                table: "Offers",
                column: "AgentID");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ClientID",
                table: "Offers",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_PropertyID",
                table: "Offers",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AgencyID",
                table: "Properties",
                column: "AgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AgentID",
                table: "Properties",
                column: "AgentID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_BuyerID",
                table: "Properties",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LocationID",
                table: "Properties",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_SellerID",
                table: "Properties",
                column: "SellerID");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ClientID_PropertyID",
                table: "Wishlists",
                columns: new[] { "ClientID", "PropertyID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_PropertyID",
                table: "Wishlists",
                column: "PropertyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
