using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class MessageController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;


        public MessageController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _databaseContext = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var id = user.Id;
            IEnumerable<Message> messages = _databaseContext.Message
                .Include(message => message.Receiver)
                .Include(message => message.Sender).FromSql("SELECT * FROM \"Message\"" +
                                                            " WHERE \"Message\".\"MessageDate\" = (SELECT MAX(\"m\".\"MessageDate\") FROM \"Message\" AS \"m\"" +
                                                            " WHERE (\"m\".\"SenderId\" = \"Message\".\"SenderId\" AND \"m\".\"ReceiverId\" = \"Message\".\"ReceiverId\"" +
                                                            " AND (\"Message\".\"SenderId\" = {0} OR \"Message\".\"ReceiverId\" = {1}))" +
                                                            " OR (\"m\".\"SenderId\" = \"Message\".\"ReceiverId\" AND \"m\".\"ReceiverId\" = \"Message\".\"SenderId\"" +
                                                            " AND (\"Message\".\"SenderId\" = {0} OR \"Message\".\"ReceiverId\" = {1})))",
                    id, id)
                .ToList();
            return View(messages);
        }

        [HttpGet]
        public async Task<ViewResult> Add()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.IsAdmin == true)
            {
                ViewData["Users"] = _databaseContext.Users.ToList().Where(t=>t.Id!=user.Id);
                return View(new MessageViewModel());
            }
            else
            {
                ViewData["Users"] = _databaseContext.Doctor.ToList()/*.Where(t=>t.Id!=user.Id)*/;
                return View(new MessageViewModel());
            }
            
        }

        [HttpGet]
        public async Task<ViewResult> SendTo(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var reciever = _databaseContext.Users.FirstOrDefault(g => g.Id == id);

            ViewData["Users"] = _databaseContext.Users.ToList().Where(t => t.Id == reciever.Id);
            return View("Add", new MessageViewModel());

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(MessageViewModel model)
        {
            var sender = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                    sender = await _userManager.GetUserAsync(User);
                    var receiver = _databaseContext.Users.Find(model.ReceiverId);
                    _databaseContext.Message.Add(new Message
                    {
                        Body = model.About,
                        MessageDate = DateTime.Now,
                        Sender = sender,
                        Receiver = receiver
                    });
                    TempData["Success"] = true;
                    _databaseContext.SaveChanges();

            }
            else
            {
                var reciever = _databaseContext.Users.Find(model.ReceiverId);

                ViewData["Users"] = _databaseContext.Users.ToList().Where(p => p.Id == reciever.Id);
                return View("Add", model);

            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageViewModel model)
        {
            var sender = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                sender = await _userManager.GetUserAsync(User);
                if (sender.IsAdmin == true)
                {
                    var receiver = _databaseContext.Users.Find(model.ReceiverId);
                    _databaseContext.Message.Add(new Message
                    {
                        Body = model.About,
                        MessageDate = DateTime.Now,
                        Sender = sender,
                        Receiver = receiver
                    });
                    TempData["Success"] = true;
                    _databaseContext.SaveChanges();

                }else{

                    var receiver = _databaseContext.Doctor.Find(model.ReceiverId);
                    _databaseContext.Message.Add(new Message
                    {
                        Body = model.About,
                        MessageDate = DateTime.Now,
                        Sender = sender,
                        Receiver = receiver
                    });
                    TempData["Success"] = true;
                    _databaseContext.SaveChanges();
                }
                
            }
            else
            {
                if (sender.IsAdmin == true)
                {
                    ViewData["Users"] = _databaseContext.Users.ToList().Where(p=>p.Id!=sender.Id);
                    return View("Add", model);
                }else
                {
                    ViewData["Users"] = _databaseContext.Doctor.ToList()/*.Where(p=>p.Id!=sender.Id)*/;
                    return View("Add", model);
                }

            }

            return RedirectToAction(nameof(Index));
        }

        /*
         * Razgovor s `senderId` osobom. Dohvatimo poruke s njime. I njemu saljemo odgovor (`receiverId`).
         * 
         */
        public async Task<IActionResult> Show(int id)
        {
            var message = _databaseContext.Message.Include(m => m.Sender).Include(m => m.Receiver).Where(m => m.MessageId == id).First();
            var sender = message.Sender;
            var receiver = message.Receiver;
            //var talkingToUser = _databaseContext.Users.Find(id);
            var user = await _userManager.GetUserAsync(User);
            var messages = _databaseContext.Message
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.Sender.Id == sender.Id && m.Receiver.Id == receiver.Id) ||
                            m.Sender.Id == receiver.Id && m.Receiver.Id == sender.Id)
                .ToList();
            if (receiver.Id == user.Id)
            {
                receiver = sender;
            }
            return View(new MessageShowViewModel { Messages = messages, CurrentMessageId = id, Receiver = receiver });
        }

        [HttpPost]
        public async Task<IActionResult> CreateFromShow(MessageShowViewModel model, string id, int MessageId)
        {
            if (ModelState.IsValid)
            {
                var sender = await _userManager.GetUserAsync(User);
                var receiver = _databaseContext.Users.Find(id);
                var date = DateTime.Now;
                _databaseContext.Message.Add(new Message
                {
                    Body = model.About,
                    MessageDate = date,
                    Sender = sender,
                    Receiver = receiver
                });
                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                var message = _databaseContext.Message.Include(m => m.Sender).Include(m => m.Receiver).Where(m => m.Body == model.About && m.Sender == sender && m.Receiver == receiver && m.MessageDate == date).First();
                return RedirectToAction(nameof(Show), new { id = message.MessageId });
            }

            return RedirectToAction(nameof(Show), new { id = MessageId });
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            _databaseContext.Message.Remove(new Message { MessageId = id });
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
