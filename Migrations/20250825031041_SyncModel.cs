using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApi.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExamID",
                table: "Exams",
                newName: "ExamId");

            migrationBuilder.AddColumn<int>(
                name: "CourseID",
                table: "Exams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Exams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseID",
                table: "Exams",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Courses_CourseID",
                table: "Exams",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Courses_CourseID",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CourseID",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "Exams",
                newName: "ExamID");
        }
    }
}
