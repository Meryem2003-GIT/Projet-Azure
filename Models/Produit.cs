using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestionPharmacieApp.Models;

public partial class Produit
{
    public int Reference { get; set; }

    [Display(Name = "Description")]
    public string DescriptionP { get; set; } = null!;

    public string Libelle { get; set; } = null!;

    public double Prix { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
