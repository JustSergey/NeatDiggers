using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NeatDiggers.Controllers
{
    public class WikiController : Controller
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
    }
}
