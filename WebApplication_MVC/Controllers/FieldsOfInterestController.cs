using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication_MVC.Models;
using WebApplication_MVC.Resources;

namespace WebApplication_MVC.Controllers
{
    // The FieldsOfInterestController is a second attempt to set up a controller, this time using
    // a special view model entity instead of accessing the direct database entity, and moving all
    // data access to a repository which services the data access and context. This enforces the
    // seperation of concerns idea better as well as allowing for another testing setup.
    // The use of a view model already can protect for overposting errors, but only if it contains only
    // fields allowed for edit, which is not the case for this view, so still use binding attributes as
    // a whitelist of allowed editable fields.
    //
    // Some details worked out in this controller
    //  o Repository setup to retrieve data for view model
    //  o Simple paging on the list shown on the index page
    //  o Simple sorting on the list shown on the index page
    //  o Simple searching on the list shown on the index page
    //  o Multilingual error messages by use of language resources and data annotations on view (general localization resources, not per controller)
    //  o Store-wins concurrency protocol implemented (instead of the default Client-wins protocol)
    //  o Using TempData and ViewData dictionairies to pass information from controller to a (redirected) view
    //
    // Some details not worked out in this controller
    //  o For tracing errors the catched exceptions should be passed to a logger with all relevant model 
    //    and state information, something not yet done here. 

    public class FieldsOfInterestController : Controller
    {
        private readonly ICrudRepositoryAsync<FieldOfInterestView, string> _repo;

        public FieldsOfInterestController(ICrudRepositoryAsync<FieldOfInterestView, string> repo)
        {
            _repo = repo;
        }

        // GET: FieldsOfInterest
        public async Task<IActionResult> Index(string sortOrder,             // if filled it contains the desired/new sortorder
                                               string currentSearchString,   // contains the displayed/old search string
                                               string searchString,          // if filled it contains the desired/new search string
                                               int? page)                    // if filled it contains a specific/new page number
        {
            // The sort parameters are used in a simple <a> tag in the table header of the various properties.
            // This sorting is server sided and retrieves the data again. When there are even more columns to
            // sort there is an option to use the EF.Property method to retrieve the property from a string and 
            // limit the sorting case to one if with 1 ascending and 1 descending clause. Using strings like this
            // will reduce refactoring possibilities, so consider using the nameof() route.
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["FieldOfInterestIdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "fieldofinterestid_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "description" ? "description_desc" : "description";
            ViewData["CreatedSortParm"] = sortOrder == "created" ? "created_desc" : "created";

            // The parameters used for the search box, saving the search value so it can be presented back in view
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentSearchString;  // restore displayed value
            }
            ViewData["CurrentSearchString"] = searchString;

            // TODO: parsing sortOrder en searchString to repository and let that figure it out ?

            var fields = await _repo.ReadAllAsync();

            switch (sortOrder)
            {
                case "fieldofinterestid_desc":
                    fields = fields.OrderByDescending(x => x.FieldOfInterestId);
                    break;
                case "description":
                    fields = fields.OrderBy(x => x.Description);
                    break;
                case "description_desc":
                    fields = fields.OrderByDescending(x => x.Description);
                    break;
                case "created":
                    fields = fields.OrderBy(x => x.Created);
                    break;
                case "created_desc":
                    fields = fields.OrderByDescending(x => x.Created);
                    break;
                default:
                    fields = fields.OrderBy(x => x.FieldOfInterestId);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // SQL would normally search case insensative, but this is performed on a list, so
                // to simulate case insensative search (which most people expect) added ToUpper's.

                searchString = searchString.ToUpper();
                fields = fields.Where(x => x.FieldOfInterestId.ToUpper().Contains(searchString)
                                        || x.Description.ToUpper().Contains(searchString));
            }

            // TODO: this is not the proper method, since everything is loaded and only then reduced
            //       while it is much better to compose the correct Queryable statement and let SQL limit the data
            // TODO: a better way to do this is also not working based on page numbers, since in a 
            //       multiuser system someone else can add items, so i would use next key and prev key id's
            //       but beware the id depends on the sorting and search fields... For simple items this works ok
            int pageSize = 5;  //TODO add as parameter from page or constant
            var paginatedFields = PaginatedList<FieldOfInterestView>.Create(fields.AsQueryable(), page ?? 1, pageSize);
            //var paginatedFields = await PaginatedList<FieldOfInterestView>.CreateAsync(fields.AsQueryable(), page ?? 1, pageSize);

            return View(paginatedFields);
        }

        // GET: FieldsOfInterest/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {               
                TempData["RedirectMessage"] = ErrorMessage.FieldOfInterestIdNull; 
                return RedirectToAction(nameof(Index));
            }

            var field = await _repo.ReadAsync(id);

