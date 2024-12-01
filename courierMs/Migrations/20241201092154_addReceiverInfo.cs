using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courierMs.Migrations
{
    /// <inheritdoc />
    public partial class addReceiverInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ReceiverInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiverInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiverInfo");

            migrationBuilder.CreateTable(
                name: "Recievers",
                columns: table => new
                {
                    RecieverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    R_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    R_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    R_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    R_Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    R_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    R_city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recievers", x => x.RecieverId);
                });

            migrationBuilder.CreateTable(
                name: "Senders",
                columns: table => new
                {
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    S_Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    S_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    S_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    S_Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    S_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    S_city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.SenderId);
                });
        }
    }
}
