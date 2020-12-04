using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeatDiggers.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Matches()
        {
            return View();
        }
    }
}
