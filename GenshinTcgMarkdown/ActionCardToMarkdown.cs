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

            foreach (var tag in actionCard.tags)
            {
                if (tag == "GCG_TAG_NON_DISCOVERABLE") continue;
                if (TcgConstants.Map.TryGetValue(tag, out var nation))
                    infoParts.Add(nation);
                else
                    infoParts.Add(tag);
            }

            if (actionCard.playCost != null)
            {
                var costParts = new List<string>();
                foreach (var cost in actionCard.playCost)
                {
                    if (cost == null || cost.type == null) continue;
                    var costType = TcgConstants.Map.TryGetValue(cost.type, out var ctp) ? ctp : cost.type;
                    costParts.Add($"{cost.count}{costType}");
                }
                infoParts.Add(string.Join("", costParts));
            }

            sb.AppendLine(string.Join(" ", infoParts));

            sb.AppendLine();
            sb.AppendLine(actionCard.description.Replace("\n", "<br>"));

            var derivatives = ExtractDerivatives(versionData, actionCard.rawDescription);
            foreach (var derivative in derivatives)
            {
                var entityType = TcgConstants.Map.TryGetValue(derivative.type, out var dt) ? dt : derivative.type;
                sb.AppendLine();
                sb.AppendLine($"> **{derivative.name}** {entityType}");
                sb.AppendLine($"> <br> {derivative.description.Replace("\n", "<br>")}");
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
