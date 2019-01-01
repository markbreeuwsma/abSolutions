using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApplication_MVC.TagHelpers
{
    // Used to replace <my-email mail-to="Support"></my-email> with <a mailto:"Support@...">Support</a>
    public class MyEmailTagHelper : TagHelper
    {
        private const string EmailDomain = "myemailtest.com";

        // passed via <my-email mail-to="..." /> because lower-kebab-case gets translated to pascal-case
        public string MailTo { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            if (output.TagMode != TagMode.StartTagAndEndTag) throw new NotImplementedException("Start tag must be paired with an end tag");

            output.TagName = "a";                               // Replaces <email> with <a> tag

            var address = MailTo + "@" + EmailDomain;
            output.Attributes.SetAttribute("href", "mailto:" + address);
            output.Content.SetContent(MailTo);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Process(context, output);            
        }    
    }
}
