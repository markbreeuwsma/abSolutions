using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication_MVC.Controllers
{
    // The CountriesController is a first attempt to set up a controller for a relative simple model where
    // Country and CountryDescription are maintained as one entity for the user.
    // Although the controller manages this (eventually), there were additions needed to the Country model 
    // entity to make it work and that is not what you want. The direct approach on a database entity works
    // for extremely plain records, but otherwise should work with a repository and a View model. As shown
    // below even this simple instance already makes for alott of model related programming in the controller,
    // which should be done by a repository and the controller just limited to simple calls with re-directions.
    // For this to work LanuageId and Description were added to the database entity, but notmapped there. Also
    // needed to add package for mvc to be able to add remote attribute to database entity. Either alone is 
    // already a good enough reason to work with a seperate View entity. 
    // 
    // Some details worked out in this controller
    //  o The CountryId property is editable by the user and is forced uppercase (with set property and style in view)
    //  o The CountryId is remotely checked if it exists before user can save (with Remote attribute and ValidateCountryId action)
    //  o The LanguageId passed to and from views using a hidden form field
    //  o All the foreign language descriptions are shown in the details view
    //
    // Some details not worked out in this controller
    //  o Multilingual error messages
    //  o Best practices involving using nameof instead of property named strings
    //  o Best practices involving some try/catch blocks
    //  o Best practices involving edit function to work properly for none editable fields

    public class CountriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CountriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            // Adding the include clause for the foreign descriptions helps with reducing the number
            // of SQL calls to one instead of one per country. It could require more resources tho !
            // Debugging the actual SQL commands by using the SQL profiler form the Microsoft SQL management studio
            return View(await _context.Countries.Include(x => x.Descriptions).ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            // This is needed to set the LanguageId, but not the way to go
            var country = new Country { };
            string s = country.Description;

            return View(country);
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Because Description and LanguageId properties in Country are notmapped, a seperate parameter is needed to map to
        // Naming the possible binding properties secures for overposting, that is to call an URL directly with property names and values not supposed to be editable
        public async Task<IActionResult> Create([Bind("CountryId","Description","LanguageId")] Country country, string description, [BindRequired] string languageId)
        {
            if (country.CountryId != null)
            {
                // By convention countrycodes should be uppercase, done in the property setter, so redundant here
                country.CountryId = country.CountryId.ToUpper();

                // TODO: Possibly add a custom attribute in the model for this and add client-side checking/conversion
                //       Added style= "text-transform:uppercase" to html in view presents the information in uppercase
                //       for the user however the data is still in the case it was entered !!!
            }

            if (String.IsNullOrEmpty(country.CountryId))
            {
                // Redundant model check due to [Required] attribute on property
                ModelState.AddModelError(nameof(country.CountryId), "Country cannot be empty");
            }
            else if (country.CountryId.Length > 2)
            {
                // Redundant model check, already done because of [StringLength(2)] attribute on property, because
                // the model check is added to the scripting of the rendered page as early check to minimize communication with server        
                ModelState.AddModelError(nameof(country.CountryId), $"Country {country.CountryId} exceeds length of 2 chracters");
            }
            else if (CountryExists(country.CountryId))
            {
                ModelState.AddModelError(nameof(country.CountryId), $"Country {country.CountryId} already exists");
            }

            if (description != null && description.Length > 80)
            {
                // Another redundant model check due to [StringLength(80)] attribute on property
                ModelState.AddModelError(nameof(country.Description), "Country description exceeds length of 80 characters");
            }

            if (description != null)
            {
                country.Descriptions.Add(
                new CountryDescription
                {
                    CountryId = country.CountryId,
                    LanguageId = languageId,
                    Description = description
                });
            }

            // Additional validation done when properties are set after model bound and validated all parameters initialy
            TryValidateModel(country);

            // Check to make sure the binding processes passed all the model validation attributes
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(country);                  
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "A problem occured writing data to SQL, try again or contact administrator if error keeps returning");
                    return View(country);
                }
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryId","Description","LanguageId")] Country country, string description, [BindRequired] string languageId)
        {
            if (id != country.CountryId)
            {
                return NotFound();
            }

            // This function updates specific fields of a valid country object from the ControlerContext instead of 
            // reinitializing the whole country object as binding would do. No problem with country now as it only
            // has one real field, eg. CountryId.
            //var countryToUpdate = await _context.Countries.SingleOrDefaultAsync(x => x.CountryId == id);
            //if (await TryUpdateModelAsync<Country>( (countryToUpdate, "",
            //                                        x => x.CountryId, x => x.Description, x => x.LanguageId))

            if (ModelState.IsValid)
            {
                var countryDescription = await _context.CountryDescriptions.FirstOrDefaultAsync(x => (x.CountryId == country.CountryId && x.LanguageId == languageId));
                if (countryDescription == null)
                {
                    if (!String.IsNullOrEmpty(description))
                    {
                        countryDescription = new CountryDescription
                        {
                            CountryId = country.CountryId,
                            LanguageId = languageId,
                            Description = description
                        };
                        _context.Add<CountryDescription>(countryDescription);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(description))
                    {                        
                        countryDescription.Description = description;
                        _context.Update<CountryDescription>(countryDescription);
                    }
                    else
                    {
                        _context.Remove<CountryDescription>(countryDescription);
                    }
                }

                try
                {
                    _context.Update(country); 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Remove only works if all foreign key constraints are deleted as well, so they need to be tracked
            // by the context, so they must be loaded to be trackable. Just deleting country without having the 
            // country descriptions loaded in the context will result in an SQL exception

            //var country = await _context.Countries.FindAsync(id);
            var country = await _context.Countries.Include(x => x.Descriptions).FirstOrDefaultAsync(x => x.CountryId == id);
            
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(string id)
        {
            // Default SQL string comparison does this case-insensative (altho can be changed in SQL setup), so 
            // countries DE, de, De and dE would all mean the same, however they can be stored in 4 different 
            // datarecords under 4 different keys. Adding case sensative searches is not wanted/needed here either 
            // since countryid's should be stored in uppercase only, enforced here in the property setter.
       
            return _context.Countries.Any(e => e.CountryId == id.ToUpper());
        }


        // Method used by the [Remote(action: "ValidateCountryId", controller: "Countries")] attribute on CountryId
        // This method is called remotely by client-side scripting. Not adding much in this example, but can be used
        // to give user direct information when entering some data that needs to be checked against a database.
        // Consider however if the direct information weighs up against more additional server calls, since this check
        // can/should also be present to the Create method in the controler because of concurrency issues
        [AcceptVerbs("Get", "Post")]
        public IActionResult ValidateCountryId(string countryId)
        {
            if (CountryExists(countryId.ToUpper()))
            {
                return Json($"Country {countryId.ToUpper()} is already in use.");
            }

            return Json(true);
        }
    }
}
