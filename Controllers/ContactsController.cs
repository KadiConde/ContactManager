using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;

namespace ContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacts
        // Les paramètres permettent de recevoir les critères de tri, de recherche et le numéro de page
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            // 1. Gestion du Tri (ViewData pour la vue)
            ViewData["CurrentSort"] = sortOrder;
            // Si sortOrder est vide, on trie par nom ascendant, sinon on inverse
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // 2. Gestion du Filtrage (Recherche)
            if (searchString != null)
            {
                pageNumber = 1; // Si on fait une nouvelle recherche, on revient en page 1
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            // 3. Construction de la requête de base
            var contacts = _context.Contacts.Include(c => c.Categorie).AsQueryable();

            // 4. Application du filtre de recherche (Nom ou Prénom)
            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => s.Nom.Contains(searchString)
                                       || s.Prenom.Contains(searchString));
            }

            // 5. Application du Tri
            switch (sortOrder)
            {
                case "name_desc":
                    contacts = contacts.OrderByDescending(s => s.Nom);
                    break;
                case "Date":
                    contacts = contacts.OrderBy(s => s.DateCreation);
                    break;
                case "date_desc":
                    contacts = contacts.OrderByDescending(s => s.DateCreation);
                    break;
                default:
                    contacts = contacts.OrderBy(s => s.Nom);
                    break;
            }

            // 6. Pagination
            int pageSize = 5; // Nombre de contacts par page (vous pouvez changer ce chiffre)

            // On utilise la méthode statique CreateAsync de votre classe PaginatedList
            return View(await PaginatedList<Contact>.CreateAsync(contacts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // --- LES AUTRES MÉTHODES RESTENT IDENTIQUES ---

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Categorie)
                .FirstOrDefaultAsync(m => m.ContactID == id);

            if (contact == null) return NotFound();

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "Nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactID,Prenom,Nom,Adresse,Ville,Province,CodePostal,Telephone,Courriel,DateCreation,CategorieID")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                // Optionnel : Forcer la date de création au moment de l'ajout
                contact.DateCreation = DateTime.Now;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "Nom", contact.CategorieID);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();

            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "Nom", contact.CategorieID);
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactID,Prenom,Nom,Adresse,Ville,Province,CodePostal,Telephone,Courriel,DateCreation,CategorieID")] Contact contact)
        {
            if (id != contact.ContactID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorieID"] = new SelectList(_context.Categories, "CategorieID", "Nom", contact.CategorieID);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Categorie)
                .FirstOrDefaultAsync(m => m.ContactID == id);

            if (contact == null) return NotFound();

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ContactID == id);
        }
    }
}