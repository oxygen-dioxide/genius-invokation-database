using GenshinDB.Tcg;
using System.Text;
using System.Text.RegularExpressions;

namespace GenshinTcgMarkdown
{
    public static class ActionCardToMarkdown
    {
        public static string Convert(VersionData versionData, int actionCardId)
        {
            if (!versionData.IdToTcgObject.TryGetValue(actionCardId, out var tcgObject))
            {
                return $"ActionCard with id {actionCardId} not found";
            }

            var actionCard = tcgObject as ActionCard;
            if (actionCard == null)
            {
                return $"Object with id {actionCardId} is not an action card";
            }

            var sb = new StringBuilder();

            sb.AppendLine($"# {actionCard.name}");

            var infoParts = new List<string>();
            var cardType = TcgConstants.Map.TryGetValue(actionCard.type, out var ct) ? ct : actionCard.type;
            infoParts.Add(cardType);
            var tagStr = Util.TagStr(actionCard.tags);
            if (!string.IsNullOrEmpty(tagStr))
            {
                infoParts.Add(tagStr);
            }

            infoParts.Add(Util.CostStr(actionCard.playCost));            

            sb.AppendLine(string.Join(" ", infoParts));

            sb.AppendLine();
            sb.AppendLine(actionCard.description.Replace("\n", "<br>"));

            var derivativeIds = Util.ExtractDerivatives(versionData, actionCard.id);
            foreach (var derivativeId in derivativeIds.Skip(1))
            {
                sb.AppendLine();
                sb.AppendLine(DerivativeToMarkdown.Convert(versionData, derivativeId));
            }

            return sb.ToString();
        }

        private static List<Entity> ExtractDerivatives(VersionData versionData, string rawDescription)
        {
            var result = new List<Entity>();
            var seenIds = new HashSet<int>();
            var regex = new Regex(@"\$\[C(\d+)\]");
            var matches = regex.Matches(rawDescription);

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int entityId))
                {
                    if (seenIds.Contains(entityId)) continue;
                    if (versionData.IdToTcgObject.TryGetValue(entityId, out var obj) && obj is Entity entity)
                    {
                        seenIds.Add(entityId);
                        result.Add(entity);
                    }
                }
            }

            return result;
        }
    }
}
