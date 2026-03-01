namespace Supercell.Magic.Servers.Admin.Logic.Entry
{
	public class UserEntry
	{
		public string User
		{
			get; set;
		}
		public string Password
		{
			get; set;
		}
		public UserRole Role
		{
			get; set;
		}
		public SessionEntry CurrentSession
		{
			get; set;
		}

		public UserEntry(string user, string password, UserRole role)
		{
			User = user;
			Password = password;
			Role = role;
		}
	}
}