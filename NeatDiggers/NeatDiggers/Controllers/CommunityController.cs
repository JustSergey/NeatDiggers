using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NeatDiggers.Controllers
{
    public class CommunityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(string query)
        {
            ViewData["Query"] = query;
            return View();
        }

        public IActionResult Wiki()
        {
            return View();
        }
    }
}
