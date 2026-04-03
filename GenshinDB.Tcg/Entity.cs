using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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