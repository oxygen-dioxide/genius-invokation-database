namespace GenshinDB.Tcg
{
    public class Skill
    {
        public int id;
        public string name;
        public string type;
        public string rawDescription;
        public string description;
        public List<Cost> playCost;
        public bool hidden;
    }
}