using AutoMapper;
using Base.Bot.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AssociationController : BaseApiController
    {
        private readonly IAssociationTenantManager _associationTenantManager;
        private readonly IAssociationManager _associationManager;
        public AssociationController(IAssociationManager associationManager, IAssociationTenantManager associationTenantManager, IMapper mapper) : base(mapper)
        {
            _associationTenantManager = associationTenantManager;
            _associationManager = associationManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTenants()
        {
            var tenants = await _associationTenantManager.GetAllAsync();
            return Ok("Damn Sexy!");
        }
    }
}
