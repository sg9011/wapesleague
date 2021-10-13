using Base.Bot.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WaPesLeague.API.Infrastructure;

namespace WaPesLeague.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BaseProgram.BaseBuildWebHost<Startup>(args).Run();
        }
    }
}
