using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Client
{
<<<<<<< HEAD
    public String Cin { get; set; }
=======
    public String? Cin { get; set; }
>>>>>>> 477c3e2c67237392deae329f2b414efcf2765d44

    public int IdClient { get; set; }

    public virtual Compte? CinNavigation { get; set; }

    public virtual ProgFidelite ProgFidelites { get; set; } 

    public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
