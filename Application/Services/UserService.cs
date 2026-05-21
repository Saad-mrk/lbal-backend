using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        // Ici, tu devrais normalement injecter AutoMapper pour simplifier les conversions

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            // Conversion Entity -> DTO (Mapping manuel pour l'exemple)
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Nom = user.Nom,
                Prenom = user.Prenom,
                EstActif = user.EstActif,

                Role = user.RoleId == 2 ? "admin" : "client",
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(UserFilterDto filter)
        {
            var query = _userRepository.GetAllQueryable();

            // Application des filtres
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(u => u.Email.Contains(filter.SearchTerm) || u.Nom.Contains(filter.SearchTerm));

            if (filter.EstActif.HasValue)
                query = query.Where(u => u.EstActif == filter.EstActif.Value);

            // Mapping et exécution de la requête
            return query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserDto { /* Mapping ici */ })
                .ToList();
        }
        // Dans Application/Services/UserService.cs
        public async Task<bool> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Nom = userDto.Nom;
            user.Prenom = userDto.Prenom;
            user.Email = userDto.Email;
            user.Telephone = userDto.Telephone;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.EstActif = false; // On fait un "Soft Delete"
            await _userRepository.UpdateAsync(user);
            return true;
        }
        public async Task UpdateLastConnectionAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.DerniereConnexion = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}
