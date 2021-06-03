using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DCReplace
{
	public class Config : IConfig
	{
		[Description("Is the plugin enabled?")]
		public bool IsEnabled { get; set; } = true;
		[Description("Should the message sent be a hint? Setting to false will use broadcasts.")]
		public bool UseHints { get; set; } = false;
		[Description("The message sent to players when they replace someone.")]
		public string ReplaceMessage { get; set; } = "<i>You have replaced a player who has disconnected.</i>";
		[Description("Duratation of the message sent to players when they replace someone.")]
		public ushort MessageDuration { get; set; } = 5;
	}
}
