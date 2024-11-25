using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;
        private readonly ILogger<BidListController> _logger;

        public BidListController(IBidRepository bidRepository, ILogger<BidListController> logger)
        {
            _bidRepository = bidRepository;
            _logger = logger;
        }

        /// <summary>
        /// Récupération de tous les Bids       
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> GetAllBids()
        {
            _logger.LogInformation("Tentative de récupération de tous les Bids.");

            try
            {
                var bids = await _bidRepository.GetAllBidsAsync();
                if (bids == null || !bids.Any())
                {
                    _logger.LogWarning("Aucun Bid trouvé.");
                    return Ok(new List<BidList>());
                }

                _logger.LogInformation("{BidCount} Bids récupérés avec succès.", bids.Count());
                return Ok(bids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération de tous les Bids.");
                return StatusCode(500, "Une erreur est survenue lors de la récupération de tous les Bids.");
            }
        }

        /// <summary>
        /// Récupération d'un Bid par son Id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> GetBidById(int id)
        {
            _logger.LogInformation("Tentative de récupération du Bid avec ID {BidId}.", id);

            try
            {
                var bid = await _bidRepository.GetBidByIdAsync(id);
                if (bid == null)
                {
                    _logger.LogWarning("Aucun Bid trouvé avec l'ID {BidId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("Bid avec ID {BidId} récupéré avec succès.", id);
                return Ok(bid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération du Bid avec ID {BidId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un Bid
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> CreateBid([FromBody] BidList bid)
        {
            _logger.LogInformation("Tentative de création d'un nouveau Bid.");

            if (bid == null)
            {
                _logger.LogWarning("Objet Bid nul fourni dans la requête.");
                return BadRequest("Objet Bid nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la création d'un Bid.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdBid = await _bidRepository.CreateBidAsync(bid);
                _logger.LogInformation("Bid créé avec l'ID {BidListId}.", createdBid.BidListId);
                return CreatedAtAction(nameof(GetBidById), new { id = createdBid.BidListId }, createdBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la création du Bid.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un Bid à jour
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidList bid)
        {
            _logger.LogInformation("Tentative de mise à jour du Bid avec ID {BidListId}.", id);

            if (bid == null || bid.BidListId != id)
            {
                _logger.LogWarning("Objet Bid nul ou ID de Bid invalide pour la mise à jour du Bid. Bid.Id : {BidListId}.", id);
                return BadRequest("Objet Bid nul ou ID de Bid invalide");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise à jour du Bid avec l'ID {BidListId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedBid = await _bidRepository.UpdateBidAsync(bid);
                if (updatedBid == null)
                {
                    _logger.LogWarning("Aucun Bid trouvé avec l'ID {BidListId} pour la mise à jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("Bid avec ID {BidListId} mis à jour avec succès.", id);
                return Ok(updatedBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise à jour du Bid avec ID {BidListId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour du Bid");
            }
        }

        /// <summary>
        /// Supprimer un Bid
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            _logger.LogInformation("Tentative de suppression du Bid avec ID {BidListId}.", id);

            try
            {
                var result = await _bidRepository.DeleteBidAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Aucun Bid trouvé avec l'ID {BidListId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("Bid avec ID {BidListId} supprimé avec succès.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la suppression du Bid avec ID {BidListId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
