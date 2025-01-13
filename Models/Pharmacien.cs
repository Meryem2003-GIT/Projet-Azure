using System;
using System.Collections.Generic;

namespace gestionPharmacieApp.Models;

public partial class Pharmacien
{
<<<<<<< HEAD
    public String Cin { get; set; }
=======
    public String? Cin { get; set; }
>>>>>>> 477c3e2c67237392deae329f2b414efcf2765d44

    public int IdPharmacien { get; set; }

    public string MotPasse { get; set; } = null!;

    public virtual Compte? CinNavigation { get; set; }
}
