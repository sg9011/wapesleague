using Base.Bot.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using WaPesLeague.Bot.Infrastructure;

namespace WaPesLeague.Bot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BaseProgram.BaseBuildWebHost<Startup>(args).Run();
        }
    }
}
