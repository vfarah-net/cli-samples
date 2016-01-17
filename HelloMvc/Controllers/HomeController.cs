using Microsoft.AspNet.Mvc;

namespace HelloMvc
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();
    }
}