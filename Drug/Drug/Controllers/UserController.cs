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

        public async Task<ViewResult> Index()
        {
            ViewData["Success"] = TempData["Success"];
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<User> users = _databaseContext.Users.Include(t=>t.City).ThenInclude(t=>t.Country).ToList().Where(p=>p.IsDoctor==false).Where(r=>r.IsAdmin==false).Where(t=>t.Id!=user.Id.ToString());
            return View(users);
        }

        [HttpPost]
        public IActionResult Delete(string id)
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

            _databaseContext.Database.ExecuteSqlCommand("DELETE FROM \"AspNetUsers\" WHERE \"AspNetUsers\".\"Id\" = {0}", id);
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}
