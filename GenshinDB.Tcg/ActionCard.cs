using System.Collections.Generic;
namespace GenshinDB.Tcg
{
    public class ActionCard
    {
        public int id;
        public bool obtainable;
        public string name;
        public string[] tags;
        public int? relatedCharacterId;
        public List<Cost> playCost;
        public string rawDescription;
        public string description;
    }
}