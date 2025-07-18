﻿using Shared.Data.Entities;
using Shared.Data.Entities.Supervive;
using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Schemas.Supervive.Public;

using MatchType = Shared.Data.Entities.Supervive.MatchType;

namespace Shared.Services {
	public static class SuperviveDataAdapter {
		public static Player PlayerMatchDataToPlayerDb(PrivateMatchData data) => new() {
			PlayerId        = data.PlayerId,
			PlayerIdEncoded = data.PlayerIdEncoded,
			Platform        = data.Platform?.Code ?? GetPlatform(data.PlatformId)
		};

		public static Match MatchDataToDb(PrivateMatchData data, PublicMatchData[] extraData) => new() {
			MatchId      = data.MatchId,
			Platform     = data.Platform?.Code ?? GetPlatform(data.PlatformId),
			WinnerTeam   = extraData.FirstOrDefault(p => p.Placement == 1)?.TeamId,
			Type         = GetMatchType(data, extraData),
			IsRanked     = false,
			IsCustomGame = data.QueueId == "customgame",
			IsInhouse    = false,
			MatchStart   = data.MatchStart,
			MatchEnd     = data.MatchEnd
		};

		public static MatchPlayer IndividualMatchDataToDb(string matchId, PublicMatchData data) => new() {
			MatchId                  = matchId,
			PlayerIdEncoded          = data.PlayerIdEncoded,
			TeamId                   = data.TeamId,
			Hero                     = data.HeroAssetId,
			SurvivalDuration         = data.SurvivalDuration,
			Placement                = data.Placement,
			Kills                    = data.Stats.Kills,
			Deaths                   = data.Stats.Deaths,
			Assists                  = data.Stats.Assists,
			HealingGiven             = data.Stats.HealingGiven,
			HealingGivenSelf         = data.Stats.HealingGivenSelf,
			HeroEffectiveDamageDone  = data.Stats.HeroEffectiveDamageDone,
			HeroEffectiveDamageTaken = data.Stats.HeroEffectiveDamageTaken
		};

		public static MatchPlayerAdvancedStats PrivateMatchDataAdvancedStatsToDb(string matchId, PrivateMatchData data) => new() {
			MatchId                  = matchId,
			PlayerId                 = data.PlayerId,
			Hero                     = data.HeroAssetId,
			SurvivalDuration         = data.SurvivalDuration,
			Kills                    = data.Stats.Kills                    ?? 0,
			Deaths                   = data.Stats.Deaths                   ?? 0,
			Assists                  = data.Stats.Assists                  ?? 0,
			Ressurects               = data.Stats.Resurrected              ?? 0,
			Revived                  = data.Stats.Revived                  ?? 0,
			Knocks                   = data.Stats.Knocks                   ?? 0,
			Knocked                  = data.Stats.Knocked                  ?? 0,
			MaxKillStreak            = data.Stats.MaxKillStreak            ?? 0,
			MaxKnockStreak           = data.Stats.MaxKnockStreak           ?? 0,
			CreepKills               = data.Stats.CreepKills               ?? 0,
			GoldFromEnemies          = data.Stats.GoldFromEnemies          ?? 0,
			GoldFromMonsters         = data.Stats.GoldFromMonsters         ?? 0,
			HealingGiven             = data.Stats.HealingGiven             ?? 0,
			HealingGivenSelf         = data.Stats.HealingGivenSelf         ?? 0,
			HealingReceived          = data.Stats.HealingReceived          ?? 0,
			DamageDone               = data.Stats.DamageDone               ?? 0,
			EffectiveDamageDone      = data.Stats.EffectiveDamageDone      ?? 0,
			HeroDamageDone           = data.Stats.HeroDamageDone           ?? 0,
			HeroEffectiveDamageDone  = data.Stats.HeroEffectiveDamageDone  ?? 0,
			DamageTaken              = data.Stats.DamageTaken              ?? 0,
			EffectiveDamageTaken     = data.Stats.EffectiveDamageTaken     ?? 0,
			HeroDamageTaken          = data.Stats.HeroDamageTaken          ?? 0,
			HeroEffectiveDamageTaken = data.Stats.HeroEffectiveDamageTaken ?? 0,
			ShieldMitigatedDamage    = data.Stats.ShieldMitigatedDamage    ?? 0
		};

		public static MatchType GetMatchType(PrivateMatchData privateData, PublicMatchData[] extraData) =>
			privateData.QueueId switch {
				"deathmatch" => MatchType.Arena,
				"default"    => MatchType.Trios,
				_            => extraData.Length <= 9 ? MatchType.Arena : MatchType.Trios
			};

		public static string GetPlatform(int platformCode) => platformCode switch {
			1 => "steam",
			_ => throw new Exception("Unknown platform id")
		};

		public static string GetInternalHeroName(SuperviveHero hero) => hero switch {
			SuperviveHero.Brall   => "hero:ronin",
			SuperviveHero.Carbine => "hero:bountyhunter",
			SuperviveHero.Crysta  => "hero:burstcaster",
			SuperviveHero.Ghost   => "hero:assault",
			SuperviveHero.Jin     => "hero:stalker",
			SuperviveHero.Joule   => "hero:storm",
			SuperviveHero.Myth    => "hero:huntress",
			SuperviveHero.Saros   => "hero:farshot",
			SuperviveHero.Shiv    => "hero:flex",
			SuperviveHero.Shrike  => "hero:sniper",
			SuperviveHero.Bishop  => "hero:rocketjumper",
			SuperviveHero.Kingpin => "hero:hookguy",
			SuperviveHero.Felix   => "hero:firefox",
			SuperviveHero.Oath    => "hero:shieldbot",
			SuperviveHero.Elluna  => "hero:reshealer",
			SuperviveHero.Eva     => "hero:succubus",
			SuperviveHero.Zeph    => "hero:backlinehealer",
			SuperviveHero.Beebo   => "hero:beebo",
			SuperviveHero.Celeste => "hero:freeze",
			SuperviveHero.Hudson  => "hero:gunner",
			SuperviveHero.Void    => "hero:void",
			_                     => throw new ArgumentOutOfRangeException(nameof(hero), hero, "Unknown hero")
		};
	}
}