using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace TestFIService;

class Program
{
    [Obsolete]
    static void Main(string[] args)
    {
       

        try
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var appSettingsName = string.IsNullOrEmpty(environment) ? "appsettings.json" : $"appsettings.{environment}.json";
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appSettingsName);

            var configuration = builder.Build();
            Console.WriteLine("Testing FI Service");
            string filePath = $"{assemblyPath}{configuration["XmlFileName"]}";
            string AcctInqXml = File.ReadAllText(filePath);
            Console.WriteLine($"XML File Read \n {AcctInqXml}");

            FIService fIService = new FIService(configuration);

            var response = fIService.InvokeService(AcctInqXml);

            Console.WriteLine($"XML Response from FI \n {response}");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error " + ex.ToString());
        }

        Console.Write("Press Enter to Exit .....");
        Console.ReadLine();
    }
}

