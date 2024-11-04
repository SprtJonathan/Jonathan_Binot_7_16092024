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
        /// R�cup�ration de tous les Trades       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            _logger.LogInformation("Tentative de r�cup�ration de tous les Trades.");

            try
            {
                var trades = await _tradeRepository.GetAllTradesAsync();
                if (trades == null || !trades.Any())
                {
                    _logger.LogWarning("Aucun Trade trouv�.");
                    return NotFound();
                }

                _logger.LogInformation("{TradeCount} Trades r�cup�r�s avec succ�s.", trades.Count());
                return Ok(trades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration de tous les Trades.");
                return StatusCode(500, "Une erreur est survenue lors de la r�cup�ration de tous les Trades.");
            }
        }

        /// <summary>
        /// R�cup�ration d'un Trade par son Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(int id)
        {
            _logger.LogInformation("Tentative de r�cup�ration du Trade avec ID {TradeId}.", id);

            try
            {
                var trade = await _tradeRepository.GetTradeByIdAsync(id);
                if (trade == null)
                {
                    _logger.LogWarning("Aucun Trade trouv� avec l'ID {TradeId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} r�cup�r� avec succ�s.", id);
                return Ok(trade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration du Trade avec ID {TradeId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un Trade
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTrade([FromBody] Trade trade)
        {
            _logger.LogInformation("Tentative de cr�ation d'un nouveau Trade.");

            if (trade == null)
            {
                _logger.LogWarning("Objet Trade nul fourni dans la requ�te.");
                return BadRequest("Objet Trade nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la cr�ation d'un Trade.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdTrade = await _tradeRepository.CreateTradeAsync(trade);
                _logger.LogInformation("Trade cr�� avec succ�s avec l'ID {TradeId}.", createdTrade.TradeId);
                return CreatedAtAction(nameof(GetTradeById), new { id = createdTrade.TradeId }, createdTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la cr�ation du Trade.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un Trade � jour
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] Trade trade)
        {
            _logger.LogInformation("Tentative de mise � jour du Trade avec ID {TradeId}.", id);

            if (trade == null || trade.TradeId != id)
            {
                _logger.LogWarning("Objet Trade nul ou incompatibilit� d'ID pour la mise � jour du Trade avec ID {TradeId}.", id);
                return BadRequest("Objet Trade nul ou incompatibilit� d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise � jour du Trade avec ID {TradeId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTrade = await _tradeRepository.UpdateTradeAsync(trade);
                if (updatedTrade == null)
                {
                    _logger.LogWarning("Aucun Trade trouv� avec l'ID {TradeId} pour la mise � jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} mis � jour avec succ�s.", id);
                return Ok(updatedTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise � jour du Trade avec ID {TradeId}.", id);
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
                    _logger.LogWarning("Aucun Trade trouv� avec l'ID {TradeId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("Trade avec ID {TradeId} supprim� avec succ�s.", id);
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
