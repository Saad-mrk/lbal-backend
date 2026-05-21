using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum UserRole
{
    User = 1,
    Admin = 2,
  
}
// StatutAnnonce.cs
public enum StatutAnnonce
{
    Draft = 5,
    PendingValidation = 6,
    Published = 7,
    Suspended = 8,
    Sold = 9,
    Archived = 10
}
// EtatProduit.cs
public enum EtatProduit
{
    NewWithTags = 4,
    NewWithoutTags = 5,
    VeryGood = 6,
    Good = 7,
    Acceptable = 8,
    Used = 9
}