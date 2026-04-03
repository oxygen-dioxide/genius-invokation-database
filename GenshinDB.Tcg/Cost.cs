namespace GenshinDB.Tcg
{
    public class Cost
    {
        public string type;
        public int count;

        public Cost()
        {
            type = "GCG_COST_DICE_SAME";
            count = 0;
        }

        public Cost(string type, int count)
        {
            this.type = type;
            this.count = count;
        }
    }
}