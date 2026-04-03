using GenshinDB.Tcg;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GenshinTcgMarkdown
{
    public static class CharacterToMarkdown
    {
        public static string Convert(VersionData versionData, int characterId)
        {
            if (!versionData.IdToTcgObject.TryGetValue(characterId, out var tcgObject))
            {
                return $"Character with id {characterId} not found";
            }

            var character = tcgObject as Character;
            if (character == null)
            {
                return $"Object with id {characterId} is not a character";
            }

            var sb = new StringBuilder();

            sb.AppendLine($"# {character.name}");
            
            var tagStr = Util.TagStr(character.tags);
            sb.AppendLine(tagStr);

            sb.AppendLine();
            sb.AppendLine($"生命值：{character.hp}");

            sb.AppendLine();
            sb.AppendLine("### 技能");

            foreach (var skill in character.skills)
            {
                if (skill.hidden) continue;

                var skillType = TcgConstants.Map.TryGetValue(skill.type, out var st) ? st : skill.type;
                var costStr = Util.CostStr(skill.playCost);

                sb.AppendLine($"> **{skill.name}** {skillType} {costStr}<br>");
                sb.AppendLine($"> {skill.description.Replace("\n", "<br>")}");

                var derivatives = Util.ExtractDerivatives(versionData, skill.id);
                foreach (var derivative in derivatives.Skip(1))
                {
                    sb.Append(DerivativeToMarkdown.Convert(versionData, derivative, 2));
                    sb.AppendLine($">");
                }
                sb.AppendLine("");
            }

            if (versionData.CharacterTalent.TryGetValue(characterId, out var talents) && talents.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("### 天赋牌");

                foreach (var talent in talents)
                {
                    if (!talent.obtainable) continue;
                    var talentTagStr = Util.TagStr(talent.tags);
                    var costStr = Util.CostStr(talent.playCost);

                    sb.AppendLine($"> **{talent.name}** {talentTagStr} {costStr}<br>");
                    sb.AppendLine($"> {talent.description.Replace("\n", "<br>")}");
                    var derivatives = Util.ExtractDerivatives(versionData, talent.id);
                    foreach (var derivative in derivatives.Skip(1))
                    {
                        sb.Append(DerivativeToMarkdown.Convert(versionData, derivative, 2));
                        sb.AppendLine($">");
                    }
                }
            }

            return sb.ToString();
        }

        private static List<Entity> ExtractDerivatives(VersionData versionData, string rawDescription)
        {
            var result = new List<Entity>();
            var regex = new Regex(@"\$\[C(\d+)\]");
            var matches = regex.Matches(rawDescription);

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int entityId))
                {
                    if (versionData.IdToTcgObject.TryGetValue(entityId, out var obj) && obj is Entity entity)
                    {
                        result.Add(entity);
                    }
                }
            }

            return result;
        }
    }
}
