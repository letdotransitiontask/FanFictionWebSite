using FanFictionWebSite.Entities;
using FanFictionWebSite.Models;
using Markdig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Services
{
    public class FanficsService
    {
        public string Parse(string markdown)
        {
            if (markdown == null) return "";
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            return Markdown.ToHtml(markdown, pipeline);
        }

        private readonly AppDbContext appDbContext;
        public FanficsService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        
        public ChapterCreatingInputModel GetChapterCreatingInputModel(int fanficId, int number)
        {
            var fanfic = appDbContext.FanFictions.Include(f => f.Chapters).Single(f => f.Id == fanficId);
            if(number >= fanfic.Chapters.Count)
            {
                return new ChapterCreatingInputModel()
                {
                    IsUpdating = false,
                    FanficId = fanficId,
                    Number = number
                };
            }
            else
            {
                var chapter = fanfic.Chapters.Single(c => c.Number == number);
                return new ChapterCreatingInputModel()
                {
                    Number = number,
                    Name = chapter.Name,
                    Content = chapter.Content,
                    Image = chapter.Image,
                    FanficId = fanficId,
                    IsUpdating = true
                };
            }
        }
        public FanficCreatingInputModel GetFanficCreatingInputModel(int fanficId)
        {
            if (fanficId == -1) return new FanficCreatingInputModel() {FanficId = fanficId, IsUpdating = false};
            else
            {
                var fanfic = appDbContext.FanFictions.Include(f => f.Category).Single(f => f.Id == fanficId);
                return new FanficCreatingInputModel()
                {
                    FanficId = fanficId,
                    Name = fanfic.Name,
                    Description = fanfic.Description,
                    Category = fanfic.Category.Name, 
                    IsUpdating = true
                };
            }
        }
        public FanFiction CreateFanfic(User user, FanficCreatingInputModel model)
        {
            var fanfic = new FanFiction()
            {
                Author = user,
                Name = model.Name,
                Description = model.Description,
                Category = appDbContext.Categories.Single(c => c.Name == model.Category)
            };
            appDbContext.FanFictions.Add(fanfic);
            appDbContext.SaveChanges();
            return fanfic;
        }

        public FanFiction GetFanfic(int id)
        {
            return appDbContext.FanFictions.Include(f => f.Author).Single(f => f.Id == id);
        }
        public FanFiction UpdateFanfic(User user, FanficCreatingInputModel model)
        {
            var fanfic = appDbContext.FanFictions.Find(model.FanficId);
            fanfic.Name = model.Name;
            fanfic.Category = appDbContext.Categories.Single(c => c.Name == model.Category);
            fanfic.Description = model.Description;
            appDbContext.SaveChanges();
            return fanfic;
        }

        public Chapter CreateChapter(ChapterCreatingInputModel model)
        {
            var fanfic = appDbContext.FanFictions.Find(model.FanficId);
            var chapter = new Chapter()
            {
                Name = model.Name,
                Content = model.Content,
                Image = model.Image,
                FanFiction = fanfic,
                Number = model.Number
            };
            appDbContext.Chapters.Add(chapter);
            appDbContext.SaveChanges();
            return chapter;
        }

        public Chapter UpdateChapter(ChapterCreatingInputModel model)
        {
            var chapter = appDbContext.Chapters.Include(c => c.FanFiction).Single(c => c.FanFiction.Id == model.FanficId && c.Number == model.Number);
            chapter.Name = model.Name;
            chapter.Image = model.Image;
            chapter.Content = model.Content;
            appDbContext.SaveChanges();
            return chapter;
        }

        public List<FanficInListViewModel> GetFanficsList()
        {
            var fanfics = appDbContext.FanFictions.Include(f => f.Comments).Include(f => f.Category).Include(f => f.Marks).ToList();
            var result = new List<FanficInListViewModel>();
            foreach(var fanfic in fanfics)
            {
                double rating = 0;
                if (fanfic.Marks.Count == 0) rating = -1;
                else
                {
                    foreach (var m in fanfic.Marks)
                    {
                        rating += m.Value;
                    }
                    rating /= fanfic.Marks.Count;
                }
                result.Add(new FanficInListViewModel() { Name = fanfic.Name, Description = fanfic.Description, Id = fanfic.Id, Comments = fanfic.Comments.Count, Rating = rating, Category = fanfic.Category.Name });
            }
            return result;
        }

        public bool IsRated(User user, int fanficId)
        {
            var marks = appDbContext.Marks.Include(m => m.User).Include(m => m.Fanfic).Where(m => m.User.Id == user.Id && m.Fanfic.Id == fanficId);
            return (marks.Count() > 0);
        }

        public Mark SetRating(User user, int fanficId, int value)
        {
            var fanfic = appDbContext.FanFictions.Find(fanficId);
            var marks = appDbContext.Marks.Include(m => m.User).Include(m => m.Fanfic).Where(m => m.User.Id == user.Id && m.Fanfic.Id == fanficId);
            Mark mark;
            if(marks.Count() == 0)
            {
                mark = new Mark()
                {
                    User = user,
                    Fanfic = fanfic,
                    Value = value
                };
                appDbContext.Add(mark);
            }
            else
            {
                mark = marks.FirstOrDefault();
                mark.Value = value;
            }
            appDbContext.SaveChanges();
            return mark;
        }

        public int GetFanficRating(int fanficId)
        {
            int result = 0;
            var marks = appDbContext.Marks.Where(m => m.Fanfic.Id == fanficId).ToArray();
            if (marks.Length == 0) return -1;
            foreach(var mark in marks)
            {
                result += mark.Value;
            }
            result /= marks.Length;
            return result;
        }

        public bool IsLiked(User user, int chapterNumber, int fanficId)
        {
            var fanfic = appDbContext.FanFictions.Find(fanficId);
            var chapter = appDbContext.Chapters.Include(c => c.FanFiction).Include(c => c.Likes).Single(c => c.FanFiction.Id == fanficId && c.Number == chapterNumber);
            var likes = appDbContext.Likes.Where(l => l.Chapter.Id == chapter.Id && l.Author.Id == user.Id).ToList();
            return (likes.Count > 0);
        }

        public Like CreateLike(User user, int chapterNumber, int fanficId)
        {
            var fanfic = appDbContext.FanFictions.Find(fanficId);
            var chapter = appDbContext.Chapters.Include(c => c.FanFiction).Single(c => c.FanFiction.Id == fanficId && c.Number == chapterNumber);
            var like = new Like() { Author = user, Chapter = chapter};
            appDbContext.Likes.Add(like);
            appDbContext.SaveChanges();
            return like;
        }

        public Comment CreateComment(int fanficId, User author, string text)
        {
            var fanfic = appDbContext.FanFictions.Find(fanficId);
            var comment = new Comment()
            {
                Author = author,
                Content = text,
                FanFiction = fanfic
            };
            appDbContext.Comments.Add(comment);
            appDbContext.SaveChanges();
            return comment;
        }

        public UserFanficControlViewModel GetUserFanficControlViewModel(User user)
        {
            var result = new UserFanficControlViewModel() { Fanfics = new List<FanficInUserControlListViewModel>() };
            var fanfics = appDbContext.FanFictions.Include(f => f.Author).Where(f => f.Author.Id == user.Id).ToList();
            foreach(var f in fanfics)
            {
                result.Fanfics.Add(new FanficInUserControlListViewModel() { Id = f.Id, Name = f.Name });
            }
            return result;
        }

        public void RemoveFanfic(int fanficId, string userId)
        {
            var fanfic = appDbContext.FanFictions.Include(f => f.Author).Include(f => f.Comments).Include(f => f.Chapters).Include(f => f.Marks).Single(f => f.Id == fanficId);
            if(fanfic.Author.Id == userId) {
                foreach(var c in fanfic.Chapters)
                {
                    var chapterWithLikes = appDbContext.Chapters.Include(c => c.Likes).Single(chapter => chapter.Id == c.Id);
                    foreach(var l in chapterWithLikes.Likes)
                    {
                        appDbContext.Remove(l);
                    }
                    appDbContext.SaveChanges();
                    appDbContext.Chapters.Remove(c);
                }
                appDbContext.SaveChanges();
                foreach (var m in fanfic.Marks)
                {
                    appDbContext.Remove(m);
                }
                appDbContext.SaveChanges();
                appDbContext.SaveChanges();
                foreach (var c in fanfic.Comments)
                {
                    appDbContext.Comments.Remove(c);
                }
                appDbContext.SaveChanges();
                appDbContext.FanFictions.Remove(fanfic);
                appDbContext.SaveChanges();
            }
        }

        public ViewChapterViewModel GetChapterViewModel(int fanficId, int chapterNumber, User user)
        {
            var chapters = appDbContext.FanFictions.Include(f => f.Chapters).ThenInclude(c => c.Likes).Single(f => f.Id == fanficId).Chapters.ToArray();
            var comments = appDbContext.FanFictions.Include(f => f.Comments).ThenInclude(c => c.Author).Single(f => f.Id == fanficId).Comments.ToList();
            Chapter chapter;
            if(chapters.Length <= chapterNumber)
            {
                return null;
            }
            else
            {
                chapter = chapters.Single(c => c.Number == chapterNumber);
                return new ViewChapterViewModel() { 
                    Comments = comments,
                    Name = chapter.Name,
                    Content = Parse(chapter.Content), 
                    FanficId = fanficId, 
                    ChapterNumber = chapterNumber, 
                    Image = chapter.Image, 
                    IsLast = chapterNumber == chapters.Length - 1,
                    Likes = chapter.Likes,
                    IsLiked = (user == null)? true : IsLiked(user, chapterNumber, fanficId),
                    IsRated = (user == null) ? true : IsRated(user, fanficId)
                };
            }
        }
    }
}
