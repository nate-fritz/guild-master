namespace GuildMaster.Data
{
    /// <summary>
    /// Contains narrative text and lore for the game.
    /// Provides a single source of truth for story elements to avoid duplication.
    /// </summary>
    public static class NarrativeData
    {
        /// <summary>
        /// Generates the welcome note from Alaron that players find at the start of the game.
        /// This letter explains the game's premise and sets the player's objective.
        /// </summary>
        /// <param name="playerName">The player's chosen character name</param>
        /// <param name="className">The player's chosen class name (Legionnaire, Venator, Oracle)</param>
        /// <returns>Formatted HTML string containing the complete welcome letter</returns>
        public static string GenerateWelcomeNote(string playerName, string className)
        {
            return "You pick up the note, unfold it, and begin reading the letter addressed to you.<br><br>" +

                $"\"Dear {playerName},<br><br>" +

                "Forgive me - I had hoped to be here when you woke, but my journey could wait no longer.<br><br>" +

                $"Seventeen days ago I locked the doors of this old guild hall for what I believed was the last time. I hadn't made it a hundred paces before I found you lying in the road. I carried you back inside and patched you up as best I could. You drifted in and out for days - long enough for me to learn that you are {playerName}, an itinerant {className}, and little else. Whatever brought you here, you kept it to yourself. But you did seem to enjoy my stories of this hall's glory days.<br><br>" +

                "So I leave you the guild - or what remains of it. I am the last member, and Guild Master, of the old Adventurer's Guild. Now the title is yours.<br><br>If you will take it.<br><br>" +

                "I hope that you do. Xanthea's long peace will not last forever, and a time is coming when protectors of the realm will be needed.<br><br>" +

                "Gather like-minded souls - at least ten - and fill the coffers once more; a hundred gold would be a fine start. Saving the world is expensive work. You have a hundred days; if I survive my journey, I will write to you then.<br><br>" +

                "Good fortune, and may the gods watch over you.<br><br>" +

                "Signed,<br><br>" +

                "Alaron, former Guild Master of the Adventurer's Guild\"";
        }
    }
}
