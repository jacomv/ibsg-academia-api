using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bookmarks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookmarks", x => x.id);
                    table.ForeignKey(
                        name: "fk_bookmarks_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bookmarks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_prerequisites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prerequisite_course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_prerequisites", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_prerequisites_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_prerequisites_courses_prerequisite_course_id",
                        column: x => x.prerequisite_course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lesson_attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_lesson_attachments_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    timestamp_seconds = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_student_notes_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_notes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bookmarks_lesson_id",
                table: "bookmarks",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookmarks_user_id_lesson_id",
                table: "bookmarks",
                columns: new[] { "user_id", "lesson_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_course_id_prerequisite_course_id",
                table: "course_prerequisites",
                columns: new[] { "course_id", "prerequisite_course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_prerequisite_course_id",
                table: "course_prerequisites",
                column: "prerequisite_course_id");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_attachments_lesson_id",
                table: "lesson_attachments",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_notes_lesson_id",
                table: "student_notes",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_notes_user_id_lesson_id",
                table: "student_notes",
                columns: new[] { "user_id", "lesson_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookmarks");

            migrationBuilder.DropTable(
                name: "course_prerequisites");

            migrationBuilder.DropTable(
                name: "lesson_attachments");

            migrationBuilder.DropTable(
                name: "student_notes");
        }
    }
}
