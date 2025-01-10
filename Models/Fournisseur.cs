using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Fournisseur
{
    public int IdFournisseur { get; set; }

    public string NomSociete { get; set; } = null!;

    public string Adresse { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();
}
