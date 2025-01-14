using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace gestionPharmacieApp.Models;

public partial class Vente
{
    public int IdVente { get; set; }

    public DateOnly DateVente { get; set; }
    [Required]
    public int Reference { get; set; }
    [Required]

    [Range(1, 30, ErrorMessage = "La quantité doit être supérieure à 0.")]
    public int Quantite { get; set; }

    public int IdClient { get; set; }
    public int? IdFacture { get; set; }

    public virtual Facture IdFactureNavigation { get; set; } = null!; 
    

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Produit ReferenceNavigation { get; set; } = null!;
}
