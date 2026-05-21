using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LBAL_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorieController : ControllerBase
    {
        private readonly CategorieService _categorieService;

        public CategorieController(CategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var tree = await _categorieService.GetCategoriesTreeAsync();
            return Ok(tree); // Renvoie le statut 200 avec le JSON hiérarchisé
        }
    }
}
