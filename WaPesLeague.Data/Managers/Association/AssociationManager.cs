using System.Threading.Tasks;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class AssociationManager : IAssociationManager
    {
        private readonly WaPesDbContext _context;
        public AssociationManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Association.Association> AddAsync(Entities.Association.Association association)
        {
            await _context.Associations.AddAsync(association);
            await _context.SaveChangesAsync();

            return association;
        }
    }
}
