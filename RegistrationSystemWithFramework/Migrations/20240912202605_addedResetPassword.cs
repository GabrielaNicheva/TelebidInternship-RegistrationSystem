using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistrationSystemWithFramework.Migrations
{
    /// <inheritdoc />
    public partial class addedResetPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resetPassword",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resetPassword",
                table: "Users");
        }
    }
}
