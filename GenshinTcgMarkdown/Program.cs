using GenshinDB.Tcg;
using Newtonsoft.Json;
using GenshinTcgMarkdown;
using System.Text;
using System;
using System.Diagnostics.SymbolStore;
using System.Text.Json.Serialization;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;

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


/*List<Character> characters = JsonConvert.DeserializeObject<CharactersJson>(
    File.ReadAllText($"{inputFolder}/{version}/{language}/characters.json")
    ).data;
List<Entity> entities = JsonConvert.DeserializeObject<EntitiesJson>(
    File.ReadAllText($"{inputFolder}/{version}/{language}/entities.json")
    ).data;
List<ActionCard> actionCards = JsonConvert.DeserializeObject<ActionCardsJson>(
    File.ReadAllText($"{inputFolder}/{version}/{language}/actioncards.json")
    ).data;*/