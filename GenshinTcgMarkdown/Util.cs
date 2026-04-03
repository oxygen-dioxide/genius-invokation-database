using GenshinDB.Tcg;
namespace GenshinTcgMarkdown
{
    public static class Util{
        public static string CostStr(List<Cost>? playCost)
        {
            if(playCost == null || playCost.Count == 0)
            {
                playCost = new List<Cost>
                {
                    new Cost(),
                };
            }
            var costParts = new List<string>();
            foreach (var cost in playCost)
            {
                var costType = TcgConstants.Map.TryGetValue(cost.type, out var ct) ? ct : cost.type;
                costParts.Add($"{cost.count}{costType}");
            }
            return string.Join("", costParts);
        }

        public static string TagStr(string[] tags)
        {
            return string.Join(
                " ", 
                tags
                    .Select(tag => TcgConstants.Map.TryGetValue(tag, out var t) ? t : tag)
                    .Where(t => !string.IsNullOrEmpty(t))
            );
        }

        /// <summary>
        /// 从描述中递归提取衍生物id，返回列表第一个元素为原始id，后续元素为衍生物id
        /// </summary>
        public static List<int> ExtractDerivatives(VersionData versionData, int id)
        {
            var extractor = new DerivativeExtractor();
            extractor.Process(versionData, id);
            return extractor.result;
        }
    }
}