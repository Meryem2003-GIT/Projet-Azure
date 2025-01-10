using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Commande
{
    public int IdCommande { get; set; }

    public DateOnly DateCommande { get; set; }

    public int Reference { get; set; }

    public int Quantite { get; set; }

    public int? IdFournisseur { get; set; }

    public virtual Fournisseur? IdFournisseurNavigation { get; set; }

    public virtual Produit ReferenceNavigation { get; set; } = null!;
}
