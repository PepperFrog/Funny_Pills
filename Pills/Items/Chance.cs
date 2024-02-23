namespace FunnyPills
{
    public class Chance
    {
        public int MinimumChance { get; set; }
        public int MaximumChance { get; set; }

        public Chance(int minimumChance, int maximumChance)
        {
            MinimumChance = minimumChance;
            MaximumChance = maximumChance;
        }
    }
}