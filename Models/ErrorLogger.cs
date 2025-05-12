using System;
using System.IO;

public static class ErrorLogger
{
    private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "errorlog.txt");

    public static void LogError(Exception ex, string additionalMessage = "")
    {
        try
        {
            // Create or append to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                // Write the timestamp and error message
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {additionalMessage}");
                writer.WriteLine($"Exception Message: {ex.Message}");
                writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                writer.WriteLine();
            }
        }
        catch (Exception logEx)
        {
            // If logging fails, print to the console
            Console.WriteLine($"Error logging failed: {logEx.Message}");
        }
    }
}
