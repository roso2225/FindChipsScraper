
FindChips Web Scraping and Excel Export

Project Overview:
This project scrapes part numbers from the FindChips website(https://www.findchips.com) using Playwright (or Selenium WebDriver for Firefox). It retrieves information about distributors, sellers, prices, and more, then exports the data to an Excel file using ClosedXML.

Prerequisites:

1. .NET SDK:
    Make sure you have .NET SDK 6.0 or later installed on your machine. You can download it from here:
   - Download .NET SDK: https://dotnet.microsoft.com/download/dotnet

2. Playwright:
   -Install Playwright for browser automation. You can install it via the .NET CLI:
  
   -dotnet add package Microsoft.Playwright
   
   -After installation, run the following command to install the necessary browsers:
   
   -playwright install
   

3. Selenium WebDriver (For Firefox):
     -Selenium WebDriver for Firefox, install the following pkgs in Nuget Manager:
   
   -dotnet add package Selenium.WebDriver
   -dotnet add package Selenium.WebDriver.ChromeDriver
   -dotnet add package Selenium.WebDriver.GeckoDriver
   

4. HTMLAgilityPack:
   Install HTMLAgilityPack for HTML parsing:
   
   dotnet add package HtmlAgilityPack
   

5. ClosedXML:
   Install ClosedXML to handle Excel export:
  
   dotnet add package ClosedXML

6. Need Firefox browser to run this project.

Steps to Execute the Project:
1. Open the Project:
   Open the project using Visual Studio or Visual Studio Code.

2. Build the Project:
   Build the project using F5 or run the following command in the terminal:
    dotnet build
  
3. Run the Application:
   Execute the program by running:
   dotnet run
   The program will scrape the data for the default part number 2N222 from the FindChips website and then generate an Excel file with the scraped information.

4. Download the Excel File:
   The scraped data will be exported into an Excel file named FindChips_Offers.xlsx.

Error Logging:

Any errors encountered during the scraping process or Excel file generation will be logged in the errorlog.txt file in the root directory of the project.


