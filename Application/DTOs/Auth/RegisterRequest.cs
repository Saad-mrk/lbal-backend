using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth;

public record RegisterRequest(
    string Nom,
    string Prenom,
    string Email,
    string Password,
    string? Telephone
);
