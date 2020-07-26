using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanFictionWebSite.Entities;
using FanFictionWebSite.Models;
using FanFictionWebSite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FanFictionWebSite.Controllers
{
    public class FanficController : Controller
    {
        private readonly FanficsService fanficService;
        private readonly UserManager<User> userManager;
        public FanficController(AppDbContext appDbContext, UserManager<User> userManager)
        {
            this.userManager = userManager;
            fanficService = new FanficsService(appDbContext);
        }

        public async Task<IActionResult> UserFanficControl()
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            return View(fanficService.GetUserFanficControlViewModel(author));
        }

        

        public async  Task<IActionResult> ViewChapter(int fanficId, int number)
        {
            ViewChapterViewModel model;
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(User); 
                model = fanficService.GetChapterViewModel(fanficId, number, user);
            }
            else model = fanficService.GetChapterViewModel(fanficId, number, null);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEditChapter(int fanficId, int number)
        {
            var fanfic = fanficService.GetFanfic(fanficId);
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login/");
            var user = await userManager.GetUserAsync(User);
            var admin = await userManager.IsInRoleAsync(user, "admin");
            if (user.Id != fanfic.Author.Id && !admin) return Redirect("/Home/Index/");
            ViewBag.FanficId = fanficId;
            ViewBag.Number = number;
            return View(fanficService.GetChapterCreatingInputModel(fanficId, number));
        }

        public async Task<IActionResult> RemoveFanfic(int fanficId)
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            fanficService.RemoveFanfic(fanficId, author.Id);
            return Redirect("/Fanfic/UserFanficControl/");
        }

        [HttpPost]
        public IActionResult CreateOrEditChapter(ChapterCreatingInputModel model, string createFanfic, string goToNext)
        {
            if (model.IsUpdating) fanficService.UpdateChapter(model);
            else fanficService.CreateChapter(model);
            if (goToNext == null && createFanfic != null) return Redirect("/Home/Index");
            return Redirect("/Fanfic/CreateOrEditChapter/?fanficId=" + model.FanficId + "&number=" + (model.Number + 1));
        }

        [HttpPost]
        public async Task<IActionResult> SetRating(ViewChapterViewModel model)
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            if (!fanficService.IsRated(author, model.FanficId))
                fanficService.SetRating(author, model.FanficId, model.Rating);
            return Redirect("/Fanfic/ViewChapter/?fanficId=" + model.FanficId + "&number=" + model.ChapterNumber);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int fanficId = -1)
        {
            if(fanficId != -1)
            {
                var fanfic = fanficService.GetFanfic(fanficId);
                if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login/");
                var user = await userManager.GetUserAsync(User);
                var admin = await userManager.IsInRoleAsync(user, "admin");
                if (user.Id != fanfic.Author.Id && !admin) return Redirect("/Home/Index/");
            }
            return View(fanficService.GetFanficCreatingInputModel(fanficId));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(FanficCreatingInputModel model)
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            FanFiction fanfic;
            if (model.IsUpdating) {
                fanfic = fanficService.UpdateFanfic(author, model);
            }
            else fanfic = fanficService.CreateFanfic(author, model);
            return Redirect("/Fanfic/CreateOrEditChapter/?fanficId=" + fanfic.Id + "&number=0");
        }

        [HttpPost]
        public async Task<IActionResult> Like(ViewChapterViewModel model)
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            if(!fanficService.IsLiked(author, model.ChapterNumber, model.FanficId))
                fanficService.CreateLike(author, model.ChapterNumber, model.FanficId);
            return Redirect("/Fanfic/ViewChapter/?fanficId=" + model.FanficId + "&number=" + model.ChapterNumber);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveComment(ViewChapterViewModel model)
        {
            var author = await userManager.FindByNameAsync(User.Identity.Name);
            fanficService.CreateComment(model.FanficId, author, model.CommentContent);
            return Redirect("/Fanfic/ViewChapter/?fanficId=" + model.FanficId + "&number=" + model.ChapterNumber);
        }
    }
}
