using GenshinDB.Tcg;
using System.Text;
using System.Text.RegularExpressions;

namespace GenshinTcgMarkdown
{
    public static class CharacterToMarkdown
    {
        private static readonly Dictionary<string, string> SkillTypeMap = new()
        {
            { "GCG_SKILL_TAG_A", "普通攻击" },
            { "GCG_SKILL_TAG_E", "元素战技" },
            { "GCG_SKILL_TAG_Q", "元素爆发" },
            { "GCG_SKILL_TAG_PASSIVE", "被动" }
        };

        private static readonly Dictionary<string, string> CostTypeMap = new()
        {
            { "GCG_COST_DICE_CRYO", "冰" },
            { "GCG_COST_DICE_PYRO", "火" },
            { "GCG_COST_DICE_HYDRO", "水" },
            { "GCG_COST_DICE_ELECTRO", "雷" },
            { "GCG_COST_DICE_ANEMO", "风" },
            { "GCG_COST_DICE_GEO", "岩" },
            { "GCG_COST_DICE_DENDRO", "草" },
            { "GCG_COST_DICE_VOID", "黑" },
            { "GCG_COST_DICE_SAME", "白" },
            { "GCG_COST_ENERGY", "充" }
        };

        private static readonly Dictionary<string, string> ElementMap = new()
        {
            { "GCG_TAG_ELEMENT_CRYO", "冰" },
            { "GCG_TAG_ELEMENT_PYRO", "火" },
            { "GCG_TAG_ELEMENT_HYDRO", "水" },
            { "GCG_TAG_ELEMENT_ELECTRO", "雷" },
            { "GCG_TAG_ELEMENT_ANEMO", "风" },
            { "GCG_TAG_ELEMENT_GEO", "岩" },
            { "GCG_TAG_ELEMENT_DENDRO", "草" }
        };

        private static readonly Dictionary<string, string> WeaponMap = new()
        {
            { "GCG_TAG_WEAPON_SWORD", "单手剑" },
            { "GCG_TAG_WEAPON_CLAYMORE", "双手剑" },
            { "GCG_TAG_WEAPON_BOW", "弓" },
            { "GCG_TAG_WEAPON_POLEARM", "长柄武器" },
            { "GCG_TAG_WEAPON_CATALYST", "法器" }
        };

        private static readonly Dictionary<string, string> NationMap = new()
        {
            { "GCG_TAG_NATION_MONDSTADT", "蒙德" },
            { "GCG_TAG_NATION_LIYUE", "璃月" },
            { "GCG_TAG_NATION_INAZUMA", "稻妻" },
            { "GCG_TAG_NATION_SUMERU", "须弥" },
            { "GCG_TAG_NATION_FONTAINE", "枫丹" },
            { "GCG_TAG_NATION_NATLAN", "纳塔" },
            { "GCG_TAG_NATION_SNEZHNAYA", "至冬" }
        };

        private static readonly Dictionary<string, string> CardTypeMap = new()
        {
            { "GCG_CARD_STATE", "状态" },
            { "GCG_CARD_ONSTAGE", "出战状态" },
            { "GCG_CARD_SUMMON", "召唤物" },
            { "GCG_CARD_ATTACHMENT", "附着效果状态" },
            { "GCG_CARD_MODIFY", "装备" }
        };

        private static readonly Dictionary<string, string> TalentTagMap = new()
        {
            { "GCG_TAG_TALENT", "天赋" },
            { "GCG_TAG_SLOWLY", "战斗行动" }
        };

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
                if (ElementMap.TryGetValue(tag, out var element))
                    tags.Add(element);
                else if (WeaponMap.TryGetValue(tag, out var weapon))
                    tags.Add(weapon);
                else if (NationMap.TryGetValue(tag, out var nation))
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

                var skillType = SkillTypeMap.TryGetValue(skill.type, out var st) ? st : skill.type;
                
                var costParts = new List<string>();
                if (skill.playCost != null)
                {
                    foreach (var cost in skill.playCost)
                    {
                        if (cost == null || cost.costtype == null) continue;
                        var costType = CostTypeMap.TryGetValue(cost.costtype, out var ct) ? ct : cost.costtype;
                        costParts.Add($"{cost.count}{costType}");
                    }
                }
                var costStr = string.Join("", costParts);

                sb.AppendLine($"> **{skill.name}** {skillType} {costStr}");
                sb.AppendLine($"> <br>{skill.description.Replace("\n", "<br>")}");

                var derivatives = ExtractDerivatives(versionData, skill.rawDescription);
                foreach (var derivative in derivatives)
                {
                    var entityType = CardTypeMap.TryGetValue(derivative.type, out var dt) ? dt : derivative.type;
                    sb.AppendLine($"> > **{derivative.name}** {entityType}");
                    sb.AppendLine($"> > {derivative.description.Replace("\n", "<br>")}");
                }
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
                        if (TalentTagMap.TryGetValue(tag, out var tt))
                            talentTags.Add(tt);
                        else
                            talentTags.Add(tag);
                    }

                    var costParts = new List<string>();
                    if (talent.playCost != null)
                    {
                        foreach (var cost in talent.playCost)
                        {
                            if (cost == null || cost.costtype == null) continue;
                            var costType = CostTypeMap.TryGetValue(cost.costtype, out var ct) ? ct : cost.costtype;
                            costParts.Add($"{cost.count}{costType}");
                        }
                    }
                    var costStr = string.Join("", costParts);

                    sb.AppendLine($"> **{talent.name}** {string.Join(" ", talentTags)} {costStr}");
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
