using System.Net;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Supercell.Magic.Servers.Admin.Helper;
using Supercell.Magic.Servers.Admin.Logic;

namespace Supercell.Magic.Servers.Admin.Controllers
{
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class PublicController : Controller
	{
		[HttpGet]
		public JObject Index()
			=> this.BuildResponse(HttpStatusCode.OK)
					   .AddAttribute("totalUsers", DashboardManager.TotalUsers)
					   .AddAttribute("onlineUsers", ServerManager.GetOnlineUsers())
					   .AddAttribute("averagePings", ServerManager.GetAveragePings());
	}
}