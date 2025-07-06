using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class init_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    platform = table.Column<string>(type: "text", nullable: false),
                    winner_team = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    is_ranked = table.Column<bool>(type: "boolean", nullable: false),
                    is_custom_game = table.Column<bool>(type: "boolean", nullable: false),
                    is_inhouse = table.Column<bool>(type: "boolean", nullable: false),
                    inhouse_server_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    match_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    match_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match", x => x.match_id);
                });

            migrationBuilder.CreateTable(
                name: "player",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "text", nullable: false),
                    player_id_encoded = table.Column<string>(type: "text", nullable: false),
                    discord_user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    platform = table.Column<string>(type: "text", nullable: false),
                    last_synced_match = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.player_id);
                    table.UniqueConstraint("AK_player_player_id_encoded", x => x.player_id_encoded);
                });

            migrationBuilder.CreateTable(
                name: "match_player",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    player_id_encoded = table.Column<string>(type: "text", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    hero = table.Column<string>(type: "text", nullable: false),
                    survival_duration = table.Column<double>(type: "double precision", nullable: false),
                    kills = table.Column<int>(type: "integer", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false),
                    HealingGiven = table.Column<double>(type: "double precision", nullable: false),
                    HealingGivenSelf = table.Column<double>(type: "double precision", nullable: false),
                    HeroEffectiveDamageDone = table.Column<double>(type: "double precision", nullable: false),
                    HeroEffectiveDamageTaken = table.Column<double>(type: "double precision", nullable: false),
                    rating_delta = table.Column<float>(type: "real", nullable: true),
                    rating = table.Column<float>(type: "real", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_player", x => new { x.match_id, x.player_id_encoded });
                    table.ForeignKey(
                        name: "FK_match_player_match_match_id",
                        column: x => x.match_id,
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_player_player_player_id_encoded",
                        column: x => x.player_id_encoded,
                        principalTable: "player",
                        principalColumn: "player_id_encoded");
                });

            migrationBuilder.CreateTable(
                name: "match_player_advanced_stats",
                columns: table => new
                {
                    match_id = table.Column<string>(type: "text", nullable: false),
                    player_id = table.Column<string>(type: "text", nullable: false),
                    hero = table.Column<string>(type: "text", nullable: false),
                    survival_duration = table.Column<double>(type: "double precision", nullable: false),
                    kills = table.Column<int>(type: "integer", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false),
                    ressurects = table.Column<int>(type: "integer", nullable: false),
                    revived = table.Column<int>(type: "integer", nullable: false),
                    knocks = table.Column<int>(type: "integer", nullable: false),
                    knocked = table.Column<int>(type: "integer", nullable: false),
                    max_kill_streak = table.Column<int>(type: "integer", nullable: false),
                    max_knock_streak = table.Column<int>(type: "integer", nullable: false),
                    creep_kills = table.Column<int>(type: "integer", nullable: false),
                    gold_from_enemies = table.Column<int>(type: "integer", nullable: false),
                    gold_from_monsters = table.Column<int>(type: "integer", nullable: false),
                    HealingGiven = table.Column<double>(type: "double precision", nullable: false),
                    HealingGivenSelf = table.Column<double>(type: "double precision", nullable: false),
                    HealingReceived = table.Column<double>(type: "double precision", nullable: false),
                    DamageDone = table.Column<double>(type: "double precision", nullable: false),
                    EffectiveDamageDone = table.Column<double>(type: "double precision", nullable: false),
                    HeroDamageDone = table.Column<double>(type: "double precision", nullable: false),
                    HeroEffectiveDamageDone = table.Column<double>(type: "double precision", nullable: false),
                    DamageTaken = table.Column<double>(type: "double precision", nullable: false),
                    EffectiveDamageTaken = table.Column<double>(type: "double precision", nullable: false),
                    HeroDamageTaken = table.Column<double>(type: "double precision", nullable: false),
                    HeroEffectiveDamageTaken = table.Column<double>(type: "double precision", nullable: false),
                    ShieldMitigatedDamage = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_player_advanced_stats", x => new { x.match_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_match_player_advanced_stats_match_match_id",
                        column: x => x.match_id,
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_player_advanced_stats_player_player_id",
                        column: x => x.player_id,
                        principalTable: "player",
                        principalColumn: "player_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_created_at",
                table: "match",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_match_inhouse_server_id",
                table: "match",
                column: "inhouse_server_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_is_inhouse",
                table: "match",
                column: "is_inhouse");

            migrationBuilder.CreateIndex(
                name: "IX_match_is_ranked",
                table: "match",
                column: "is_ranked");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_created_at",
                table: "match_player",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_deleted_at",
                table: "match_player",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_hero",
                table: "match_player",
                column: "hero");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_player_id_encoded",
                table: "match_player",
                column: "player_id_encoded");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_team_id",
                table: "match_player",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_advanced_stats_created_at",
                table: "match_player_advanced_stats",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_match_player_advanced_stats_player_id",
                table: "match_player_advanced_stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_created_at",
                table: "player",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_player_discord_user_id",
                table: "player",
                column: "discord_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_player");

            migrationBuilder.DropTable(
                name: "match_player_advanced_stats");

            migrationBuilder.DropTable(
                name: "match");

            migrationBuilder.DropTable(
                name: "player");
        }
    }
}
