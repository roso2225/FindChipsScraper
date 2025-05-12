using ClosedXML.Excel;
using FindChipsScraper.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class FindChipsController : Controller
{
    private readonly FindChipsScraperService _scraperService;

    public FindChipsController()
    {
        _scraperService = new FindChipsScraperService();
    }

    // GET: FindChips
    public async Task<ActionResult> Index(string partNumber = "2N222")
    {
        List<Offer> offers = new List<Offer>();
        try
        {
            offers = await _scraperService.ScrapeOffersAsync(partNumber);
            ViewBag.PartNumber = partNumber;
        }
        catch (Exception ex)
        {
            // Log the error to the errorlog.txt file
            ErrorLogger.LogError(ex, $"Error scraping offers for part number {partNumber}");
            ViewBag.ErrorMessage = "There was an error retrieving offers. Please try again later.";
        }
        return View(offers);
    }

    // Method to download the data as an Excel file
    public async Task<IActionResult> DownloadExcel(string partNumber = "2N222")
    {
        try
        {
            var stream = await GenerateExcelFile(partNumber);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FindChips_Offers.xlsx");
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex, "Excel generation failed");
            return BadRequest("Failed to generate Excel.");
        }
    }


    private async Task<MemoryStream> GenerateExcelFile(string partNumber)
    {
        var offers = await _scraperService.ScrapeOffersAsync(partNumber);
        var memoryStream = new MemoryStream();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Offers");

            // Headers
            worksheet.Cell(1, 1).Value = "Distributor";
            worksheet.Cell(1, 2).Value = "Seller";
            worksheet.Cell(1, 3).Value = "MOQ";
            worksheet.Cell(1, 4).Value = "SPQ"; 
            worksheet.Cell(1, 5).Value = "Unit Price";
            worksheet.Cell(1, 6).Value = "Currency";

            // Rows
            for (int i = 0; i < offers.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = offers[i].Distributor;
                worksheet.Cell(i + 2, 2).Value = offers[i].Seller;
                worksheet.Cell(i + 2, 3).Value = offers[i].MOQ;
                worksheet.Cell(i + 2, 4).Value = offers[i].SPQ;
                worksheet.Cell(i + 2, 5).Value = offers[i].UnitPrice;
                worksheet.Cell(i + 2, 6).Value = offers[i].Currency;
            }

            // Adjust column widths to fit content
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(memoryStream);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }


}
