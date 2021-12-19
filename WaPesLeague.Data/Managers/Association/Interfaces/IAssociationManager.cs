using System.Threading.Tasks;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IAssociationManager
    {
        public Task<Entities.Association.Association> AddAsync(Entities.Association.Association accociation);
        public Task<Entities.Association.Association> GetAssociationByDivisionGroupIdAsync(int divisionGroupId);
    }
}
