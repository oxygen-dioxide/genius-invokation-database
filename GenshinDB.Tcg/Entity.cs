namespace GenshinDB.Tcg
{
    public class Entity : TcgObject
    {
        public string type;
        public List<Skill> skills;
        public string category;
        public bool hidden;
    }
}