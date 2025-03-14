namespace CodeGenApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Parse command line arguments
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: EntityGeneratorConsole <filePath> <tableName> <outputDirectory>");
                    return;
                }

                string filePath = args[0];
                string tableName = args[1];
                string outputDirectory = args[2];

                var generator = new EntityGenerator();
                await generator.GenerateFromFile(filePath, tableName, outputDirectory);

                Console.WriteLine("Generation completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
