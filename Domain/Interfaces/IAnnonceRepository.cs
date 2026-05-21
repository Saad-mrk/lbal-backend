using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAnnonceRepository
    {
        Task<Annonce> AddAsync(Annonce annonce);
        Task SaveChangesAsync();
        // liste des annonces d'un utilisateur
        Task<IEnumerable<Annonce>> GetById(int id , string status);
    }
}
