using System;
using GuildMaster.Managers;

namespace GuildMaster.Services
{
    public static class ProgramStatics
    {
        public static MessageManager? messageManager { get; set; }
        public static EventManager? eventManager { get; set; }

        /// <summary>
        /// Shared Random instance for consistent RNG quality across the application.
        /// Using multiple Random instances created in quick succession can produce identical sequences.
        /// </summary>
        public static Random Random { get; } = new Random();
    }
}
