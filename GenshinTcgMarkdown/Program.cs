using GenshinDB.Tcg;
using Newtonsoft.Json;
using GenshinTcgMarkdown;
using System.Text;
using System;
using System.Diagnostics.SymbolStore;
using System.Text.Json.Serialization;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.IO;

var inputFolder = "data";
var outputFolder = "markdown";
if(!Directory.Exists(outputFolder)){
    Directory.CreateDirectory(outputFolder);
}
string language = "CHS";

List<string> versions = Directory.EnumerateDirectories(inputFolder)
    .Select(d => Path.GetFileName(d))
    .OrderBy(v => v)
    .ToList();

List<VersionData> allVersionData = new();

Console.WriteLine($"Found {versions.Count} versions: {string.Join(", ", versions)}");

foreach (string version in versions)
{
    Console.WriteLine($"Processing version {version}...");
    
    string versionFolder = Path.Combine(inputFolder, version, language);
    
    if (!Directory.Exists(versionFolder))
    {
        Console.WriteLine($"  Language folder {language} not found, skipping.");
        continue;
    }
    
    string charactersPath = Path.Combine(versionFolder, "characters.json");
    string entitiesPath = Path.Combine(versionFolder, "entities.json");
    string actionCardsPath = Path.Combine(versionFolder, "action_cards.json");
    
    if (!File.Exists(charactersPath) || !File.Exists(entitiesPath) || !File.Exists(actionCardsPath))
    {
        Console.WriteLine($"  Missing required files, skipping.");
        continue;
    }
    
    try
    {
        Console.WriteLine($"  Loading data...");
        
        var charactersJson = File.ReadAllText(charactersPath);
        var entitiesJson = File.ReadAllText(entitiesPath);
        var actionCardsJson = File.ReadAllText(actionCardsPath);
        
        var characters = JsonConvert.DeserializeObject<CharactersJson>(charactersJson)?.data;
        var entities = JsonConvert.DeserializeObject<EntitiesJson>(entitiesJson)?.data;
        var actionCards = JsonConvert.DeserializeObject<ActionCardsJson>(actionCardsJson)?.data;
        
        if (characters == null || entities == null || actionCards == null)
        {
            Console.WriteLine($"  Failed to deserialize data.");
            continue;
        }
        
        Console.WriteLine($"  Successfully loaded {characters.Count} characters, {entities.Count} entities, {actionCards.Count} action cards.");
        
        var versionData = new VersionData
        {
            Version = version,
            Characters = characters,
            Entities = entities,
            ActionCards = actionCards
        };
        
        versionData.BuildIdDictionary();
        allVersionData.Add(versionData);
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Error processing version {version}: {ex.Message}");
    }
}

var latestVersion = allVersionData.LastOrDefault();
if (latestVersion != null)
{
    var markdown = CharacterToMarkdown.Convert(latestVersion, 1101);
    Console.WriteLine(markdown);
}