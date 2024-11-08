using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRelacionamentoBookingRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bokings_Rooms_RoomId",
                table: "Bokings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokings_Rooms_RoomId",
                table: "Bokings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bokings_Rooms_RoomId",
                table: "Bokings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokings_Rooms_RoomId",
                table: "Bokings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
