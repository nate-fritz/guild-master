namespace GuildMaster.Models
{
    public class DamageOverTime
    {
        public DamageType Type { get; set; }
        public int DamagePerTurn { get; set; }
        public int RemainingTurns { get; set; }
        public string SourceName { get; set; }  // Who applied this DOT

        public DamageOverTime(DamageType type, int damagePerTurn, int duration, string sourceName)
        {
            Type = type;
            DamagePerTurn = damagePerTurn;
            RemainingTurns = duration;
            SourceName = sourceName;
        }

        public DamageOverTime Clone()
        {
            return new DamageOverTime(Type, DamagePerTurn, RemainingTurns, SourceName);
        }
    }
}