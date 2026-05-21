using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersAsync(UserFilterDto filter);
        Task<bool> UpdateUserAsync(int id, UserDto userDto);
        Task<bool> DeleteUserAsync(int id); // Souvent un "Soft Delete" (est_actif = false)
        Task UpdateLastConnectionAsync(int userId); // Nouvelle méthode pour mettre à jour la dernière connexion

    }
}
