using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Telephone { get; set; }
        public string? Role { get; set; }
        public bool? EstActif { get; set; }
        public DateTime? DateInscription { get; set; }
        public DateTime? DerniereConnexion { get; set; }
        public bool IsVerified { get; set; }
    }
}
