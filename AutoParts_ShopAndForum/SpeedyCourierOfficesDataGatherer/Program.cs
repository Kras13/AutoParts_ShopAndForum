using System.Text;
using System.Web;
using AngleSharp.Html.Parser;

namespace SpeedyCourierOfficesDataGatherer
{
    internal class Program
    {
        private static string url = "https://www.speedy.bg/bg/speedy-offices-automats";
        private static string urlAutomats = "https://www.speedy.bg/bg/speedy-offices-automats?formToken=-false";
        private static readonly HttpClient _client;
        private static readonly HtmlParser _parser;
        private static readonly Encoding _encoding = Encoding.UTF8;

        static Program()
        {
            _client = new HttpClient();
            _parser = new HtmlParser();
        }

        static void Main(string[] args)
        {
            var townOptions = GeTownOptions();
            var townsOffices = GetTownsOffices(townOptions);
            
            ;
        }

        private static Dictionary<string, string> GetTownsOffices(ICollection<TownOption> townOptions)
        {
            // speedy-offices-automats?city=182&formToken=-false

            var result = new Dictionary<string, string>();

            foreach (var option in townOptions)
            {
                var builder = new UriBuilder(urlAutomats);

                builder.Port = -1;

                var query = HttpUtility.ParseQueryString(builder.Query);

                query["city"] = option.OptionValue;

                builder.Query = query.ToString();
                
                var response = _client
                    .GetAsync(builder.ToString())
                    .GetAwaiter()
                    .GetResult();

                var rawHtml = GetHtml(response);
                var wholeDocument = _parser.ParseDocument(rawHtml);
                var offices = wholeDocument
                    .QuerySelectorAll(".offices-list");


            }

            return null;
        }

        private static ICollection<TownOption> GeTownOptions()
        {
            var result = new List<TownOption>();
            var response = _client
                .GetAsync(url)
                .GetAwaiter()
                .GetResult();

            var rawHtml = GetHtml(response);
            var wholeDocument = _parser.ParseDocument(rawHtml);

            var citiesSelect = wholeDocument
                .QuerySelectorAll("select")
                .FirstOrDefault(x => x.Id == "offices_list_choose_city_-false");

            if (citiesSelect == null)
                throw new InvalidOperationException("Office options were not found on this page.");


            var parsedOptions = _parser.ParseDocument(citiesSelect.InnerHtml);

            foreach (var option in parsedOptions.QuerySelectorAll("option"))
            {
                var valueAttr = option.Attributes["value"];

                if (valueAttr == null || valueAttr.Value == "all" || string.IsNullOrWhiteSpace(valueAttr.Value.Trim()))
                    continue;

                var defAttr = option.GetAttribute("data-select2-id");

                if (defAttr != null)
                    continue;

                var officeOption = new TownOption()
                {
                    FullName = option.InnerHtml,
                    OptionValue = valueAttr.Value,
                };

                result.Add(officeOption);
            }

            return result;
        }

        private static string GetHtml(HttpResponseMessage response)
        {
            var byteContent = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            return _encoding.GetString(byteContent);
        }
    }
}
