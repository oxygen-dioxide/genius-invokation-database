using System.Linq;

namespace GenshinDB.Tcg
{
    public class Character
    {
        public int id;
        public string name;
        public int hp;
        public int maxenergy;
        public string[] tags;
        public string source;
        public Skill[] skills;
    }
}