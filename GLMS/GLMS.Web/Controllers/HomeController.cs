using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GLMS.Web.Models;

namespace GLMS.Web.Controllers
{
    public class HomeController : Controller
    {

    //The base of the controllers in terms of how to go about the design was Ai assisted but I though of and code the logic myself.
        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
