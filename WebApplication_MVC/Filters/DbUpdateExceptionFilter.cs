using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;

namespace WebApplication_MVC.Filters
{
    // This allows for an attribute to be defined on an action, controller or all controles and gives
    // a means to handle uncatched exceptions in a generic way. This one handles an EF exception where
    // SQL returns a problem with a key not being unique. This is added to an action as attribute like
    // [DbUpdateExceptionFilter].
    // NB: Not sure how generic messages would work, since you want to provide the best and detailed
    // information to a consumer of an API and not a generic response like this, but maybe I will see
    // the light in future endevours.

    public class DbUpdateExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        // save some info through dependancy injection to possibly be used later
        public DbUpdateExceptionFilterAttribute(IHostingEnvironment hostingEnvironment,
                                                IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            // do nothing in development, let normal debuggin options run it's course
            if (!_hostingEnvironment.IsDevelopment()) return;
            if (!(context.Exception is DbUpdateException)) return;

            var sqlException = context.Exception?.InnerException?.InnerException as SqlException;

            // this demod fields are yet again disappeared and do not compile anymore, so fuck this, 
            // but you get the idea, and also MSDN does not really promote these filters as good practice
            //if (sqlException?.Number == 2627)  // hardcoded SQL-server error code for unique key constraint error
            //    context.Response = new HttpResponseMessage(HttpStatusCode.Conflict);

            //context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            // context.ExceptionHandled = true; // stop the exception to bubble upwards
        }
    }

}