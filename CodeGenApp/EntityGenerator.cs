using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenApp
{
    public class EntityGenerator
    {
        public async Task GenerateFromFile(string filePath, string tableName, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty", nameof(filePath));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // Create output directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            string fileExtension = Path.GetExtension(filePath).ToLower();
            Dictionary<string, Type> columnTypes;

            switch (fileExtension)
            {
                case ".json":
                    columnTypes = await ReadJsonFile(filePath);
                    break;
                case ".csv":
                    columnTypes = await ReadCsvFile(filePath);
                    break;
                default:
                    throw new NotSupportedException($"File format not supported: {fileExtension}. Please provide a JSON or CSV file.");
            }

            // Generate Entity class
            string entityClassName = GetClassName(tableName, "Entity");
            string entityClassContent = GenerateClassContent(entityClassName, columnTypes, true);
            string entityFilePath = Path.Combine(outputDirectory, $"{entityClassName}.cs");
            await File.WriteAllTextAsync(entityFilePath, entityClassContent);

            // Generate DTO class
            string dtoClassName = GetClassName(tableName, "Dto");
            string dtoClassContent = GenerateClassContent(dtoClassName, columnTypes, false);
            string dtoFilePath = Path.Combine(outputDirectory, $"{dtoClassName}.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoClassContent);

            Console.WriteLine($"Generated {entityClassName}.cs and {dtoClassName}.cs in {outputDirectory}");
        }

        private async Task<Dictionary<string, Type>> ReadJsonFile(string filePath)
        {
            string jsonContent = await File.ReadAllTextAsync(filePath);

            using JsonDocument document = JsonDocument.Parse(jsonContent);
            JsonElement root = document.RootElement;

            // Check if the JSON is an array
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                // Get the first object in the array
                JsonElement firstItem = root[0];
                return InferTypes(firstItem);
            }
            // Check if the JSON is an object
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Try to find array properties
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array && property.Value.GetArrayLength() > 0)
                    {
                        JsonElement firstItem = property.Value[0];
                        return InferTypes(firstItem);
                    }
                }

                // If no arrays found, use the object itself
                return InferTypes(root);
            }

            throw new InvalidDataException("JSON file does not contain a valid data structure for table inference.");
        }

        private Dictionary<string, Type> InferTypes(JsonElement element)
        {
            var columnTypes = new Dictionary<string, Type>();

            foreach (JsonProperty property in element.EnumerateObject())
            {
                string propertyName = property.Name;
                Type propertyType = GetTypeFromJsonElement(property.Value);
                columnTypes.Add(propertyName, propertyType);
            }

            return columnTypes;
        }

        private Type GetTypeFromJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    DateTime dateTime;
                    if (DateTime.TryParse(element.GetString(), out dateTime))
                        return typeof(DateTime);
                    else
                        return typeof(string);
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out _))
                        return typeof(int);
                    if (element.TryGetInt64(out _))
                        return typeof(long);
                    if (element.TryGetDecimal(out _))
                        return typeof(decimal);
                    return typeof(double);
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return typeof(bool);
                case JsonValueKind.Null:
                    return typeof(object);
                case JsonValueKind.Array:
                    return typeof(List<object>);
                case JsonValueKind.Object:
                    return typeof(object);
                default:
                    return typeof(string);
            }
        }

        private async Task<Dictionary<string, Type>> ReadCsvFile(string filePath)
        {
            var columnTypes = new Dictionary<string, Type>();

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            });

            await csv.ReadAsync();
            csv.ReadHeader();
            string[] headers = csv.HeaderRecord;

            // Read the first row to infer types
            if (await csv.ReadAsync())
            {
                foreach (string header in headers)
                {
                    string value = csv.GetField(header);
                    Type inferredType = InferType(value);
                    columnTypes.Add(header, inferredType);
                }
            }

            return columnTypes;
        }

        private Type InferType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return typeof(string);

            // Try to parse as various types
            if (bool.TryParse(value, out _))
                return typeof(bool);

            if (int.TryParse(value, out _))
                return typeof(int);

            if (long.TryParse(value, out _))
                return typeof(long);

            if (decimal.TryParse(value, out _))
                return typeof(decimal);

            if (double.TryParse(value, out _))
                return typeof(double);

            if (DateTime.TryParse(value, out _))
                return typeof(DateTime);

            return typeof(string);
        }

        private string GetClassName(string tableName, string suffix)
        {
            // Remove invalid characters and convert to PascalCase
            string className = Regex.Replace(tableName, @"[^a-zA-Z0-9_]", "");

            // Ensure it starts with a letter
            if (Regex.IsMatch(className, @"^\d"))
                className = "T" + className;

            // Convert to PascalCase if it's not already
            if (className.Length > 0)
            {
                className = char.ToUpper(className[0]) + (className.Length > 1 ? className.Substring(1) : "");
            }

            return className + suffix;
        }

        private string GenerateClassContent(string className, Dictionary<string, Type> columnTypes, bool isEntity)
        {
            var sb = new StringBuilder();

            // Add using statements
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();

            // Add namespace and class declaration
            sb.AppendLine("namespace FileToEntityGenerator.Models");
            sb.AppendLine("{");

            if (isEntity)
                sb.AppendLine($"    public class {className}");
            else
                sb.AppendLine($"    public class {className}");

            sb.AppendLine("    {");

            // Add properties
            foreach (var column in columnTypes)
            {
                string propertyName = ToPascalCase(column.Key);
                string typeName = GetCSharpTypeName(column.Value);

                if (isEntity)
                {
                    // Add Data Annotations for Entity
                    if (propertyName.ToLower() == "id" || propertyName.ToLower().EndsWith("id"))
                    {
                        sb.AppendLine("        [Key]");
                        if (propertyName.ToLower() == "id")
                        {
                            sb.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                        }
                    }
                    sb.AppendLine($"        [Column(\"{column.Key}\")]");
                }

                sb.AppendLine($"        public {typeName} {propertyName} {{ get; set; }}");
                sb.AppendLine();
            }

            // Close class and namespace
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Replace non-alphanumeric characters with spaces
            string sanitized = Regex.Replace(input, @"[^a-zA-Z0-9]", " ");

            // Split by spaces
            string[] words = sanitized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Convert to PascalCase
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }

            return string.Join("", words);
        }

        private string GetCSharpTypeName(Type type)
        {
            if (type == typeof(string))
                return "string";
            if (type == typeof(int))
                return "int";
            if (type == typeof(long))
                return "long";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(DateTime))
                return "DateTime";
            if (type == typeof(List<object>))
                return "List<object>";
            if (type == typeof(object))
                return "object";

            return "string"; // Default to string for unknown types
        }
    }
}
