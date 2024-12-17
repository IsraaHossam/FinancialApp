using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImperativeFinancialApp.Data
{
    public static class JsonHelper
    {
        public static JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}

