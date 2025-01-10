using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Facture
{
    public int IdFacture { get; set; }

    public int Total { get; set; }

    public int? IdVente { get; set; }

    public virtual Vente? IdVenteNavigation { get; set; }
}
