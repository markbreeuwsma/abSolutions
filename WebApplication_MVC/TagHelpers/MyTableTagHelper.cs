using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApplication_MVC.TagHelpers
{
    // Set up a taghelper for the HTML/Razor markup '<my-table items=<IEnumerable<T> otherattributes>'
    // To use this taghelper you must add it to the _ViewImports.cshtml file: @addTagHelper *, WebApplication_MVC (=assembly name, not project)
    // The class suffix ...TagHelper is optional but considered good practice
    // TagHelper conventions will translate Pascal cased class or properties to lower-kebab-case to use for 
    // lookup if not specifically named by use of attributes. Here attributes are added to show posibilities, 
    // but some are redundant, since the naming and default conventions of the TagHelper class achieve the same.
    //
    // Also check https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring?view=aspnetcore-2.2
    //
    // This helper as is can be used to quickly view any enumeration in HTML, but to make it a useful component, the
    // user should be able to configure the columns and this helper would set up a table based on that configuration.
    // Configuration like what columns to include, in which order, with what column name and format.

    [HtmlTargetElement("my-table", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = ItemsAttributeName)]
    public class MyTableTagHelper : TagHelper
    {
        private const string ItemsAttributeName = "items"; 

        [HtmlAttributeName(ItemsAttributeName)]  // identifies this property as to be used for storing the 'items' attribute value from the markup
        public IEnumerable<object> Items { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // context contains related to the HTML element, its attributes and all its children. Although not
            // done here you could use the childeren to check if the th row is already set and then use that instead 
            // of generating one from the property names

            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            var table = new TagBuilder("table");                // start a new HTML table
                                                                
            // This is what the book said to do, but this does not work properly AND adds them
            // to the table component, which only the innerpart of the table component is used
            // to change the output. Probably better to directly add to the output param.
            //table.GenerateId(context.UniqueId, "id");           // add an unique id attribute
            //var attributes = context.AllAttributes              // retrieve the list of all other attributes than 'items'
            //                        .Where(a => a.Name != ItemsAttributeName)
            //                        .ToDictionary(a => a.Name);
            //table.MergeAttributes(attributes);

            output.Attributes.Add("id", context.UniqueId);

            var properties = CreateTableHeader(table);

            var tbody = new TagBuilder("tbody");
            foreach (var item in Items)
            {
                var tr = new TagBuilder("tr");
                foreach (var prop in properties)
                {
                    var td = new TagBuilder("td"); 
                    td.InnerHtml.Append(prop.GetValue(item).ToString()); // encodes characters to correct HTML 
                    tr.InnerHtml.AppendHtml(td);
                }
                tbody.InnerHtml.AppendHtml(tr);
            }
            table.InnerHtml.AppendHtml(tbody);

            output.TagName = "table";
            output.Content.AppendHtml(table.InnerHtml);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // current output doesn't do anything asynchounous, like output.GetChildrenAsync, so
            // works with a call to the synchronous method. For more complex methods it could pay
            // to work out the asynchroness part.

            Process(context, output);
        }

        private List<PropertyInfo> CreateTableHeader(TagBuilder table)
        {            
            // Items.First does not work when list is empty, so better to retrieve it from the type specs 
            //var item = Items.First();
            //List<PropertyInfo> properties = item.GetType().GetProperties().Where(p => p.Name != "RowVersion").ToList();

            List<PropertyInfo> properties = Items.GetType()                            // IEnumerable<T>
                                                 .GenericTypeArguments[0]              // Type of T
                                                 .GetProperties()                      // All properties
                                                 .Where(p => p.Name != "RowVersion")   // Hardcoded filter
                                                 .ToList();                            // Convert array to list

            var thead = new TagBuilder("thead");
            var tr = new TagBuilder("tr");
            foreach (var prop in properties)
            {
                var th = new TagBuilder("th");
                th.InnerHtml.Append(prop.Name);
                tr.InnerHtml.AppendHtml(th);
            }
            thead.InnerHtml.AppendHtml(tr);
            table.InnerHtml.AppendHtml(thead);
            return properties;
        }
    }
}
