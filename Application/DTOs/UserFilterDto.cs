using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// UserFilterDto.cs
namespace Application.DTOs
{
    public class UserFilterDto
    {
        public string? SearchTerm { get; set; } // Recherche sur Nom/Email
        public string? Role { get; set; }
        public bool? EstActif { get; set; }

        // Pagination de base
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
