using NetCord.Rest;

namespace Discord.Util {
	public static class Embeds {
		public static EmbedProperties ErrorEmbed {
			get => new() {
				Author = new EmbedAuthorProperties { Name = "Cancelado", IconUrl = Images.RedCross },
				Color  = Colors.DefaultRed
			};
		}

		public static EmbedProperties WarningEmbed {
			get => new() {
				Author = new EmbedAuthorProperties { Name = "Aviso", IconUrl = Images.OrangeExclamation },
				Color  = Colors.DefaultOrange
			};
		}

		public static EmbedProperties SuccessEmbed {
			get => new() {
				Author = new EmbedAuthorProperties { Name = "Sucesso", IconUrl = Images.GreenCheck },
				Color  = Colors.DefaultGreen
			};
		}

		public static EmbedProperties CodeError(Exception e) => new() {
			Author      = new EmbedAuthorProperties { Name = e.GetType().ToString(), IconUrl = Images.RedCross },
			Title       = e.Message,
			Description = $"```\n{(e.StackTrace?.Trim().Length > 3000 ? e.StackTrace?.Trim()[..3000] : e.StackTrace?.Trim())}\n```",
			Color       = Colors.DefaultRed,
			Footer      = new EmbedFooterProperties { Text = $"Runing on {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}", IconUrl = Images.DotnetSquare},
			Timestamp   = DateTimeOffset.Now
		};
	}
}