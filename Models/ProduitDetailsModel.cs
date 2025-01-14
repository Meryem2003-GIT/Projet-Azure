namespace gestionPharmacieApp.Models
{
    public class ProduitDetailsModel
    {
        public string ProduitLibelle { get; set; }
        public double ProduitPrix { get; set; }
        public int Quantite { get; set; }
        public double MontantProduit { get; set; }
        public double? Remise { get; set; }
    }
}
