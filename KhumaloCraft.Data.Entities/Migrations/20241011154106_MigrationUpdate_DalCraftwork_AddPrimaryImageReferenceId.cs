using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhumaloCraft.Data.Entities.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUpdate_DalCraftwork_AddPrimaryImageReferenceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaryImageReferenceId",
                table: "Craftwork",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CraftworkHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Craftwork_PrimaryImageReferenceId",
                table: "Craftwork",
                column: "PrimaryImageReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Craftwork_PrimaryImageReferenceId",
                table: "Craftwork",
                column: "PrimaryImageReferenceId",
                principalTable: "ImageReference",
                principalColumn: "ReferenceId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Craftwork_PrimaryImageReferenceId",
                table: "Craftwork");

            migrationBuilder.DropIndex(
                name: "IX_Craftwork_PrimaryImageReferenceId",
                table: "Craftwork");

            migrationBuilder.DropColumn(
                name: "PrimaryImageReferenceId",
                table: "Craftwork")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CraftworkHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");
        }
    }
}
