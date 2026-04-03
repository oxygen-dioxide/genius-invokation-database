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
    }
}