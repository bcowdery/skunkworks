using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortAuthority.Data.Migrations
{
    public partial class InitialSchemaCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pa");

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<Guid>(nullable: false),
                    CorrelationId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(maxLength: 100, nullable: false),
                    Namespace = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTimeOffset>(nullable: true),
                    EndTime = table.Column<DateTimeOffset>(nullable: true),
                    Meta = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subtasks",
                schema: "pa",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<long>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTimeOffset>(nullable: true),
                    EndTime = table.Column<DateTimeOffset>(nullable: true),
                    Meta = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtasks_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "pa",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CorrelationId",
                schema: "pa",
                table: "Jobs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobId",
                schema: "pa",
                table: "Jobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Type_Namespace",
                schema: "pa",
                table: "Jobs",
                columns: new[] { "Type", "Namespace" });

            migrationBuilder.CreateIndex(
                name: "IX_Subtasks_TaskId",
                schema: "pa",
                table: "Subtasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Subtasks_JobId_Name",
                schema: "pa",
                table: "Subtasks",
                columns: new[] { "JobId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subtasks",
                schema: "pa");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "pa");
        }
    }
}
