using System.Reflection;
using System.Text.Json;

namespace Ib.Csharp.Api.Client.Tests
{
    public static class TestHelper
    {
        public static JsonSerializerOptions GetSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        
        public static void PrintNonDefaultProperties(object obj, string objectName = "Object")
        {
            var type = obj.GetType();
            Console.WriteLine($"{objectName} ({type.Name}):");

            var hasAny = false;
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var value = prop.GetValue(obj);
                    var defaultValue = prop.PropertyType.IsValueType 
                        ? Activator.CreateInstance(prop.PropertyType) 
                        : null;

                    var isDefault = Equals(value, defaultValue);

                    // Special case for strings: treat empty as default
                    if (value is string str && string.IsNullOrEmpty(str))
                        isDefault = true;

                    if (!isDefault)
                    {
                        Console.WriteLine($"  {prop.Name}: {value}");
                        hasAny = true;
                    }
                }
                catch { /* Skip properties that throw */ }
            }

            if (!hasAny)
                Console.WriteLine("  <no non-default properties>");
        }
    }
}
