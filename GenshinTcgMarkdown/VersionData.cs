using GenshinDB.Tcg;
namespace GenshinTcgMarkdown
{
    public class VersionData
    {
        public string Version { get; set; } = "";
        public List<Character> Characters { get; set; } = new();
        public List<Entity> Entities { get; set; } = new();
        public List<ActionCard> ActionCards { get; set; } = new();
        public Dictionary<int, TcgObject> IdToTcgObject { get; set; } = new();
        public Dictionary<int, List<ActionCard>> CharacterTalent { get; set; } = new();
        
        public void BuildIdDictionary()
        {
            foreach (var character in Characters)
            {
                IdToTcgObject[character.id] = character;
            }
            foreach (var entity in Entities)
            {
                IdToTcgObject[entity.id] = entity;
            }
            foreach (var actionCard in ActionCards)
            {
                IdToTcgObject[actionCard.id] = actionCard;
                
                if (actionCard.relatedCharacterId.HasValue)
                {
                    int charId = actionCard.relatedCharacterId.Value;
                    if (!CharacterTalent.ContainsKey(charId))
                    {
                        CharacterTalent[charId] = new List<ActionCard>();
                    }
                    CharacterTalent[charId].Add(actionCard);
                }
            }
        }
    }
}