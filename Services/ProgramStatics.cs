using GuildMaster.Managers;

namespace GuildMaster.Services
{
    public static class ProgramStatics
    {
        public static MessageManager? messageManager { get; set; }
        public static EventManager? eventManager { get; set; }
    }
}
