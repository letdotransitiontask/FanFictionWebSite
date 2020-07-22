using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanFictionWebSite.Entities;
using FanFictionWebSite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FanFictionWebSite.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext appDbContext;
        private readonly FanficsService FanficsService;
        private SignInManager<User> siiign;
        public HomeController(AppDbContext appDbContext, SignInManager<User> signInManager)
        {
            this.appDbContext = appDbContext;
            this.FanficsService = new FanficsService(appDbContext);
            siiign = signInManager;
        }

        public IActionResult Index()
        {
            var fanfics = FanficsService.GetFanficsList();
            return View(fanfics);
        }
    }
}
