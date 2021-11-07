using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IDivisionGroupManager
    {
        public Task<DivisionGroup> GetDivisionGroupByGoogleSheetImportTypeIdAsync(int GoogleSheetImportTypId);
    }
}
