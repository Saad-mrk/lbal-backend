using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AnnonceRepository : IAnnonceRepository
    {
        private readonly AppDbContext _context;
        public AnnonceRepository(AppDbContext context) => _context = context;
        public async Task<Annonce> AddAsync(Annonce annonce)
        {
            await _context.Annonces.AddAsync(annonce);
            await _context.SaveChangesAsync();
            return annonce;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();


        public async Task<IEnumerable<Annonce>> GetById(int id, string status)
        {
            IQueryable<Annonce> query = _context.Annonces
                .Include(a => a.Photos)
                .Include(a => a.Categorie)
                .Include(a => a.SousCategorie)
                .Include(a => a.Attributs)
                    .ThenInclude(at => at.Attribut);   // ← syntaxe corrigée (suppression du "wher" parasite)

            // Toujours filtrer par utilisateur
            query = query.Where(a => a.id_utilisateur == id);

            if (!string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
            {
                // Valide que le statut fourni existe dans l'enum avant de caster
                if (!Enum.TryParse<StatutAnnonce>(status, ignoreCase: true, out var etatEnum))
                    throw new ArgumentException($"Statut invalide : {status}");

                int etatId = (int)etatEnum;
                query = query.Where(a => a.StatutId == etatId);
            }

            return await query.ToListAsync();
        }
    }
    }
