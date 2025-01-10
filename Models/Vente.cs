using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Vente
{
    public int IdVente { get; set; }

    public DateOnly DateVente { get; set; }

    public int Reference { get; set; }

    public int Quantite { get; set; }

    public int IdClient { get; set; }

    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Produit ReferenceNavigation { get; set; } = null!;
}
