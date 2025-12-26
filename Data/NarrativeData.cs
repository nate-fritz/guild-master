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

                "I had hoped to be around when you awoke, but my journey can be delayed no longer.<br><br>" +

                "It is hard to say how much you remember of your time here. At times, you seemed perfectly lucid, and at others you hardly knew your own name.<br><br>" +

                "Seventeen days ago I had closed and locked the door to this old guild hall behind me and started on my journey, only to find you laying in the road a few hundred paces from the front door. Pardon my honesty, but your timing could not have been worse.<br><br>" +

                $"Regardless, I patched you up to the best of my ability, and after several days you awoke. We spoke for some time before you eventually drifted back into a deep sleep. Over the following days, you would drift in and out of consciousness, and as often as you felt up to it, we spoke just enough for me to learn that you are {playerName}, an itinerant {className}.<br><br>" +

                "Yesterday, I received a missive that let me know that my departure was long past due, and as such, I am forced to leave you here on your own. Not ideal, but circumstances forced my hand.<br><br>" +

                "To make it up to you, I leave you the guild. Well, the guild hall anyway; you see, I was the last remaining member and Guild Master of the old Adventurer's Guild. Now that job belongs to you, if you will take it.<br><br>" +

                "I hope that you do. The present peace and stability of the empire should not be taken for granted. A time will soon come when protectors of the realm are needed. Try to find like-minded individuals - at least ten - and see if you can fill the coffers with gold once more. Saving the world can be expensive work.<br><br>" +

                "Time is of the essence. Try to rebuild the guild within the next hundred days. If I survive my journey, I will write to you then to see how things are coming along.<br><br>" +

                "Good fortune and may the gods watch over you.<br><br>" +

                "Signed,<br><br>" +

                "Alaron, former Guild Master of the Adventurer's Guild\"";
        }
    }
}
