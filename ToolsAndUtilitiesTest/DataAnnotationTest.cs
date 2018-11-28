using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using ToolsAndUtilities;

namespace ToolsAndUtilitiesTest
{
    [TestClass]
    public class DataAnnotationTest
    {
        [TestMethod]
        public void DataAnnotationsErrorMessageResourceNameTest()
        {
            /// Verifies that all properties that are decorated with validation data-annotations, refer to 
            /// an existing resource. This will make sure, that missing resources are not referenced.

            Assembly assembly = Assembly.Load(new AssemblyName("WebApplication_MVC"));  // be sure to add dependancy to project

            var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract);
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var attributes = property.GetCustomAttributes(true);
                    foreach (var item in attributes)
                    {
                        if (item is ValidationAttribute)
                        {
                            ValidationAttribute val = item as ValidationAttribute;
                            Assert.IsNotNull(val);
                            if (val.ErrorMessageResourceType != null)
                            {
                                Assert.AreNotEqual(String.Empty, val.ErrorMessageResourceName, String.Format(@"Validation Error Resource specified on property: {0}.{1} is empty!", type.ToString(), property.Name));
                                try
                                {
                                    ResourceManager rm = new ResourceManager(val.ErrorMessageResourceType);
                                    string resourceValue = rm.GetString(val.ErrorMessageResourceName);
                                    Assert.IsFalse(String.IsNullOrEmpty(resourceValue), String.Format(@"The value of the Validation Error Resource specified on property: {0}.{1} is empty!", type.ToString(), property.Name));
                                }
                                catch (MissingManifestResourceException)
                                {
                                    Assert.Fail(String.Format(@"Validation Error Resource specified on property: {0}.{1} could not be found!", type.ToString(), property.Name));
                                }
                            }
                        }
                    }
                }
            }            
        }
    }
}
