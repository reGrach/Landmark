using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Landmark.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "races",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serialtag = table.Column<string>(name: "serial_tag", type: "text", nullable: false),
                    teamnumber = table.Column<int>(name: "team_number", type: "integer", nullable: false),
                    starttime = table.Column<TimeOnly>(name: "start_time", type: "time without time zone", nullable: true),
                    finishtime = table.Column<TimeOnly>(name: "finish_time", type: "time without time zone", nullable: true),
                    countpoints = table.Column<int>(name: "count_points", type: "integer", nullable: false),
                    sextype = table.Column<int>(name: "sex_type", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_races", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "participant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    raceid = table.Column<long>(name: "race_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_participant", x => x.id);
                    table.ForeignKey(
                        name: "fk_participant_races_race_id",
                        column: x => x.raceid,
                        principalTable: "races",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_participant_race_id",
                table: "participant",
                column: "race_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "participant");

            migrationBuilder.DropTable(
                name: "races");
        }
    }
}
