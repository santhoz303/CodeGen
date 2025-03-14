using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CodeGenApp;

namespace EntityGeneratorConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Entity and DTO Generator Console");
            Console.WriteLine("================================");

            try
            {
                // If command line arguments are provided, use them
                if (args.Length >= 3)
                {
                    string filePath = args[0];
                    string tableName = args[1];
                    string outputDirectory = args[2];

                    await RunGenerator(filePath, tableName, outputDirectory);
                    return;
                }

                // Otherwise, run in interactive mode
                await RunInteractiveMode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            // Add this line to keep the console window open
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task RunInteractiveMode()
        {
            // Get file path
            Console.Write("Enter the path to your JSON or CSV file: ");
            string filePath = Console.ReadLine();

            // Validate file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            // Check if file is JSON or CSV
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".json" && extension != ".csv")
            {
                Console.WriteLine("File must be either JSON (.json) or CSV (.csv)");
                return;
            }

            // Create sample file if user wants to see example
            if (filePath.ToLower() == "sample")
            {
                filePath = CreateSampleFile();
                Console.WriteLine($"Created sample file at: {filePath}");
            }

            // Get table name
            Console.Write("Enter the table name (used for class naming): ");
            string tableName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tableName))
            {
                Console.WriteLine("Table name cannot be empty");
                return;
            }

            // Get output directory
            Console.Write("Enter output directory (press Enter for current directory): ");
            string outputDirectory = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                outputDirectory = Directory.GetCurrentDirectory();
            }

            await RunGenerator(filePath, tableName, outputDirectory);
        }

        static async Task RunGenerator(string filePath, string tableName, string outputDirectory)
        {
            Console.WriteLine("\nGenerating classes...");
            Console.WriteLine($"File: {filePath}");
            Console.WriteLine($"Table: {tableName}");
            Console.WriteLine($"Output: {outputDirectory}");

            var generator = new EntityGenerator();
            await generator.GenerateFromFile(filePath, tableName, outputDirectory);

            Console.WriteLine("\nGeneration completed successfully!");
            Console.WriteLine($"Generated files in: {outputDirectory}");
        }

        static string CreateSampleFile()
        {
            // Create a sample JSON file
            string sampleDirectory = Path.Combine(Directory.GetCurrentDirectory(), "samples");
            Directory.CreateDirectory(sampleDirectory);

            string sampleFilePath = Path.Combine(sampleDirectory, "employees.json");

            var employees = new[]
            {
                new {
                    id = 1,
                    first_name = "John",
                    last_name = "Doe",
                    email = "john.doe@example.com",
                    hire_date = DateTime.Now.AddYears(-2).ToString("yyyy-MM-dd"),
                    salary = 65000.00,
                    department_id = 101,
                    is_active = true
                },
                new {
                    id = 2,
                    first_name = "Jane",
                    last_name = "Smith",
                    email = "jane.smith@example.com",
                    hire_date = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"),
                    salary = 72000.00,
                    department_id = 102,
                    is_active = true
                }
            };

            string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(sampleFilePath, json);

            return sampleFilePath;
        }
    }
}