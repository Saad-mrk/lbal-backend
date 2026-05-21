using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces;

using  Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategorieRepository : ICategorieRepository
    {
        private readonly AppDbContext _context;
        public CategorieRepository(AppDbContext context) => _context = context;

        // Dans ton implémentation du Repository ou directement dans ton DbContext
        public async Task<List<Categorie>> GetAllWithSousCategoriesAsync()
        {
            return await _context.CATEGORIE
                                 .Include(c => c.SousCategories)
                                 .ToListAsync();
        }
    }
}
