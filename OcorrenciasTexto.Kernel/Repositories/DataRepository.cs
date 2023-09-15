using System.Text;

namespace OcorrenciasTexto.Kernel.Repositories;

public class DataRepository
{
    public static async Task<Dictionary<string, string>> GetPlainTextFiles(string pathFolder)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string[] filesHtml = Directory.GetFiles(pathFolder, "*.htm*");

        Dictionary<string, string> filesText = new Dictionary<string, string>();
        foreach (string file in filesHtml)
        {
            string fileName = Path.GetFileName(file);
            string fileText = await File.ReadAllTextAsync(file, Encoding.GetEncoding(1252));
            filesText.TryAdd(fileName, fileText);
            
            Console.WriteLine($"[INFO] Arquivo {fileName} carregado com sucesso.");
        }
        
        Console.WriteLine($"[INFO] Total de arquivos carregados: {filesText.Count}");

        return filesText;
    }

    public static async Task CreateFilePlainText(string path, string filename, string text)
    {
        using (StreamWriter file = new StreamWriter(path + $"/{filename}"))
        {
            await file.WriteAsync(text);
        }
        
        Console.WriteLine("[INFO] Arquivo contendo o texto normalizado criado com sucesso.");
    }
    
    public static async Task CreateFileWordCount(string path, string filename, Dictionary<string, int> dictionary)
    {
        await using (StreamWriter file = new StreamWriter(path + $"/{filename}"))
        {
            foreach (KeyValuePair<string, int> word in dictionary)
            {
                await file.WriteLineAsync($"{word.Value:D5} - {word.Key}");
            }
        }
        
        Console.WriteLine("[INFO] Arquivo contendo a contagem das repetições das palavras criado com sucesso.");
    }
}