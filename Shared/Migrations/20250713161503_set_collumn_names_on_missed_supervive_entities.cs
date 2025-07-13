using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class set_collumn_names_on_missed_supervive_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShieldMitigatedDamage",
                table: "match_player_advanced_stats",
                newName: "shield_mitigated_damage");

            migrationBuilder.RenameColumn(
                name: "HeroEffectiveDamageTaken",
                table: "match_player_advanced_stats",
                newName: "hero_effective_damage_taken");

            migrationBuilder.RenameColumn(
                name: "HeroEffectiveDamageDone",
                table: "match_player_advanced_stats",
                newName: "hero_effective_damage_done");

            migrationBuilder.RenameColumn(
                name: "HeroDamageTaken",
                table: "match_player_advanced_stats",
                newName: "hero_damage_taken");

            migrationBuilder.RenameColumn(
                name: "HeroDamageDone",
                table: "match_player_advanced_stats",
                newName: "hero_damage_done");

            migrationBuilder.RenameColumn(
                name: "HealingReceived",
                table: "match_player_advanced_stats",
                newName: "healing_received");

            migrationBuilder.RenameColumn(
                name: "HealingGivenSelf",
                table: "match_player_advanced_stats",
                newName: "healing_given_self");

            migrationBuilder.RenameColumn(
                name: "HealingGiven",
                table: "match_player_advanced_stats",
                newName: "healing_given");

            migrationBuilder.RenameColumn(
                name: "EffectiveDamageTaken",
                table: "match_player_advanced_stats",
                newName: "effective_damage_taken");

            migrationBuilder.RenameColumn(
                name: "EffectiveDamageDone",
                table: "match_player_advanced_stats",
                newName: "effective_damage_done");

            migrationBuilder.RenameColumn(
                name: "DamageTaken",
                table: "match_player_advanced_stats",
                newName: "damage_taken");

            migrationBuilder.RenameColumn(
                name: "DamageDone",
                table: "match_player_advanced_stats",
                newName: "damage_done");

            migrationBuilder.RenameColumn(
                name: "HeroEffectiveDamageTaken",
                table: "match_player",
                newName: "hero_effective_damage_taken");

            migrationBuilder.RenameColumn(
                name: "HeroEffectiveDamageDone",
                table: "match_player",
                newName: "hero_effective_damage_done");

            migrationBuilder.RenameColumn(
                name: "HealingGivenSelf",
                table: "match_player",
                newName: "healing_given_self");

            migrationBuilder.RenameColumn(
                name: "HealingGiven",
                table: "match_player",
                newName: "healing_given");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "shield_mitigated_damage",
                table: "match_player_advanced_stats",
                newName: "ShieldMitigatedDamage");

            migrationBuilder.RenameColumn(
                name: "hero_effective_damage_taken",
                table: "match_player_advanced_stats",
                newName: "HeroEffectiveDamageTaken");

            migrationBuilder.RenameColumn(
                name: "hero_effective_damage_done",
                table: "match_player_advanced_stats",
                newName: "HeroEffectiveDamageDone");

            migrationBuilder.RenameColumn(
                name: "hero_damage_taken",
                table: "match_player_advanced_stats",
                newName: "HeroDamageTaken");

            migrationBuilder.RenameColumn(
                name: "hero_damage_done",
                table: "match_player_advanced_stats",
                newName: "HeroDamageDone");

            migrationBuilder.RenameColumn(
                name: "healing_received",
                table: "match_player_advanced_stats",
                newName: "HealingReceived");

            migrationBuilder.RenameColumn(
                name: "healing_given_self",
                table: "match_player_advanced_stats",
                newName: "HealingGivenSelf");

            migrationBuilder.RenameColumn(
                name: "healing_given",
                table: "match_player_advanced_stats",
                newName: "HealingGiven");

            migrationBuilder.RenameColumn(
                name: "effective_damage_taken",
                table: "match_player_advanced_stats",
                newName: "EffectiveDamageTaken");

            migrationBuilder.RenameColumn(
                name: "effective_damage_done",
                table: "match_player_advanced_stats",
                newName: "EffectiveDamageDone");

            migrationBuilder.RenameColumn(
                name: "damage_taken",
                table: "match_player_advanced_stats",
                newName: "DamageTaken");

            migrationBuilder.RenameColumn(
                name: "damage_done",
                table: "match_player_advanced_stats",
                newName: "DamageDone");

            migrationBuilder.RenameColumn(
                name: "hero_effective_damage_taken",
                table: "match_player",
                newName: "HeroEffectiveDamageTaken");

            migrationBuilder.RenameColumn(
                name: "hero_effective_damage_done",
                table: "match_player",
                newName: "HeroEffectiveDamageDone");

            migrationBuilder.RenameColumn(
                name: "healing_given_self",
                table: "match_player",
                newName: "HealingGivenSelf");

            migrationBuilder.RenameColumn(
                name: "healing_given",
                table: "match_player",
                newName: "HealingGiven");
        }
    }
}
