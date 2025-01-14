namespace gestionPharmacieApp.Models
{
    public class ProduitFacture
    {
        public int Reference { get; set; }
        public string Libelle { get; set; }
        public string Description { get; set; }
        public double Prix { get; set; }
        public int Quantite { get; set; }
        public double TotalFacture { get; set; }
    }
}
