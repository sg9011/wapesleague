using System.Threading.Tasks;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IProcessFileImportWorkflow
    {
        public Task ProcessFileImportAsync(int fileImportId);
    }
}
