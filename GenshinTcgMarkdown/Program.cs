using GenshinDB.Tcg;
using Newtonsoft.Json;
using GenshinTcgMarkdown;
using System.IO;
using System.Linq;

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
    string keywordsPath = Path.Combine(versionFolder, "keywords.json");
    
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
        var keywordsJson = File.ReadAllText(keywordsPath);
        
        var characters = JsonConvert.DeserializeObject<CharactersJson>(charactersJson)?.data;
        var entities = JsonConvert.DeserializeObject<EntitiesJson>(entitiesJson)?.data;
        var actionCards = JsonConvert.DeserializeObject<ActionCardsJson>(actionCardsJson)?.data;
        var keywords = JsonConvert.DeserializeObject<KeywordsJson>(keywordsJson)?.data;
        if (characters == null || entities == null || actionCards == null || keywords == null)
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
            ActionCards = actionCards,
            Keywords = keywords
        };
        
        versionData.BuildIdDictionary();
        allVersionData.Add(versionData);
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  Error processing version {version}: {ex.Message}");
    }
}

if (allVersionData.Count == 0)
{
    Console.WriteLine("No version data loaded.");
    return;
}

var latestVersion = allVersionData.Last();
var obtainableCharacters = latestVersion.Characters
    .Where(c => c.obtainable)
    .ToList();
var obtainableActionCards = latestVersion.ActionCards
    .Where(a => a.obtainable)
    .ToList();
var adventurePlaces = latestVersion.ActionCards
    .Where(e => e.tags.Contains("GCG_TAG_ADVENTURE_PLACE"))
    .ToList();
Console.WriteLine($"Latest version: {latestVersion.Version}");
Console.WriteLine($"Obtainable characters: {obtainableCharacters.Count}");
Console.WriteLine($"Obtainable action cards: {obtainableActionCards.Count}");
Console.WriteLine($"Adventure places: {adventurePlaces.Count}");

Console.WriteLine("Processing character cards...");

string footer(List<string> changedVersions, string currentVersion)
{
    if (changedVersions.Count <= 1) return "";
    return "\n版本历史：" 
    + string.Join(
        ", ", 
        changedVersions.Select(
            v => v == currentVersion ? $"**{v}**" : $"[{v}]({v}.md)"
        )
    ) + "\n";
}

foreach (var characterId in obtainableCharacters.Select(c => c.id))
{
    var prevMarkdown = "";
    var stringsToWrite = new string[allVersionData.Count];
    var changedVersions = new List<string>();
    
    for (int i = 0; i < allVersionData.Count; i++)
    {
        var version = allVersionData[i];
        var prevVersion = i > 0 ? allVersionData[i - 1] : null;
        if(!version.IdToTcgObject.ContainsKey(characterId)) continue;

        var character = version.IdToTcgObject[characterId] as Character;
        if (character == null) continue;
        var currMarkdown = CharacterToMarkdown.Convert(version, characterId);
        bool hasChanged = currMarkdown != prevMarkdown;
        if (hasChanged)
        {
            string header = 
                "---\n" +
                $"title: {character.name} {characterId}@{version.Version}\n" +
                "---\n\n";
            stringsToWrite[i] = header + currMarkdown;
            changedVersions.Add(version.Version);
        }
        else
        {
            stringsToWrite[i] = 
                "---\n" + 
                $"redirect_to: /characters/{characterId}/{changedVersions.LastOrDefault()}\n"+
                "---\n\n"; // No change, no need to write
        }
        prevMarkdown = currMarkdown;
    }
    
    for (int i = 0; i < stringsToWrite.Length; i++)
    {
        if (stringsToWrite[i] != null)
        {
            if(!stringsToWrite[i].Contains("redirect_to:"))
            {
                stringsToWrite[i] += footer(changedVersions, allVersionData[i].Version);
            }
            var folder = Path.Combine(outputFolder, "characters", characterId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            };
            var filePath = Path.Combine(folder, $"{allVersionData[i].Version}.md");
            File.WriteAllText(filePath, stringsToWrite[i]);
        }
    }
}

Console.WriteLine($"Generated markdown for {obtainableCharacters.Count} characters");

Console.WriteLine("Processing action cards...");

foreach (var actionCardId in obtainableActionCards.Concat(adventurePlaces).Select(a => a.id))
{
    var prevMarkdown = "";
    var stringsToWrite = new string[allVersionData.Count];
    var changedVersions = new List<string>();

    for (int i = 0; i < allVersionData.Count; i++)
    {
        var version = allVersionData[i];
        var prevVersion = i > 0 ? allVersionData[i - 1] : null;
        if(!version.IdToTcgObject.ContainsKey(actionCardId)) continue;

        var actionCard = version.IdToTcgObject[actionCardId] as ActionCard;
        if (actionCard == null) continue;
        var currMarkdown = ActionCardToMarkdown.Convert(version, actionCardId);
        bool hasChanged = currMarkdown != prevMarkdown;
        if (hasChanged)
        {
            string header = 
                "---\n" +
                $"title: {actionCard.name} {actionCardId}@{version.Version}\n" +
                "---\n\n";
            stringsToWrite[i] = header + currMarkdown;
            changedVersions.Add(version.Version);
        }
        else
        {
            stringsToWrite[i] = 
                "---\n" + 
                $"redirect_to: /action_cards/{actionCardId}/{changedVersions.LastOrDefault()}\n"+
                "---\n\n"; // No change, no need to write
        }
        prevMarkdown = currMarkdown;
    }
    
    for (int i = 0; i < stringsToWrite.Length; i++)
    {
        if (stringsToWrite[i] != null)
        {
            if(!stringsToWrite[i].Contains("redirect_to:"))
            {
                stringsToWrite[i] += footer(changedVersions, allVersionData[i].Version);
            }
            var folder = Path.Combine(outputFolder, "action_cards", actionCardId.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            };
            var filePath = Path.Combine(folder, $"{allVersionData[i].Version}.md");
            File.WriteAllText(filePath, stringsToWrite[i]);
        }
    }
}

Console.WriteLine($"Generated markdown for {obtainableActionCards.Count + adventurePlaces.Count} action cards");

Console.WriteLine("Generating index.md...");

var indexContent = "# 主页\n## 角色牌\n";
foreach (var character in obtainableCharacters.OrderBy(c => c.id))
{
    indexContent += $"- [{character.name}](characters/{character.id}/{latestVersion.Version}.md)\n";
}

indexContent += "\n## 行动牌\n";
foreach (var actionCard in obtainableActionCards.OrderBy(a => a.id))
{
    indexContent += $"- [{actionCard.name}](action_cards/{actionCard.id}/{latestVersion.Version}.md)\n";
}

indexContent += "\n## 冒险地点\n";
foreach (var actionCard in adventurePlaces.OrderBy(a => a.id))
{
    indexContent += $"- [{actionCard.name}](action_cards/{actionCard.id}/{latestVersion.Version}.md)\n";
}

File.WriteAllText(Path.Combine(outputFolder, "index.md"), indexContent);
Console.WriteLine("Done!");