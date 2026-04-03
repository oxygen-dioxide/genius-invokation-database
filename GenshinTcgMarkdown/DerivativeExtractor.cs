using GenshinDB.Tcg;
using System.Linq;
using System.Text.RegularExpressions;

namespace GenshinTcgMarkdown
{
    public class DerivativeExtractor
    {
        public List<int> result = new List<int>();
        Regex cRegex = new Regex(@"\$\[[CS](\d+)\]");
        Regex kRegex = new Regex(@"\$\[K(\d+)\]");
        public void Process(VersionData versionData, int id)
        {
            if(result.Contains(id))
            {
                return;
            }
            if (!versionData.IdToTcgObject.ContainsKey(id))
            {
                return;
            }
            result.Add(id);
            var tcgObject = versionData.IdToTcgObject[id];
            List<int> childrenIds = cRegex.Matches(tcgObject.rawDescription)
                .Select(m => int.Parse(m.Groups[1].Value))
                .Concat(kRegex.Matches(tcgObject.rawDescription)
                    .Where(m => versionData.KeywordIdMapping.ContainsKey(- int.Parse(m.Groups[1].Value)))
                    .Select(m => versionData.KeywordIdMapping[- int.Parse(m.Groups[1].Value)])
                )
                .ToList();
            foreach(var childId in childrenIds)
            {
                Process(versionData, childId);
            }
        }
    }
}