using System.Collections.Generic;

namespace gestionPharmacieApp.Models
{
    public class ProduitDetailsViewModel
    {
        public Produit ProduitSelectionne { get; set; }
        public IEnumerable<Produit> ListeProduits { get; set; }
    }
}

