using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using CurrencyRate.Application.DataAccess.Query;
using CurrencyRate.Domain.Constants;
using CurrencyRate.Domain.Entities;

namespace CurrencyExchange.Infrastructure.DataAccess.Query
{
    public class CurrencyRateQuery : ICurrencyRateQuery
    {
        public async Task<List<RateDto>> Execute(string address, string selector)
        {
            var document = await GetSourceHtmlDocumentAsync(address);
            var outputString = GetOutputStringAfterCssSelector(document, selector);
            var arrayString = ConvertOutputStringToArrayHtmlString(outputString);
            var rateDtoList = await CreateCurrencyListAsync(arrayString, address);
            return rateDtoList;
        }

        private async Task<IDocument> GetSourceHtmlDocumentAsync(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(address);
            return document;
        }

        private string GetOutputStringAfterCssSelector(IDocument document, string selector)
        {
            var elements = document.GetElementsByClassName(selector);
            var InnerHtmlPropertyElements = elements.Select(m => m.InnerHtml);

            string outputString = null;
            foreach (var innerHtmlPropertyElement in InnerHtmlPropertyElements)
            {
                outputString += innerHtmlPropertyElement;
            }

            return outputString;
        }

        private string[] ConvertOutputStringToArrayHtmlString(string outputString)
        {
            var massivStrok = outputString.Split("</tr>");
            Array.Resize(ref massivStrok, massivStrok.Length - 1);
            return massivStrok;
        }

        private async Task<List<RateDto>> CreateCurrencyListAsync(string[] massivStrok, string address)
        {
            
            List<RateDto> rateDtoList = new List<RateDto>();
            var currencyCode = string.Empty;
            if (address == Urls.AddressUsdPage)
            {
                currencyCode = "USD";
            }
            if (address == Urls.AddressEurPage)
            {
                currencyCode = "EUR";
            }
            if (address == Urls.AddressPlnPage)
            {
                currencyCode = "PLN";
            }
            foreach (var VARIABLE in massivStrok)
            {
                var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(VARIABLE));
                var valueTagA = document.QuerySelector("a");
                var listValueTagSpan = document.QuerySelectorAll("span");
                var rateDto = new RateDto();

                rateDto.BankName = valueTagA.Text();
                rateDto.BuyRate = listValueTagSpan.First().TextContent;
                rateDto.SellRate = listValueTagSpan.Last().TextContent;     
                rateDto.CurrencyCode = currencyCode;
                rateDtoList.Add(rateDto);
            }

            return rateDtoList;
        }
    }
}
