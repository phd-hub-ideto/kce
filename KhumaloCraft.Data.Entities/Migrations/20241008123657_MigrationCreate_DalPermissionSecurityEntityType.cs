using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhumaloCraft.Data.Entities.Migrations
{
    /// <inheritdoc />
    public partial class MigrationCreate_DalPermissionSecurityEntityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionSecurityEntityType",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    SecurityEntityTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionSecurityEntityType_PermissionId_SecurityEntityTypeId", x => new { x.PermissionId, x.SecurityEntityTypeId })
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_PermissionSecurityEntityType_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionSecurityEntityType_SecurityEntityTypeId",
                        column: x => x.SecurityEntityTypeId,
                        principalTable: "SecurityEntityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionSecurityEntityType_SecurityEntityTypeId",
                table: "PermissionSecurityEntityType",
                column: "SecurityEntityTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionSecurityEntityType");
        }
    }
}
