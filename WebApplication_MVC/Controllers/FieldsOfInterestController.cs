using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication_MVC.Models;

namespace WebApplication_MVC.Controllers
{
    // The FieldsOfInterestController is a second attempt to set up a controller, this time using
    // a special view model entity instead of accessing the direct database entity, and moving all
    // data access to a repository which services the data access and context. This enforces the
    // seperation of concerns idea better as well as allowing for another testing setup.
    // The use of a view model already can protect for overposting errors, but only if it contains only
    // fields allowed for edit, which is not the case for this view, so still use binding attributes as
    // a whitelist of allowed editable fields.
    // For tracing errors the catched exceptions should be passed to a logger with all relevant model 
    // and state information, something not yet done here. 

    public class FieldsOfInterestController : Controller
    {
        private readonly ICrudRepositoryAsync<FieldOfInterestView, string> _repo;

        public FieldsOfInterestController(ICrudRepositoryAsync<FieldOfInterestView, string> repo)
        {
            _repo = repo;
        }

        // GET: FieldsOfInterest
        public async Task<IActionResult> Index()
        {
            return View(await _repo.ReadAllAsync());
        }

        // GET: FieldsOfInterest/Details/5
        public async Task<IActionResult> Details(string id)
        {
            FieldOfInterestView field = null;

            if (id != null)
            {
                field = await _repo.ReadAsync(id);
            }

            if (field == null)
            {
                return NotFound();  // Redirect to index with error message ?
            }

            return View(field);
        }

        // GET: FieldsOfInterest/Create
        public IActionResult Create()
        {
            FieldOfInterestView field = new FieldOfInterestView();

            return View(field);
        }

        // POST: FieldsOfInterest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(nameof(FieldOfInterestView.FieldOfInterestId),
                                                      nameof(FieldOfInterestView.Description),
                                                      nameof(FieldOfInterestView.LanguageId))] FieldOfInterestView field)
        {
            // TryValidateModel(field);    // only needed if changes were made since binding

            if (_repo.CreateValid(field) && ModelState.IsValid)
            {
                try
                { 
                    await _repo.CreateAsync(field);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException) // includes DbUpdateConcurrencyException
                {
                    ModelState.AddModelError("", "Unable to save data. Try again, and if the problem persists contact your system administrator");
                }
            }

            return View(field);
        }

        // GET: FieldsOfInterest/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id);
        }

        // POST: FieldsOfInterest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Parameter field is bound, but initializes all other fields, so do not use for actual update
        public async Task<IActionResult> Edit(string id, [Bind(nameof(FieldOfInterestView.FieldOfInterestId),
                                                               nameof(FieldOfInterestView.Description),
                                                               nameof(FieldOfInterestView.LanguageId))] FieldOfInterestView field)
        {
            if (id == null || id != field.FieldOfInterestId)  // url id does not match hidden form field, then something's fishy
            {
                return NotFound(); // TODO return to edit with error message
            }

            var fieldToUpdate = await _repo.ReadAsync(id);

            if (await TryUpdateModelAsync<FieldOfInterestView>(fieldToUpdate, "",
                                                                x => x.FieldOfInterestId,
                                                                x => x.Description,
                                                                x => x.LanguageId))  // returns bool of ModelState.IsValid
            {
                try
                {
                    await _repo.UpdateAsync(fieldToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (await _repo.ExistAsync(id))
                    {
                        ModelState.AddModelError("", "Data was changed by another user. Refresh data and try again");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Data was deleted by another user. Refresh data and try again");
                    }
                }
                catch (DbUpdateException) // parent of DbUpdateConcurrencyException
                {
                    ModelState.AddModelError("", "Unable to save data. Try again, and if the problem persists contact your system administrator");
                }
            }

            return View(fieldToUpdate);
        }

        // GET: FieldsOfInterest/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id);
        }

        // POST: FieldOfInterest/Delete/5
        [HttpPost, ActionName("Delete")]   // ActionName needed when the GET and POST methods would otherwise have the same signature
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var fieldToDelete = await _repo.ReadAsync(id);
            if(fieldToDelete == null)
            {
                return RedirectToAction(nameof(Index));  // no longer available, so deleted, so just act as if ok
            }

            try
            {
                await _repo.DeleteAsync(fieldToDelete);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _repo.ExistAsync(id))
                {
                    // Delete does not set modelstate, so the error is set as a generic parameter which is added and displayed in the html page
                    ViewData["ErrorMessage"] = "Data was changed by another user. Refresh data and try again";
                }
                else
                {
                    return RedirectToAction(nameof(Index));  // no longer available, so deleted, so just act as if ok
                }
            }
            catch (DbUpdateException) // parent of DbUpdateConcurrencyException
            {
                ViewData["ErrorMessage"] = "Unable to save data. Try again, and if the problem persists contact your system administrator";
            }

            return View(fieldToDelete);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> RemoteValidateFieldOfInterestId(string fieldOfInterestId)
        {
            if (await _repo.ExistAsync(fieldOfInterestId))
            {
                return Json($"Field of interest {fieldOfInterestId} is already in use."); //TODO language dependant message (user/system/website)
            }

            return Json(true);
        }
    }
}
