using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VKRServer.Migrations
{
    /// <inheritdoc />
    public partial class VKRServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeTable",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    N = table.Column<int>(type: "integer", nullable: false),
                    Groop = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ModerID = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<int>(type: "integer", nullable: false),
                    EndTime = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Audience = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AdminData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    Mail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirtsName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThirdName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdminData_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<BigInteger>(type: "numeric(60,0)", nullable: false),
                    FirtsName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThirdName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ModerData_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    Groop = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FirtsName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThirdName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserData_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarkTable",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    Activity = table.Column<bool>(type: "boolean", nullable: false),
                    Column_0 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_1 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_2 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_3 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_4 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_5 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_6 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Column_7 = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Attendance = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MarkTable_UserData_ID",
                        column: x => x.ID,
                        principalTable: "UserData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarkTable_ID",
                table: "MarkTable",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeTable_Groop_DayOfWeek_N",
                table: "TimeTable",
                columns: new[] { "Groop", "DayOfWeek", "N" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminData");

            migrationBuilder.DropTable(
                name: "MarkTable");

            migrationBuilder.DropTable(
                name: "ModerData");

            migrationBuilder.DropTable(
                name: "TimeTable");

            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
