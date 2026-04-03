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
            
            var tags = new List<string>();
            foreach (var tag in character.tags)
            {
                if (TcgConstants.Map.TryGetValue(tag, out var element))
                    tags.Add(element);
                else if (TcgConstants.Map.TryGetValue(tag, out var weapon))
                    tags.Add(weapon);
                else if (TcgConstants.Map.TryGetValue(tag, out var nation))
                    tags.Add(nation);
                else
                    tags.Add(tag);
            }
            sb.AppendLine(string.Join(" ", tags));

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

                var derivatives = ExtractDerivatives(versionData, skill.rawDescription);
                foreach (var derivative in derivatives)
                {
                    var entityType = TcgConstants.Map.TryGetValue(derivative.type, out var dt) ? dt : derivative.type;
                    sb.AppendLine($"> > **{derivative.name}** {entityType}<br>");
                    sb.AppendLine($"> > {derivative.description.Replace("\n", "<br>")}");
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

                    var talentTags = new List<string>();
                    foreach (var tag in talent.tags)
                    {
                        if (TcgConstants.Map.TryGetValue(tag, out var tt))
                            talentTags.Add(tt);
                        else
                            talentTags.Add(tag);
                    }

                    var costStr = Util.CostStr(talent.playCost);

                    sb.AppendLine($"> **{talent.name}** {string.Join(" ", talentTags)} {costStr}<br>");
                    sb.AppendLine($"> {talent.description.Replace("\n", "<br>")}");
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
