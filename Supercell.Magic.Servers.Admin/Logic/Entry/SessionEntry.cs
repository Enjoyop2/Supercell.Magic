using System;

namespace Supercell.Magic.Servers.Admin.Logic.Entry
{
	public class SessionEntry
	{
		public string Token
		{
			get;
		}
		public UserEntry User
		{
			get;
		}
		public DateTime CreateTime
		{
			get;
		}
		public DateTime UpdateTime
		{
			get; set;
		}

		public SessionEntry(UserEntry user, string token)
		{
			User = user;
			Token = token;
			CreateTime = DateTime.UtcNow;
			UpdateTime = CreateTime;
		}
	}
}