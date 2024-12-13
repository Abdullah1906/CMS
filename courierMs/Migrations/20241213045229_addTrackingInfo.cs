using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courierMs.Migrations
{
    /// <inheritdoc />
    public partial class addTrackingInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackerInfo",
                columns: table => new
                {
                    TrackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverId = table.Column<string>(nullable: true),
                    CustomerId = table.Column<string>(nullable: true),
                    PercelId = table.Column<string>(nullable: true),
                    TrackerName = table.Column<string>(nullable: true),
                    Tracker_P_Number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackerInfo", x => x.TrackId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackerInfo");
        }
    }
}
