using System;
using System.Threading.Tasks;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IDivisionGroupRoundManager
    {
        public Task DeleteByDivisionGroupIdAsync(int divisionGroupId);
        
    }
}
