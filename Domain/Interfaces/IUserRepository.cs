using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    // --- LECTURE ---
    // On garde un seul GetById, pas besoin de deux méthodes identiques
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);

    // --- RECHERCHE ET PAGINATION ---
    // Cette méthode est la clé pour utiliser ton UserFilterDto
    // On retourne IQueryable pour que le filtrage se fasse CÔTÉ BASE DE DONNÉES
    IQueryable<User> GetAllQueryable();

    // --- ÉCRITURE ---
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id); // Ajout du delete (souvent nécessaire)

    // --- PERSISTANCE ---
    Task SaveChangesAsync(); // Important pour valider les transactions
}