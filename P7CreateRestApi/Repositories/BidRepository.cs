using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace P7CreateRestApi.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly LocalDbContext _context;

        public BidRepository(LocalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crée une nouvelle offre en l'ajoutant à la base de données.
        /// </summary>
        /// <param name="bid">L'objet <see cref="BidList"/> représentant l'offre à ajouter.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre ajoutée comme résultat.</returns>
        public async Task<BidList> CreateBidAsync(BidList bid)
        {
            // Ajouter l'offre à la collection Bids du contexte.
            _context.BidLists.Add(bid);
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner l'offre ajoutée.
            return bid;
        }

        /// <summary>
        /// Récupère une offre spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'offre à récupérer.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre trouvée comme résultat.</returns>
        public async Task<BidList> GetBidByIdAsync(int id)
        {
            // Rechercher l'offre par son identifiant.
            return await _context.BidLists.FindAsync(id);
        }

        /// <summary>
        /// Récupère toutes les offres disponibles.
        /// </summary>
        /// <returns>La tâche représentant l'opération asynchrone, avec une collection des offres comme résultat.</returns>
        public async Task<IEnumerable<BidList>> GetAllBidsAsync()
        {
            // Récupérer toutes les offres sous forme de liste.
            return await _context.BidLists.ToListAsync();
        }

        /// <summary>
        /// Met à jour une offre existante dans la base de données.
        /// </summary>
        /// <param name="bid">L'objet <see cref="BidList"/> contenant les données mises à jour de l'offre.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre mise à jour comme résultat.</returns>
        public async Task<BidList> UpdateBidAsync(BidList bid)
        {
            // Modifier l'état de l'entité pour indiquer qu'elle est en mode de mise à jour.
            _context.BidLists.Update(bid);
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner l'offre mise à jour.
            return bid;
        }

        /// <summary>
        /// Supprime une offre de la base de données par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'offre à supprimer.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec un booléen indiquant si la suppression a réussi.</returns>
        public async Task<bool> DeleteBidAsync(int id)
        {
            // Rechercher l'offre à supprimer.
            var bid = await _context.BidLists.FindAsync(id);
            // Vérifier si l'offre existe.
            if (bid == null) return false;

            // Supprimer l'offre de la collection Bids du contexte.
            _context.BidLists.Remove(bid);
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner vrai pour indiquer que la suppression a réussi.
            return true;
        }
    }
}
