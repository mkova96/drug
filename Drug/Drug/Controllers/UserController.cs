using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class UserController:Controller
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public UserController(ApplicationDbContext context, UserManager<User> userManager, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _userManager = userManager;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "Id_desc" : "Id";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var user = await _userManager.GetUserAsync(User);
            var students = from s in _databaseContext.User.Include(t => t.City).ThenInclude(t => t.Country)
                          .Where(p => p.IsAdmin == false).Where(p => p.IsDoctor == false).Where(t=>t.UserName!="sanitas@ljekarna.com")

                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString) || s.Surname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Surname);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.UserDate);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.UserDate);
                    break;
                default:
                    students = students.OrderBy(s => s.Surname);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<User>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, int? page)
        {
            var user = _databaseContext.Users.Find(id);

            var comments = _databaseContext.Comment.Include(i=>i.User).Where(t => t.User.Id == id).ToList();
            foreach(var t in comments)
            {
                _databaseContext.Comment.Remove(t);
            }
            _databaseContext.SaveChanges();

            var messages = _databaseContext.Message.Include(i => i.Sender).Include(t=>t.Receiver).Where(t => (t.Sender.Id == id || t.Receiver.Id==id)).ToList();
            foreach (var t in messages)
            {
                _databaseContext.Message.Remove(t);
            }
            _databaseContext.SaveChanges();

            var orders = _databaseContext.Order.Include(i => i.User).Where(t => t.User.Id == id).ToList();
            foreach (var t in orders)
            {
                _databaseContext.Order.Remove(t);
            }
            _databaseContext.SaveChanges();

            try
            {
                _databaseContext.Database.ExecuteSqlCommand("DELETE FROM \"AspNetUsers\" WHERE \"AspNetUsers\".\"Id\" = {0}", id);
                _databaseContext.SaveChanges();

                var userx = await _userManager.GetUserAsync(User);


                var x = _databaseContext.Users.Where(p => p.IsAdmin == true).Where(t => t.Id != userx.Id.ToString()).ToList().Count;


                if ((page - 1) * 8 == x - 1 && page != 1)
                {
                    --page;
                }
                TempData[Constants.Message] = $"Korisnik je obrisan";
                TempData[Constants.ErrorOccurred] = false;

                return RedirectToAction(nameof(Index), new { page = page });
            }
            catch(Exception e)
            {
                TempData[Constants.Message] = $"Korisnika nije moguće obrisati";
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction(nameof(Index), new { page = page });
            }

        }



    }
}
