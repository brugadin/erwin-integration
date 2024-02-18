using ErwinDataExtractorLib;

class Program
{
    static void Main(string[] args)
    {
        string erwinFilePath = @"C:\Users\gabri\Downloads\test.erwin";
        
        try
        {
            var entitiesString = ErwinDataExtractor.ExtractErwinDataToJson(erwinFilePath);
            Console.WriteLine(entitiesString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed due to: {ex.Message}");
        }
    }
}
