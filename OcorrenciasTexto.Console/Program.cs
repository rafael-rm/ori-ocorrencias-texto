using OcorrenciasTexto.Kernel.Controllers;
using OcorrenciasTexto.Kernel.Repositories;

namespace OcorrenciasTexto.Console;

abstract class Program
{
    static async Task Main()
    {
        string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
        
        string pathFolderHtmls = projectDirectory + "/Data/htmlFiles";
        string pathFolderResults = projectDirectory + "/Data/Results";
        
        Dictionary<string, string> filesText = await DataController.GetTreatedTextFiles(pathFolderHtmls);

        string allText = await DataController.GetAllTextDictonary(filesText);
        
        Dictionary<string, int> wordCount = await DataController.GetRepetitionCount(allText);

        wordCount = await DataController.OrderRepetition(wordCount);

        await DataRepository.CreateFilePlainText(pathFolderResults, "treatedtext.txt", allText);
        
        await DataRepository.CreateFileWordCount(pathFolderResults, "wordcount.txt", wordCount);
    }
}