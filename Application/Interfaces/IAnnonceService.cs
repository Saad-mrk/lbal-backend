using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAnnonceService
    {
        Task<int> CreateAnnonceAsync(AnnonceCreateDto dto, int userId);
        Task<IEnumerable<AnnonceDto>> GetAnnoncesByUserIdAsync(string statut, int userId);
    }
}
