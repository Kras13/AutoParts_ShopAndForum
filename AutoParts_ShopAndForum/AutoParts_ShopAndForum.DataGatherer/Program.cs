using System.Text;
using System.Web;
using AngleSharp.Common;
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

            foreach (var offices in townsOffices)
            {
                foreach (var concreteOffice in offices.Value)
                {
                    var town = AddOrUpdateTown(concreteOffice, context); // stara zagora

                    if (town == null)
                        continue;

                    AddOrUpdateOffice(concreteOffice, context, town);
                }
            }
        }

        private static void AddOrUpdateOffice(
            OfficeModel office, ApplicationDbContext context, Town town)
        {
            var title = office.Title;
            var fullAddress = office.FullAddress;
            
            var station = context.CourierStations.FirstOrDefault(s => s.Title == title && s.TownId == town.Id);

            if (station != null)
            {
                station.FullAddress = fullAddress;
                
                context.SaveChanges();

                return;
            }

            context.CourierStations.Add(new CourierStation()
            {
                FullAddress = fullAddress,
                Title = title,
                Town = town,
                Firm = CourierStationFirm.Speedy,
                Type = title.Contains("автомат", StringComparison.CurrentCultureIgnoreCase)
                    ? CourierStationType.Machine 
                    : CourierStationType.Office,
            });
            
            context.SaveChanges();
        }

        private static Town? AddOrUpdateTown(OfficeModel office, ApplicationDbContext context)
        {
            const string cityKeWord = "гр.";
            const string villageKeyWord = "с.";
            
            var townName = office.FullAddress;
            var isCity = townName.Contains(cityKeWord);
            string code = null;

            if (townName.Contains('['))
              code = townName.Substring(townName.IndexOf('[') + 1, 4);
            
            var clearTownName = townName
                .Replace(cityKeWord, "")
                .Replace(villageKeyWord, "");

            string name;
            
            if (code == null) // oblasten grad (nqma post code)
                name = clearTownName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            else
                name = clearTownName[..(clearTownName.IndexOf('[')-1)].Trim();

            var town = context.Towns
                .FirstOrDefault(x => x.PostCode == code && x.Name == name);

            if (town == null)
            {
                town = context.Towns.Add(new Town
                {
                    PostCode = code,
                    Name = name,
                    IsCity = isCity,
                }).Entity;
                
                context.SaveChanges();
            }

            return town;
        }

        private static string TransLiterateCyrToLatin(string cyrText)
        {
            if (string.IsNullOrWhiteSpace(cyrText))
                return string.Empty;

            var map = new Dictionary<string, string>
            {
                {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
                {"е", "e"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"}, {"й", "y"},
                {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"}, {"о", "o"},
                {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"}, {"у", "u"},
                {"ф", "f"}, {"х", "h"}, {"ц", "ts"}, {"ч", "ch"}, {"ш", "sh"},
                {"щ", "sht"}, {"ъ", "a"}, {"ь", "y"}, {"ю", "yu"}, {"я", "ya"},

                {"А", "A"}, {"Б", "B"}, {"В", "V"}, {"Г", "G"}, {"Д", "D"},
                {"Е", "E"}, {"Ж", "Zh"}, {"З", "Z"}, {"И", "I"}, {"Й", "Y"},
                {"К", "K"}, {"Л", "L"}, {"М", "M"}, {"Н", "N"}, {"О", "O"},
                {"П", "P"}, {"Р", "R"}, {"С", "S"}, {"Т", "T"}, {"У", "U"},
                {"Ф", "F"}, {"Х", "H"}, {"Ц", "Ts"}, {"Ч", "Ch"}, {"Ш", "Sh"},
                {"Щ", "Sht"}, {"Ъ", "A"}, {"Ь", "Y"}, {"Ю", "Yu"}, {"Я", "Ya"}
            };

            var result = new StringBuilder();

            foreach (var ch in cyrText)
            {
                var s = ch.ToString();
                
                result.Append(map.GetValueOrDefault(s, s));
            }

            return result.ToString();
        }

        private static Dictionary<string, List<OfficeModel>> GetTownsOffices(ICollection<TownOption> townOptions)
        {
            // speedy-offices-automats?city=182&formToken=-false

            var result = new Dictionary<string, List<OfficeModel>>();

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
                    
                    if (!result.ContainsKey(option.FullName))
                        result[option.FullName] = new List<OfficeModel>();
                    
                    result[option.FullName].Add(new OfficeModel
                    {
                        Title = officeTitle,
                        FullAddress = officeFullAddress,
                    });
                }
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
