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


namespace Drug.Controllers
{
    public class DrugController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;

        public DrugController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, UserManager<User> userManager)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
        }

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Medication> drugs = _databaseContext.Drug.Include(p=>p.Manufacturer)
                .Include(p=>p.DrugSideEffects).ThenInclude(i=>i.SideEffect)
                .Include(r=>r.Currancy).Include(i=>i.Package).Include(p=>p.Substitutions)
                .Include(e => e.DrugDiseases).ThenInclude(eu => eu.Disease)
                .ToList();
            return View(drugs);
        }
        public ViewResult Show(int id)
        {
            Medication drug = _databaseContext.Drug.Include(t=>t.Comments).ThenInclude(p=>p.User)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currancy).Include(i => i.Package).Include(p => p.Substitutions)
                .Include(p => p.Manufacturer).Include(e => e.DrugDiseases)
                .ThenInclude(eu => eu.Disease).FirstOrDefault(m => m.DrugId == id);
            return View(drug);
        }

        [HttpGet]
        public ViewResult Add()
        {
            ViewData["Categories"] = _databaseContext.Disease.ToList();
            ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            ViewData["Drugs"] = _databaseContext.Drug.ToList();
            ViewData["Packages"] = _databaseContext.Package.ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();

            return View(new DrugViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DrugViewModel model)
        {
            ViewData["Categories"] = _databaseContext.Disease.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
            ViewData["Drugs"] = _databaseContext.Drug.ToList();
            ViewData["Packages"] = _databaseContext.Package.ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();



            if (ModelState.IsValid)
            {
                Manufacturer man;

                if (model.ManufacturerType == "new")
                {
                    // Additional validation before creating the Company
                    var requiredFields = new[]
                    {
                        new Tuple<string, object>("Name", model.Manufacturer.ManufacturerName),
                        new Tuple<string, object>("About", model.Manufacturer.About)

                    };

                    foreach (var field in requiredFields)
                    {
                        if (field.Item2 == null || field.Item2.Equals(""))
                        {
                            ModelState.AddModelError(string.Empty, $"{field.Item1} field is required.");
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    man = model.Manufacturer;
                    _databaseContext.Manufacturer.Add(man);
                }
                else
                {
                    man = _databaseContext.Manufacturer.Find(model.ManufacturerId);
                }



                var val = _databaseContext.Currency.FirstOrDefault(m => m.CurrencyId == model.CurrencyId);
                var pac = _databaseContext.Package.FirstOrDefault(m => m.PackageId == model.PackageId);
                var drugCategories = new List<DrugDisease>();
                var ses = new List<DrugSideEffect>();

                var drug = new Medication
                {

                    DrugName = model.Name,
                    DateProduced = model.Produced,
                    DateExpires = model.Expires,
                    Price = model.Price,
                    Quantity = model.Stock,
                    Manufacturer = man,
                    Package = pac,
                    Currancy = val,
                    PackageSize = model.PackageSize.ToString() + model.x,
                    DrugSize = model.DrugSize.ToString() + model.y,
                    Usage = model.Usage

                };
                

                if (model.z == true)
                {
                    var drugs = new List<Medication>();
                    drugs = model.DrugIds.Select(id => _databaseContext.Drug.Find(id)).ToList();
                    drug.Substitutions = drugs;

                }

                drugCategories = model.CategoryIds.Select(id => _databaseContext.Disease.Find(id))
               .Select(u => new DrugDisease { Disease = u, Drug = drug }).ToList();

                    ses = model.SideEffectIds.Select(id => _databaseContext.SideEffect.Find(id))
                   .Select(u => new DrugSideEffect { SideEffect = u, Drug = drug }).ToList();

                    foreach (var dc in drugCategories)
                    {
                        var drugCategoriesInDb = _databaseContext.DrugDisease.SingleOrDefault(s => s.Drug.DrugId == dc.Drug.DrugId &&
                                                                                         s.Disease.DiseaseId == dc.Disease.DiseaseId);
                        if (drugCategoriesInDb == null)
                        {
                            _databaseContext.DrugDisease.Add(dc);
                        }
                    }

                    foreach (var dc in ses)
                    {
                        var sesInDb = _databaseContext.DrugSideEffect.SingleOrDefault(s => s.Drug.DrugId == dc.Drug.DrugId &&
                                                                                         s.SideEffect.SideEffectId == dc.SideEffect.SideEffectId);
                        if (sesInDb == null)
                        {
                            _databaseContext.DrugSideEffect.Add(dc);
                        }
                    }

                var x = _databaseContext.Drug.FirstOrDefault(g => (g.DrugName == drug.DrugName && g.Manufacturer==drug.Manufacturer && g.Package==drug.Package));

                if (x != null)
                {
                    TempData[Constants.Message] = $"Takav lijek već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Drug.Add(drug);

                    TempData["Success"] = true;
                    _databaseContext.SaveChanges();
                    TempData[Constants.Message] = $"Lijek je dodan";
                    TempData[Constants.ErrorOccurred] = false;
            }
            else
            {
                ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
                ViewData["Categories"] = _databaseContext.Disease.ToList();
                ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
                ViewData["Drugs"] = _databaseContext.Drug.ToList();
                ViewData["Packages"] = _databaseContext.Package.ToList();
                ViewData["Currencies"] = _databaseContext.Currency.ToList();


                return View("Add", model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            var Drug = _databaseContext.Drug.Include(u=>u.Manufacturer)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currancy).Include(i => i.Package).Include(p => p.Substitutions)
                .Include(g => g.DrugDiseases)
                .ThenInclude(eu => eu.Disease)
                .First(g => g.DrugId == id);

            var categoryIds = Drug.DrugDiseases.Select(eu => eu.Disease.DiseaseId);
            var sesIds = Drug.DrugSideEffects.Select(eu => eu.SideEffect.SideEffectId);

            ViewData["Categories"] = _databaseContext.Disease.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
            ViewData["Drugs"] = _databaseContext.Drug.ToList();
            ViewData["Packages"] = _databaseContext.Package.ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();


            return View(new EditDrugViewModel { Drug = Drug, SideEffectIds = sesIds, CategoryIds = categoryIds,
                ManufacturerId =Drug.Manufacturer.ManufacturerId,PackageId=Drug.Package.PackageId,CurrencyId=Drug.Currancy.CurrencyId });
        }

        [HttpPost]
        public IActionResult Update(int id, EditDrugViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = _databaseContext.Disease.ToList();
                ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
                ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
                ViewData["Packages"] = _databaseContext.Package.ToList();
                ViewData["Currencies"] = _databaseContext.Currency.ToList();

                return View(nameof(Edit), model);
            }
            var Drug = _databaseContext.Drug
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currancy).Include(i => i.Package)
                .Include(g => g.DrugDiseases)
                .ThenInclude(eu => eu.Disease)
                .First(g => g.DrugId == id);

            Drug.Manufacturer= _databaseContext.Manufacturer.ToList().First(c => c.ManufacturerId == model.ManufacturerId);
            Drug.Currancy = _databaseContext.Currency.ToList().First(c => c.CurrencyId == model.CurrencyId);
            Drug.Package = _databaseContext.Package.ToList().First(c => c.PackageId == model.PackageId);

            Drug.DrugName = model.Drug.DrugName;
            Drug.DateProduced = model.Drug.DateProduced;
            Drug.DateExpires = model.Drug.DateExpires;
            Drug.Quantity = model.Drug.Quantity;
            Drug.Price = model.Drug.Price;
            Drug.DrugName = model.Drug.DrugName;

            TempData["Success"] = true;
            Drug.DrugDiseases.Clear();
            Drug.DrugSideEffects.Clear();

            var newCategories = _databaseContext.Disease.Where(u => model.CategoryIds.Contains(u.DiseaseId)).ToList();
            foreach (var cat in newCategories)
            {
                Drug.DrugDiseases.Add(new DrugDisease { Disease = cat, Drug = Drug });
            }

            var newSes = _databaseContext.SideEffect.Where(u => model.SideEffectIds.Contains(u.SideEffectId)).ToList();
            foreach (var cat in newSes)
            {
                Drug.DrugSideEffects.Add(new DrugSideEffect { SideEffect = cat, Drug = Drug });
            }
            _databaseContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _databaseContext.Drug.Remove(new Medication { DrugId = id });
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult AddComment(int id)
        {
            var Drug = _databaseContext.Drug.Include(u => u.Manufacturer).Include(t => t.Comments)
                .Include(r => r.Currancy).Include(i => i.Package).Include(p => p.Substitutions)
                .Include(g => g.DrugDiseases)
                .ThenInclude(eu => eu.Disease)
                .First(g => g.DrugId == id);

            return View(new CommentViewModel { Drug = Drug });
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(int id, CommentViewModel model)
        {
            var sender = await _userManager.GetUserAsync(User);

            var Drug = _databaseContext.Drug.Include(t => t.Comments).ThenInclude(eu=>eu.User)
                   .Include(r => r.Currancy).Include(i => i.Package).Include(p => p.Substitutions)
                   .Include(g => g.DrugDiseases)
                   .ThenInclude(eu => eu.Disease)
                   .First(g => g.DrugId == id);
            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = model.Content,
                    Drug = model.Drug,
                    User=sender
                };
                TempData["Success"] = true;

                _databaseContext.Comment.Add(comment);

                System.Diagnostics.Debug.WriteLine("AAA" + comment.CommentId.ToString());
                System.Diagnostics.Debug.WriteLine("USER" + sender.FullName.ToString());

                sender.Comments.Add(comment);
                Drug.Comments.Add(comment);
                _databaseContext.SaveChanges();

            }
            return RedirectToAction("Show", "Drug", new { Id = Drug.DrugId });
        }

        [HttpGet]
        public ViewResult EditComment(int id)
        {
            var Comment = _databaseContext.Comment
                 .First(g => g.CommentId == id);

            return View(new EditCommentViewModel { Comment = Comment });
        }

        [HttpPost]
        public IActionResult UpdateComment(int id, EditCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EditCommentViewModel), model);
            }
            var Comment = _databaseContext.Comment
                .First(g => g.CommentId == id);

            Comment.Content = model.Comment.Content;
            _databaseContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }

    }

