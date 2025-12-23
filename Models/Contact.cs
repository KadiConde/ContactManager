using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Contact
    {
        public int ContactID { get; set; } //PK

        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Province { get; set; }
        public string CodePostal { get; set; }

        [Phone]
        public string Telephone { get; set; }


        [EmailAddress]
        public string Courriel { get; set; }

        public DateTime DateCreation { get; set; }

        public int CategorieID { get; set; } // FK


        public Categorie? Categorie { get; set; }
    }
}