using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Produit
{
    public int Reference { get; set; }

    public string DescriptionP { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public double Prix { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
