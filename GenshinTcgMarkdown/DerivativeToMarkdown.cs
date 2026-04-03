using GenshinDB.Tcg;
namespace GenshinTcgMarkdown
{
    public static class DerivativeToMarkdown
    {
        public static string Convert(VersionData versionData, int DerivativeId, int indentLevel = 1)
        {
            string indent = string.Concat(Enumerable.Repeat("> ", indentLevel));
            var sb = new System.Text.StringBuilder();
            var tcgObject = versionData.IdToTcgObject[DerivativeId];
            switch (tcgObject)
            {
                case Entity entity:
                    var entityType = TcgConstants.Map.TryGetValue(entity.type, out var dt) ? dt : entity.type;
                    sb.AppendLine($"{indent}**{entity.name}** {entityType}");
                    sb.AppendLine($"{indent}<br> {entity.description.Replace("\n", "<br>")}");
                    break;
                case Skill skill:
                    var skillType = TcgConstants.Map.TryGetValue(skill.type, out var st) ? st : skill.type;
                    var costStr = Util.CostStr(skill.playCost);
                    sb.AppendLine($"{indent}**{skill.name}** {skillType} {costStr}");
                    sb.AppendLine($"{indent}<br> {skill.description.Replace("\n", "<br>")}");
                    break;
                case ActionCard actionCard:
                    var cardType = TcgConstants.Map.TryGetValue(actionCard.type, out var ct) ? ct : actionCard.type;
                    var actionCostStr = Util.CostStr(actionCard.playCost);
                    var tagStr = Util.TagStr(actionCard.tags);
                    sb.AppendLine($"{indent}**{actionCard.name}** {cardType} {tagStr} {actionCostStr}");
                    sb.AppendLine($"{indent}<br> {actionCard.description.Replace("\n", "<br>")}");
                    break;
            }
            return sb.ToString();
        }
    }
    
}
    