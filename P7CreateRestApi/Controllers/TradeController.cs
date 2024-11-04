using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly ILogger<TradeController> _logger;

        public TradeController(ITradeRepository tradeRepository, ILogger<TradeController> logger)
        {
            _tradeRepository = tradeRepository;
            _logger = logger;
        }

        /// <summary>
        /// Récupération de tous les Trades       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            _logger.LogInformation("Tentative de récupération de tous les Trades.");

            try
            {
                var trades = await _tradeRepository.GetAllTradesAsync();
                if (trades == null || !trades.Any())
                {
                    _logger.LogWarning("Aucun Trade trouvé.");
                    return NotFound();
                }

                _logger.LogInformation("{TradeCount} Trades récupérés avec succès.", trades.Count());
                return Ok(trades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération de tous les Trades.");
                return StatusCode(500, "Une erreur est survenue lors de la récupération de tous les Trades.");
            }
        }

        /// <summary>
        /// Récupération d'un Trade par son Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(int id)
        {
            _logger.LogInformation("Tentative de récupération du Trade avec ID {TradeId}.", id);

            try
            {
                var trade = await _tradeRepository.GetTradeByIdAsync(id);
                if (trade == null)
                {
                    _logger.LogWarning("Aucun Trade trouvé avec l'ID {TradeId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} récupéré avec succès.", id);
                return Ok(trade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération du Trade avec ID {TradeId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un Trade
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTrade([FromBody] Trade trade)
        {
            _logger.LogInformation("Tentative de création d'un nouveau Trade.");

            if (trade == null)
            {
                _logger.LogWarning("Objet Trade nul fourni dans la requête.");
                return BadRequest("Objet Trade nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la création d'un Trade.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdTrade = await _tradeRepository.CreateTradeAsync(trade);
                _logger.LogInformation("Trade créé avec succès avec l'ID {TradeId}.", createdTrade.TradeId);
                return CreatedAtAction(nameof(GetTradeById), new { id = createdTrade.TradeId }, createdTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la création du Trade.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un Trade à jour
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] Trade trade)
        {
            _logger.LogInformation("Tentative de mise à jour du Trade avec ID {TradeId}.", id);

            if (trade == null || trade.TradeId != id)
            {
                _logger.LogWarning("Objet Trade nul ou incompatibilité d'ID pour la mise à jour du Trade avec ID {TradeId}.", id);
                return BadRequest("Objet Trade nul ou incompatibilité d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise à jour du Trade avec ID {TradeId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTrade = await _tradeRepository.UpdateTradeAsync(trade);
                if (updatedTrade == null)
                {
                    _logger.LogWarning("Aucun Trade trouvé avec l'ID {TradeId} pour la mise à jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} mis à jour avec succès.", id);
                return Ok(updatedTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise à jour du Trade avec ID {TradeId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un Trade
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            _logger.LogInformation("Tentative de suppression du Trade avec ID {TradeId}.", id);

            try
            {
                var result = await _tradeRepository.DeleteTradeAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Aucun Trade trouvé avec l'ID {TradeId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} supprimé avec succès.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la suppression du Trade avec ID {TradeId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
