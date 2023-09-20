using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using OcorrenciasTexto.Kernel.Repositories;
using StopWord;

namespace OcorrenciasTexto.Kernel.Controllers;

public class DataController
{
    public static async Task<Dictionary<string, string>> GetTreatedTextFiles(string filePath)
    {
        Dictionary<string, string> dictionaryFiles = await DataRepository.GetPlainTextFiles(filePath);

        foreach (KeyValuePair<string, string> file in dictionaryFiles)
        {
            string text = file.Value;
            text = Regex.Replace(text, "<.*?>", string.Empty);
            text = WebUtility.HtmlDecode(text);
            text = text.Replace("-", " ");
            text = Regex.Replace(text, @"\s+", " ");
            text = text.ToLower();
            text = Regex.Replace(text, @"[:;()\[\].,?!<>]", string.Empty);
            text = Regex.Replace(text, @"[^\p{L}\d\sÀ-ÖØ-öø-ÿ]", string.Empty);
            text = text.Normalize(NormalizationForm.FormD);
            
            text = new string(text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
            dictionaryFiles[file.Key] = text;
            
            Console.WriteLine($"[INFO] Texto do arquivo {file.Key} limpo e normalizado com sucesso.");
        }

        Console.WriteLine($"[INFO] Todos os textos dos {dictionaryFiles.Count} foram limpos e normalizados com sucesso.");
        
        return dictionaryFiles;
    }

    public static Task<Dictionary<string, int>> GetRepetitionCount(string text)
    {
        Dictionary<string, int> wordCount = new Dictionary<string, int>();
        
        Console.WriteLine("[INFO] Iniciando contagem das repetições de palavras.");

        foreach (string word in text.Split(' '))
        {
            if (string.IsNullOrEmpty(word))
                continue;

            if (wordCount.ContainsKey(word))
            {
                wordCount[word]++;
                Console.WriteLine($"[INFO] Palavra {word} somada no dicionario, contagem atual: {wordCount[word]}");
            }
            else
            {
                wordCount[word] = 1;
                Console.WriteLine($"[INFO] Palavra {word} adicionada ao dicionario, contagem atual: {wordCount[word]}");
            }
        }
        
        Console.WriteLine("[INFO] Contagem de repetições de palavras finalizada com sucesso.");

        return Task.FromResult(wordCount);
    }

    public static Task<Dictionary<string, int>> OrderRepetition(Dictionary<string, int> dictionary, bool descending = true)
    {
        Dictionary<string, int> orderedDictionary;

        orderedDictionary = descending 
            ? dictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value) 
            : dictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        Console.WriteLine("[INFO] Dicionario ordenado com sucesso.");
        
        return Task.FromResult(orderedDictionary);
    }

    public static Task<string> GetAllTextDictonary(Dictionary<string, string> dictionary)
    {
        string allText = string.Empty;
        
        foreach (KeyValuePair<string, string> file in dictionary)
        {
            allText += file.Value;
        }
        
        Console.WriteLine($"$[INFO] O texto de {dictionary.Count} arquivos foi mesclado com sucesso.");
        Console.WriteLine($"$[INFO] Caracteres totais: {allText.Length}");
        
        return Task.FromResult(allText);
    }

    public static Task<string> RemoveStopWords(string input, string shortLanguageName)
    {
        return Task.FromResult(input.RemoveStopWords(shortLanguageName));
    }
}