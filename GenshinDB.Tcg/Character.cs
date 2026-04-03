using System.Linq;

namespace GenshinDB.Tcg
{
    public class Character: TcgObject
    {
        public bool obtainable;
        public string[] tags;
        public Skill[] skills;
        public int hp;
        public int maxenergy;
    }
}