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

            var students = from s in _databaseContext.Drug.Include(p => p.Manufacturer)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t=>t.Measure).Include(p => p.Substitutions)
                .Include(e => e.DrugDiseases).ThenInclude(eu => eu.Disease)
                
            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.DrugName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.DrugName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.DrugId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.DrugId);
                    break;
                default:
                    students = students.OrderBy(s => s.DrugName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Medication>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }
        public ViewResult Show(int id)
        {
            Medication drug = _databaseContext.Drug.Include(t=>t.Comments).ThenInclude(p=>p.User)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
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
            ViewData["Packages"] = _databaseContext.Package.Include(t => t.Measure).ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();
            ViewData["Measures"] = _databaseContext.Measure.ToList();


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
            ViewData["Packages"] = _databaseContext.Package.Include(t=>t.Measure).ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();
            ViewData["Measures"] = _databaseContext.Measure.ToList();



            if (ModelState.IsValid)
            {
                Manufacturer man;
                Manufacturer x;

                if (model.ManufacturerType == "new")
                {
                  
                    if (model.Manufacturer.ManufacturerName==null || model.Manufacturer.About == null|| model.Manufacturer.ImagePath == null)
                    {
                        TempData[Constants.Message] = $"Sva polja za proizvođača su obavezna.\n";
                        TempData[Constants.ErrorOccurred] = true;
                        System.Diagnostics.Debug.WriteLine("fuego");
                        return View("Add", model);
                    }

                    man = model.Manufacturer;
                    x = _databaseContext.Manufacturer.FirstOrDefault(g => g.ManufacturerName == man.ManufacturerName);

                    if (x != null)
                    {
                        TempData[Constants.Message] = $"Proizvođač tog imena već postoji.\n";
                        TempData[Constants.ErrorOccurred] = true;
                        return View("Add", model);
                    }
                    _databaseContext.Manufacturer.Add(man);
                }
                else
                {
                    man = _databaseContext.Manufacturer.Find(model.ManufacturerId);
                }

                ///////////////
                Package pac;
                Package y;

                if (model.PackageType == "new")
                {
                    if (model.Package.PackageType == null || model.Package.Quantity ==0 || model.Package.IndividualSize==0 )
                    {
                        TempData[Constants.Message] = $"Sva polja za pakiranje su obavezna te moraju biti različita od 0.\n";
                        TempData[Constants.ErrorOccurred] = true;
                        System.Diagnostics.Debug.WriteLine("fuego");
                        return View("Add", model);
                    }

                    var m = _databaseContext.Measure.FirstOrDefault(t => t.MeasureId == model.MeasureId);
                    pac = model.Package;
                    pac.Measure = m;

                    y = _databaseContext.Package.FirstOrDefault(g => (g.PackageType == pac.PackageType && g.IndividualSize == pac.IndividualSize && g.Quantity == pac.Quantity && g.Measure == pac.Measure));

                    if (y != null)
                    {
                        TempData[Constants.Message] = $"Takvo pakiranje već postoji.\n";
                        TempData[Constants.ErrorOccurred] = true;
                        return View("Add", model);
                    }
                    _databaseContext.Package.Add(pac);
                }
                else
                {
                    pac = _databaseContext.Package.Include(t=>t.Measure).FirstOrDefault(t=>t.PackageId==model.PackageId);
                }
                ///////////////

                try
                {
                    if (model.SubstitutionType == "existing" && !model.DrugIds.Any())
                    {

                    }

                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine("konj");
                    TempData[Constants.Message] = $"Zamjenski proizvodi su obavezni.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return View("Add", model);
                }

                var val = _databaseContext.Currency.FirstOrDefault(m => m.CurrencyId == model.CurrencyId);
                //var pac = _databaseContext.Package.FirstOrDefault(m => m.PackageId == model.PackageId);
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
                    Currency = val,
                    Usage = model.Usage,
                    ImagePath=model.ImagePath
                 

                };
                

                if (model.SubstitutionType == "existing")
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

                var z = _databaseContext.Drug.FirstOrDefault(g => (g.DrugName == drug.DrugName && g.Manufacturer==drug.Manufacturer && g.Package==drug.Package));

                if (z != null)
                {
                    TempData[Constants.Message] = $"Takav lijek već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return View("Add", model);
                }
                if (drug.DateProduced>=drug.DateExpires)
                {
                    TempData[Constants.Message] = $"Datum isteka roka mora biti veći od datuma proizvodnje.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return View("Add", model);
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
                ViewData["Packages"] = _databaseContext.Package.Include(t => t.Measure).ToList();
                ViewData["Currencies"] = _databaseContext.Currency.ToList();
                ViewData["Measures"] = _databaseContext.Measure.ToList();



                return View("Add", model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            var Drug = _databaseContext.Drug.Include(u=>u.Manufacturer)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
                .Include(g => g.DrugDiseases)
                .ThenInclude(eu => eu.Disease)
                .First(g => g.DrugId == id);

            var categoryIds = Drug.DrugDiseases.Select(eu => eu.Disease.DiseaseId);
            var sesIds = Drug.DrugSideEffects.Select(eu => eu.SideEffect.SideEffectId);

            ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
            ViewData["Categories"] = _databaseContext.Disease.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            ViewData["Drugs"] = _databaseContext.Drug.Where(g=>g.DrugId!=id).ToList();
            ViewData["Packages"] = _databaseContext.Package.Include(t => t.Measure).ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();
            ViewData["Measures"] = _databaseContext.Measure.ToList();


            return View(new EditDrugViewModel { Drug = Drug, SideEffectIds = sesIds, CategoryIds = categoryIds,
                ManufacturerId =Drug.Manufacturer.ManufacturerId,PackageId=Drug.Package.PackageId,CurrencyId=Drug.Currency.CurrencyId });
        }

        [HttpPost]
        public IActionResult Update(int id, EditDrugViewModel model)
        {
            ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
            ViewData["Categories"] = _databaseContext.Disease.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            ViewData["Drugs"] = _databaseContext.Drug.Where(g => g.DrugId != id).ToList();
            ViewData["Packages"] = _databaseContext.Package.Include(t => t.Measure).ToList();
            ViewData["Currencies"] = _databaseContext.Currency.ToList();
            ViewData["Measures"] = _databaseContext.Measure.ToList();
            if (!ModelState.IsValid)
            {
                ViewData["SideEffects"] = _databaseContext.SideEffect.ToList();
                ViewData["Categories"] = _databaseContext.Disease.ToList();
                ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
                ViewData["Drugs"] = _databaseContext.Drug.ToList();
                ViewData["Packages"] = _databaseContext.Package.Include(t => t.Measure).ToList();
                ViewData["Currencies"] = _databaseContext.Currency.ToList();
                ViewData["Measures"] = _databaseContext.Measure.ToList();

                return View(nameof(Edit), model);
            }

            var man = _databaseContext.Manufacturer.FirstOrDefault(t => t.ManufacturerId == model.ManufacturerId);

            var Drug = _databaseContext.Drug
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure)
                .Include(g => g.DrugDiseases)
                .ThenInclude(eu => eu.Disease)
                .FirstOrDefault(g => g.DrugId == id);

            Drug.Manufacturer= man;
            Drug.Currency = _databaseContext.Currency.ToList().First(c => c.CurrencyId == model.CurrencyId);
            Drug.Package = _databaseContext.Package.ToList().First(c => c.PackageId == model.PackageId);

            Drug.DrugName = model.Drug.DrugName;
            Drug.DateProduced = model.Drug.DateProduced;
            Drug.DateExpires = model.Drug.DateExpires;
            Drug.Quantity = model.Drug.Quantity;
            Drug.Price = model.Drug.Price;
            Drug.DrugName = model.Drug.DrugName;
            Drug.ImagePath = model.Drug.ImagePath;

            if (model.z == true)
            {
                var drugs = new List<Medication>();
                drugs = model.DrugIds.Select(x => _databaseContext.Drug.Find(x)).ToList();
                Drug.Substitutions = drugs;

            }

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
        public IActionResult Delete(int id, int? page)
        {
            var messages = _databaseContext.OrderDetail.Include(i => i.Drug).Where(t =>t.Drug.DrugId == id ).ToList();
            foreach (var t in messages)
            {
                _databaseContext.OrderDetail.Remove(t);
            }
            _databaseContext.SaveChanges();

            _databaseContext.Drug.Remove(new Medication { DrugId = id });
            _databaseContext.SaveChanges();


            var x = _databaseContext.Drug.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }

        [HttpGet]
        public ViewResult AddComment(int id)
        {
            var Drug = _databaseContext.Drug.Include(u => u.Manufacturer).Include(t => t.Comments)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
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
                   .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
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

                sender.Comments.Add(comment);
                Drug.Comments.Add(comment);
                _databaseContext.SaveChanges();
                return RedirectToAction("Show", "Drugs", new { Id = Drug.DrugId });


            }
            return View("AddComment", model);

        }

        [HttpGet]
        public ViewResult EditComment(int id1,int id2)
        {
            var Comment = _databaseContext.Comment
                 .First(g => g.CommentId == id1);
            var drug = _databaseContext.Drug.FirstOrDefault(g => g.DrugId == id2);

            return View(new EditCommentViewModel { Comment = Comment,Drug=drug });
        }

        [HttpPost]
        public IActionResult UpdateComment(int id1,int id2, EditCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EditCommentViewModel), model);
            }
            var Comment = _databaseContext.Comment
                .First(g => g.CommentId == id1);

            var drug = _databaseContext.Drug.FirstOrDefault(t => t.DrugId == id2);

            Comment.Content = model.Comment.Content;
            _databaseContext.SaveChanges();

            return RedirectToAction("Show", "Drugs", new { Id = drug.DrugId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int id1,int id2)
        {
            var ses = _databaseContext.Comment
            .FirstOrDefault(p => p.CommentId == id1);
            var drug = _databaseContext.Drug.FirstOrDefault(p => p.DrugId==id2);

            try
            {
                _databaseContext.Comment.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Pakiranje je obrisano";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Pakiranje nije moguće obrisati jer postoje lijekovi koji ga sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }

            return RedirectToAction("Show", "Drugs", new { Id = drug.DrugId });
        }
    }

    }

