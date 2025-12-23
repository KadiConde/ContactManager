using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class Categorie
    {
        public int CategorieID { get; set; } //PK

        [Required]
        public string Nom { get; set; }

        public ICollection<Contact>? Contacts { get; set; }
    }
}