using DatabaseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication_MVC.Controllers
{
    // TODO: This controller, much like the MVC controller for countries, would benefit from using a repository.
    // Also the various actions should be updated to work more like the FieldsOfInterestControler.
    // For best seperation of concern, use different models to work with the 'same' entity to seperate every
    // responsibility to the correct model (database, view, input, output can/should all be different models)

    // When using Postman to mock up requests, check setting for SSL when request doesn't seem to work   
    // When testing with Postman, be sure to send a JSON body AND put body format in 'application/json' else 
    // it will not be excepted and result in a bad request response.

    // set the response content of this specific controller to return JSON formatted data. This is also the default
    // in ASP.NET core but can be overruled in the startup class for all controllers, e.g. for XML formatten data.
    [Produces("application/json")]
    // to set the response content to XML formatted data, you need to install the Microsoft.aspnetcore.mvc.formatters.xml 
    // NuGet package and add it to the services in the configuration class
    //[Produces("application/xml")]

    // add authorization requirement to access any action in this controller. Being not authorized will result
    // in a 401 response. Attribute can also be used on individual actions, but this is more common and then
    // possibly whitelist open API actions by setting the [AllowAnonymous] attribute for a specific action.
    [Authorize]

    [Route("api/countries")]   // alternative route
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesApiController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CountriesApiController> _logger;


        public CountriesApiController(DatabaseContext context, ILogger<CountriesApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CountriesApi
        [HttpGet]
        // a GET call can only work with URI query parameters to filter. When wanting very complex filters, it is
        // possible to switch to a POST call where a body can be send with all the filter parameters. Those will
        // be set up as a new filter class with default values, so only set filters will be acted upon.
        // A simple annotations which sets a response header to let the CLIENT know it can use caching for this
        // data and for how long it can use the cached data before sending another new request to the server.
        // Selecting the correct duration is obviously a factor, but even a few seconds can help immensly for a 
        // server at no real cost to the user experience. This requires a UseReponseCaching service to be set at 
        // startup. Another methode uses a UseMemoryCache service which caches in memory at the SERVER and injects
        // that information into a controller or repository where wanted so methods can use that, if wanted. The 
        // contents of the cache is set and get by methods, altho no automation, so a bit more work involved.
        [ResponseCache(Duration = 60)]
        public IEnumerable<Country> GetCountries()
        {
            var list = _context.Countries.Include(x => x.Descriptions).AsNoTracking();

            // example of how to set the statuscode by hand, altho OK would already be the default
            Request.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            // example of how to create and assign a new header attribute to the response, so the caller can use that
            Request.HttpContext.Response.Headers.Add("X-Number-Of-Countries", list.Count().ToString());

            // returning countries with nested descriptions did not work properly when the description had a referal
            // back to the country table. With data annotations JsonIgnore or not including them as DataMember this
            // problem was resolved. Best control of what is exported can be done using the DataContract and DataMember
            // annotations (possibly with parameters or IgnoreDataMember) in the oject model.
            return list;
        }

        // GET: api/CountriesApi/5
        [HttpGet("{id}")]  // the {id} makes that this method gets called if the URI contains an {id} placeholder                           

        // the [FromRoute] means the specific parameters should come from the URI. Other options are [FromQuery]
        // which is the string of parameters after ? in an URI, [FromBody] used for complex data type mapping and 
        // only allowed once per method. Also [FromForm], [FromHeader] or [FromServices] can be applied.
        // Furthermore [BindRequired] or [BindNever] can be applies to indicate mandatory (never) binding parameters.

        // API calls communicate using the HTTP protocol so return an answer that possibly has multiple HTTP status
        // codes should return an IActionResult type (with or without <T>). The previous GetCountries function is 
        // the only method here that can return the default status.

        // within a secured controller, this action can now still be accessed without authorization by this attribute
        [AllowAnonymous]

        // These data annotation are not really needed anymore (i think) but allow for describing the possible responces.
        // Also the return type at 200 can be removed as overredundant and is derived from the passed parameter or could
        // be derived from the IActionResult<Country> if that was used in an synchronous method.
        [ProducesResponseType(200, Type = typeof(Country))]     // Ok
        [ProducesResponseType(400)]                             // BadRequest
        [ProducesResponseType(404)]                             // NotFound
        public async Task<IActionResult> GetCountry([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }
          
            return Ok(country); 
        }

        // PUT: api/CountriesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry([FromRoute] string id, [FromBody] Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // sets the response code, but also passes the modelstate error messages to the caller, linked to the property causing the problem
            }

            if (id != country.CountryId)
            {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
            // or return data after created, in this case the data another action / method returns
            // basically redirecting to another function, with an id parameter and ...
            // return CreatedByAction("GetCountry", new { id = customer.Customer }, customer });
        }
        
        //// POST: api/CountriesApi
        //[HttpPost]
        //public async Task<IActionResult> PostCountry([FromBody] Country country)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Countries.Add(country);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (CountryExists(country.CountryId))
        //        {
        //            return new StatusCodeResult(StatusCodes.Status409Conflict);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    // retrieve data from Action 'GetCountry' with param string 'new { id = .. }' set by object 'Country'
        //    return CreatedAtAction("GetCountry", new { id = country.CountryId }, country);
        //}

        //// DELETE: api/CountriesApi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCountry([FromRoute] string id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var country = await _context.Countries.FindAsync(id);
        //    if (country == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Countries.Remove(country);
        //    await _context.SaveChangesAsync();

        //    return Ok(country);
        //}

        private bool CountryExists(string id)
            {
                return _context.Countries.Any(e => e.CountryId == id);
            }
        }
}