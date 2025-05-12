using FindChipsScraper.Models;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FindChipsScraperService
{
    private const int MaxRetries = 3;  // Max retry attempts for network-related failures
    private const int TimeoutInMilliseconds = 30000; // 30 seconds timeout for operations
    private const string LogFilePath = "scraping_log.txt";  // Log file path

    public async Task<List<Offer>> ScrapeOffersAsync(string partNumber)
    {
        var offers = new List<Offer>();
        try
        {
            LogMessage("Scraping started.");
            var playwright = await Playwright.CreateAsync();
            LogMessage("Playwright created successfully.");
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                SlowMo = 0
            });
            LogMessage("Browser launched successfully.");
            var page = await browser.NewPageAsync();
            LogMessage("New page created.");
            var url = $"https://www.findchips.com/search/{partNumber}";
            LogMessage($"Navigating to: {url}");

            var retries = 0;
            bool pageLoaded = false;
            while (retries < MaxRetries && !pageLoaded)
            {
                try
                {
                    await page.GotoAsync(url, new PageGotoOptions { Timeout = TimeoutInMilliseconds });
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = TimeoutInMilliseconds });
                    pageLoaded = true;
                    LogMessage("Page loaded successfully.");
                }
                catch (TimeoutException)
                {
                    retries++;
                    LogMessage($"Retry {retries}/{MaxRetries}: Timeout occurred while navigating to the page.");
                }
            }

            if (!pageLoaded)
            {
                LogMessage("Failed to load the page after multiple attempts.");
                return offers;
            }

            var containers = await page.QuerySelectorAllAsync("div.distributor-results");
            LogMessage($"Found {containers.Count} distributor containers. Inspecting up to 5...");

            foreach (var container in containers.Take(5)) // only 5 distributors
            {
                var parsedOffers = await ParseDistributorPlaywright(container);
                offers.AddRange(parsedOffers.Take(5)); // only 5 offers per distributor
            }


            await browser.CloseAsync();
            LogMessage("Browser closed.");
        }
        catch (Exception ex)
        {
            LogMessage($"Scraping failed: {ex.Message}");
        }

        return offers;
    }

    private async Task<List<Offer>> ParseDistributorPlaywright(IElementHandle container)
    {
        var offers = new List<Offer>();

        try
        {
            var distributorName = await container.GetAttributeAsync("data-distributor_name") ?? "Unknown Distributor";
            var rows = await container.QuerySelectorAllAsync("table tbody tr");

            foreach (var row in rows)
            {
                var offer = new Offer
                {
                    Distributor = distributorName,
                    Seller = await GetElementTextAsync(row, "td:nth-child(2)"),
                    MOQ = await GetElementTextAsync(row, "td:nth-child(4)"),
                    SPQ = await GetElementTextAsync(row, "td:nth-child(6)"), 
                    UnitPrice = await GetElementTextAsync(row, "td:nth-child(5)"), 
                    Currency = await GetCurrencyAsync(row), 
                  //  OfferUrl = "N/A", // You can remove or keep this if needed
                    Timestamp = DateTime.Now
                };

                offers.Add(offer);
            }

            LogMessage($"{offers.Count} offers parsed for {distributorName}.");
        }
        catch (Exception ex)
        {
            LogMessage($"Error parsing distributor offers: {ex.Message}");
        }

        return offers;
    }



    private async Task<string> GetCurrencyAsync(IElementHandle row)
    {
        try
        {
            var element = await row.QuerySelectorAsync("td:nth-child(5)");
            if (element != null)
            {
                var text = await element.InnerTextAsync();

                // Find all currency matches
                var matches = System.Text.RegularExpressions.Regex.Matches(text, @"[₹$€£¥]");

                // Return the first match if any found
                return matches.Count > 0 ? matches[0].Value : "N/A";
            }
            return "N/A";
        }
        catch (Exception ex)
        {
            LogMessage($"Error retrieving currency: {ex.Message}");
            return "N/A";
        }
    }



    private async Task<string> GetElementTextAsync(IElementHandle row, string selector)
    {
        try
        {
            var element = await row.QuerySelectorAsync(selector);
            var text = element != null ? await element.InnerTextAsync() : "N/A";

            // Extract the first numeric price (assuming the price has a currency symbol like "$" or "₹")
            var price = ExtractPrice(text);

            return price;
        }
        catch (Exception ex)
        {
            LogMessage($"Error retrieving element text for selector {selector}: {ex.Message}");
            return "N/A";
        }
    }

    private string ExtractPrice(string text)
    {
        var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+(\.\d{1,2})?");
        return match.Success ? match.Value : "N/A";
    }


    private async Task<string> GetOfferUrlAsync(IElementHandle row)
    {
        try
        {
            var link = await row.QuerySelectorAsync("a");
            return link != null ? "https://www.findchips.com" + (await link.GetAttributeAsync("href") ?? "") : "N/A";
        }
        catch (Exception ex)
        {
            LogMessage($"Error retrieving offer URL: {ex.Message}");
            return "N/A";
        }
    }

    private void LogMessage(string message)
    {
        try
        {
            using (var writer = new StreamWriter(LogFilePath, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing log to file: {ex.Message}");
        }
    }
}
