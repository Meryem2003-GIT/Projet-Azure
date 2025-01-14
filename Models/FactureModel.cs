using Microsoft.AspNetCore.Mvc.Rendering;

namespace gestionPharmacieApp.Models
{
    public class FactureModel
    {
        public int ClientId  { get; set; }   
        public int FactureId { get; set; }
        public double FactureTotal { get; set; }
        public string ClientNom { get; set; }
        public string ClientPrenom { get; set; }
        public string ClientEmail { get; set; }
        public List<ProduitDetailsModel> Produits { get; set; }
        public List<SelectListItem> ProduitsBD { get; set; }
    }
}
