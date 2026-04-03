namespace GenshinDB.Tcg
{
    public class Skill : TcgObject
    {
        public string type;
        public List<Cost> playCost;
        public bool hidden;
    }
}