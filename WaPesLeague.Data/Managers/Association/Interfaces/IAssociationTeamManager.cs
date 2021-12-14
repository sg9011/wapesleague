using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IAssociationTeamManager
    {
        public Task<AssociationTeam> AddAsync(AssociationTeam accociationTeam);
        public Task<IReadOnlyCollection<AssociationTeam>> GetAssociationTeamsByAssociationIdAsync(int associationId);
        public Task<List<AssociationTeam>> AddMultipleAsync(List<AssociationTeam> associationTeams);


    }
}
