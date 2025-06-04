using System.Text;
using System.Web;
using AngleSharp.Html.Parser;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;

namespace AutoParts_ShopAndForum.DataGatherer
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
            using var context = new ApplicationDbContext();
            
            var townOptions = GeTownOptions();
            var townsOffices = GetTownsOffices(townOptions);

            foreach (var office in townsOffices)
            {
                var town = AddOrUpdateTown(office.Value, context);
            }
        }

        private static Town AddOrUpdateTown(OfficeModel office, ApplicationDbContext context)
        {
            const string cityKeWord = "гр.";
            const string villageKeyWord = "с.";
            
            var townName = office.FullAddress;
            var isCity = townName.Contains(cityKeWord);
            var code = townName.Substring(townName.IndexOf('[') + 1, 4);
            var clearTownName = townName
                .Replace(cityKeWord, "")
                .Replace(villageKeyWord, "");
            var name = clearTownName[..(clearTownName.IndexOf('[')-1)].Trim();

            var town = context.Towns
                .FirstOrDefault(x => x.PostCode == code);

            if (town == null)
            {
                town = context.Towns.Add(new Town
                {
                    PostCode = code,
                    Name = name,
                    IsCity = isCity,
                }).Entity;
            }

            return town;
        }

        private static Dictionary<string, OfficeModel> GetTownsOffices(ICollection<TownOption> townOptions)
        {
            // speedy-offices-automats?city=182&formToken=-false

            var result = new Dictionary<string, OfficeModel>();

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
                    .QuerySelectorAll(".offices-list .boxes .box");

                foreach (var office in offices)
                {
                    var officeRaw = office.InnerHtml;
                    var parsedOfficeDocument = _parser.ParseDocument(officeRaw);
                    
                    var officeTitle = parsedOfficeDocument.QuerySelector(".office-name")?.InnerHtml;
                    var officeFullAddress = parsedOfficeDocument.QuerySelector("br")?.NextSibling?.TextContent ?? "";
                    
                    if (string.IsNullOrEmpty(officeTitle))
                        continue;
                    
                    result[option.FullName] = new OfficeModel
                    {
                        Title = officeTitle,
                        FullAddress = officeFullAddress,
                    };

                    break;
                }

                break;
            }

            return result;
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
