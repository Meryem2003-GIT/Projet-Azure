using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Client
{
    public String Cin { get; set; }

    public int IdClient { get; set; }

    public virtual Compte? CinNavigation { get; set; }

    public virtual ICollection<ProgFidelite> ProgFidelites { get; set; } = new List<ProgFidelite>();

    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
