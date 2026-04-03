using GenshinDB.Tcg;
namespace GenshinTcgMarkdown
{
    public class VersionData
    {
        public string Version { get; set; } = "";
        public List<Character> Characters { get; set; } = new();
        public List<Entity> Entities { get; set; } = new();
        public List<ActionCard> ActionCards { get; set; } = new();
        public List<Keyword> Keywords { get; set; } = new();
        public Dictionary<int, TcgObject> IdToTcgObject { get; set; } = new();
        public Dictionary<int, List<ActionCard>> CharacterTalent { get; set; } = new();
        public Dictionary<int, int> KeywordIdMapping { get; set; } = new();
        
        public void BuildIdDictionary()
        {
            Dictionary<string, int> NameToId = new();
            foreach (var character in Characters)
            {
                IdToTcgObject[character.id] = character;
                foreach (var skill in character.skills)
                {
                    IdToTcgObject[skill.id] = skill;
                }
            }
            foreach (var entity in Entities)
            {
                IdToTcgObject[entity.id] = entity;
                NameToId[entity.name] = entity.id;
                foreach (var skill in entity.skills)
                {
                    IdToTcgObject[skill.id] = skill;
                }
            }
            foreach (var actionCard in ActionCards)
            {
                IdToTcgObject[actionCard.id] = actionCard;
                NameToId[actionCard.name] = actionCard.id;
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
            foreach (var keyword in Keywords)
            {
                if (NameToId.ContainsKey(keyword.name))
                {
                    KeywordIdMapping[keyword.id] = NameToId[keyword.name];
                }
            }
        }
    }
}