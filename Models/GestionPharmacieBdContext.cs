using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace gestionPharmacieApp.Models;

public partial class GestionPharmacieBdContext : DbContext
{
    public GestionPharmacieBdContext()
    {
    }

    public GestionPharmacieBdContext(DbContextOptions<GestionPharmacieBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<Fournisseur> Fournisseurs { get; set; }

    public virtual DbSet<Pharmacien> Pharmaciens { get; set; }

    public virtual DbSet<Produit> Produits { get; set; }

    public virtual DbSet<ProgFidelite> ProgFidelites { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Vente> Ventes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-MGAJ56I\\MIMSSQL; Database=GestionPharmacieBD;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("PK__Client__6EC2B6C001323562");

            entity.ToTable("Client");

            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.Cin).HasColumnName("CIN");

            entity.HasOne(d => d.CinNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.Cin)
                .HasConstraintName("FK__Client__CIN__3E52440B");
        });

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.IdCommande).HasName("PK__Commande__385131BF637F10F4");

            entity.ToTable("Commande");

            entity.Property(e => e.IdCommande).HasColumnName("id_commande");
            entity.Property(e => e.DateCommande).HasColumnName("date_commande");
            entity.Property(e => e.IdFournisseur).HasColumnName("id_fournisseur");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.Reference).HasColumnName("reference");

            entity.HasOne(d => d.IdFournisseurNavigation).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.IdFournisseur)
                .HasConstraintName("FK__Commande__id_fou__47DBAE45");

            entity.HasOne(d => d.ReferenceNavigation).WithMany(p => p.Commandes)
                .HasForeignKey(d => d.Reference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commande__refere__46E78A0C");
        });

        modelBuilder.Entity<Compte>(entity =>
        {
            entity.HasKey(e => e.Cin).HasName("PK__Compte__C1F8DC579DAE2561");

            entity.ToTable("Compte");

            entity.Property(e => e.Cin)
                .ValueGeneratedNever()
                .HasColumnName("CIN");
            entity.Property(e => e.Adresse)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("adresse");
            entity.Property(e => e.DateNaissance).HasColumnName("date_naissance");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom");
            entity.Property(e => e.Prenom)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("prenom");
            entity.Property(e => e.Telephone)
                .HasMaxLength(1)
                .HasColumnName("telephone");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.IdFacture).HasName("PK__Facture__6C08ED57FA498AC6");

            entity.ToTable("Facture");

            entity.Property(e => e.IdFacture).HasColumnName("id_facture");
            entity.Property(e => e.IdVente).HasColumnName("id_vente");
            entity.Property(e => e.Total).HasColumnName("total");

            entity.HasOne(d => d.IdVenteNavigation).WithMany(p => p.Factures)
                .HasForeignKey(d => d.IdVente)
                .HasConstraintName("FK__Facture__id_vent__5165187F");
        });

        modelBuilder.Entity<Fournisseur>(entity =>
        {
            entity.HasKey(e => e.IdFournisseur).HasName("PK__Fourniss__5B874F948CEDABEE");

            entity.ToTable("Fournisseur");

            entity.Property(e => e.IdFournisseur).HasColumnName("id_fournisseur");
            entity.Property(e => e.Adresse)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("adresse");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.NomSociete)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_societe");
        });

        modelBuilder.Entity<Pharmacien>(entity =>
        {
            entity.HasKey(e => e.IdPharmacien).HasName("PK__Pharmaci__05C3D25DEFC06BED");

            entity.ToTable("Pharmacien");

            entity.Property(e => e.IdPharmacien).HasColumnName("id_pharmacien");
            entity.Property(e => e.Cin).HasColumnName("CIN");
            entity.Property(e => e.MotPasse)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("mot_passe");

            entity.HasOne(d => d.CinNavigation).WithMany(p => p.Pharmaciens)
                .HasForeignKey(d => d.Cin)
                .HasConstraintName("FK__Pharmacien__CIN__398D8EEE");
        });

        modelBuilder.Entity<Produit>(entity =>
        {
            entity.HasKey(e => e.Reference).HasName("PK__Produit__FD90DA98B99205C2");

            entity.ToTable("Produit");

            entity.Property(e => e.Reference).HasColumnName("reference");
            entity.Property(e => e.DescriptionP)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description_p");
            entity.Property(e => e.Libelle)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("libelle");
            entity.Property(e => e.Prix).HasColumnName("prix");
        });

        modelBuilder.Entity<ProgFidelite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProgFide__3213E83F024DDDBA");

            entity.ToTable("ProgFidelite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.Remise).HasColumnName("remise");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.ProgFidelites)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProgFidel__id_cl__4E88ABD4");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock).HasName("PK__Stock__3A39590ACE68D373");

            entity.ToTable("Stock");

            entity.Property(e => e.IdStock).HasColumnName("id_stock");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.Reference).HasColumnName("reference");

            entity.HasOne(d => d.ReferenceNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.Reference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stock__quantite__440B1D61");
        });

        modelBuilder.Entity<Vente>(entity =>
        {
            entity.HasKey(e => e.IdVente).HasName("PK__Vente__459533B36DC72F93");

            entity.ToTable("Vente");

            entity.Property(e => e.IdVente).HasColumnName("id_vente");
            entity.Property(e => e.DateVente).HasColumnName("date_vente");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.Quantite).HasColumnName("quantite");
            entity.Property(e => e.Reference).HasColumnName("reference");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Ventes)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vente__id_client__4BAC3F29");

            entity.HasOne(d => d.ReferenceNavigation).WithMany(p => p.Ventes)
                .HasForeignKey(d => d.Reference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vente__reference__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
