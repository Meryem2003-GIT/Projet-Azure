using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Facture
{
    public int IdFacture { get; set; }

    public double Total { get; set; }

    
    public virtual ICollection<Vente> Ventes { get; set; }
    
}