            if (field == null)
            {
                TempData["RedirectMessage"] = String.Format(ErrorMessage.RecordUnknown, ErrorMessage.FieldOfInterestId, id);
                return RedirectToAction(nameof(Index));
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
                    ModelState.AddModelError("", ErrorMessage.DatabaseSaveError);
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
        // Parameter field is bound, but initializes all other fields, so do not use field object for actual update
        public async Task<IActionResult> Edit(string id, byte[] rowVersion, [Bind(nameof(FieldOfInterestView.FieldOfInterestId),
                                                                                  nameof(FieldOfInterestView.Description),
                                                                                  nameof(FieldOfInterestView.LanguageId))] FieldOfInterestView field)
        {
            if (id == null || id != field.FieldOfInterestId)  // url id does not match hidden form field, then something's fishy
            {
                TempData["RedirectMessage"] = String.Format(ErrorMessage.RecordIdError, ErrorMessage.FieldOfInterestId.ToLower());
                return RedirectToAction(nameof(Index));
            }

            bool concurrencyError = false;
            bool fieldDeleted = false;
            var fieldToUpdate = await _repo.ReadAsync(id);

            if (fieldToUpdate == null)
            {
                fieldDeleted = true;
                fieldToUpdate = new FieldOfInterestView();
            }

            // also update model when deleted to show the data the user entered back to the user
            await TryUpdateModelAsync<FieldOfInterestView>(fieldToUpdate, "",
                                                                x => x.FieldOfInterestId,
                                                                x => x.Description,
                                                                x => x.LanguageId);

            if (!fieldDeleted && ModelState.IsValid)
            {
                if (!fieldToUpdate.RowVersion.SequenceEqual(rowVersion))   // use this method to check equality on element level (not ==)
                {
                    concurrencyError = true;
                }
                else
                {
                    try
                    {
                        await _repo.UpdateAsync(fieldToUpdate);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (await _repo.ExistAsync(id))
                        {
                            concurrencyError = true;
                        }
                        else
                        {
                            fieldDeleted = true;
                        }
                    }
                    catch (DbUpdateException) // parent of DbUpdateConcurrencyException
                    {
                        ModelState.AddModelError("", ErrorMessage.DatabaseSaveError);
                    }
                }
            }

            if (fieldDeleted)
            {
                ModelState.AddModelError("", ErrorMessage.DatabaseUpdateDeletedError);
            }

            if (concurrencyError)
            {
                var fieldInDatabase = await _repo.RetrieveOriginalValuesAsync(fieldToUpdate);

                // TODO: retrieve error message from page, size save might not be called Save
                ModelState.AddModelError("", ErrorMessage.DatabaseUpdateConcurrencyError);

                // Next lines are to make it possible to press save again and save it anyways (unless another 
                // concurrency has occured in the meantime). This is one way to handle these situations, but 
                // could also opt to not allow updates until new data is loaded or redirect the user directly 
                // back to the index page with a warning. If this methodology is choosen, then a good practice 
                // is to retrieve and show the changed database values as done below.
                // It depends on the entity how much trouble to put into this nice processing or if it is even 
                // possible without reloading all data.

                if (fieldToUpdate.Description != fieldInDatabase.Description)
                {
                    ModelState.AddModelError(nameof(fieldToUpdate.Description), String.Format(ErrorMessage.DatabaseCurrentValue, fieldInDatabase.Description));
                }

                // set rowversion to current version so pressing Save button again WILL save the data
                fieldToUpdate.RowVersion = fieldInDatabase.RowVersion;
                // remove field from modelstate since it contains the old value and modelstate takes precendence over the model property if both are present
                ModelState.Remove("RowVersion");
            }

            return View(fieldToUpdate);
        }

        // GET: FieldsOfInterest/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id);
        }

        // POST: FieldOfInterest/Delete/5
        [HttpPost, ActionName("Delete")]   // ActionName needed when the GET and POST methods would otherwise have the same method signature (not needed here perse)
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, byte[] rowVersion, [Bind(nameof(FieldOfInterestView.FieldOfInterestId))] FieldOfInterestView field)
        {
            if (id == null || id != field.FieldOfInterestId)  // url id does not match hidden form field, then something's fishy
            {
                TempData["RedirectMessage"] = String.Format(ErrorMessage.RecordIdError, ErrorMessage.FieldOfInterestId.ToLower());
                return RedirectToAction(nameof(Index));
            }

            bool concurrencyError = false;
            bool fieldDeleted = false;
            var fieldToDelete = await _repo.ReadAsync(id);

            if(fieldToDelete == null)
            {
                fieldDeleted = true;
            }

            if (!fieldDeleted)
            {
                if (!fieldToDelete.RowVersion.SequenceEqual(rowVersion))  // use this method to check equality on element level (not ==)
                {
                    concurrencyError = true;
                }
                else
                {
                    try
                    {
                        await _repo.DeleteAsync(fieldToDelete);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (await _repo.ExistAsync(id))
                        {
                            concurrencyError = true;
                        }
                        else
                        {
                            fieldDeleted = true;
                        }
                    }
                    catch (DbUpdateException) // parent of DbUpdateConcurrencyException
                    {
                        ViewData["ErrorMessage"] = ErrorMessage.DatabaseSaveError;
                    }
                }
            }

            if (fieldDeleted)
            {
                return RedirectToAction(nameof(Index));  // no longer available, so deleted, so just act as if ok
            }

            if (concurrencyError)
            {
                // Delete does not set modelstate, so the error is set as a generic parameter which is added and displayed in the html page
                ViewData["ErrorMessage"] = ErrorMessage.DatabaseDeleteConcurrencyError;

                // remove field from modelstate since it contains the old value and modelstate takes precendence over the model property if both are present
                ModelState.Remove("RowVersion");
            }

            return View(fieldToDelete);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> RemoteValidateFieldOfInterestId(string fieldOfInterestId)
        {
            if (await _repo.ExistAsync(fieldOfInterestId))
            {
                return Json(String.Format(ErrorMessage.FieldOfInterestIdExists, fieldOfInterestId)); 
            }

            return Json(true);
        }
    }
}
