using Base.Bot.Infrastructure;
using ELO.Bot.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ELO.Bot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BaseProgram.BaseBuildWebHost<Startup>(args).Run();
        }
    }
}
