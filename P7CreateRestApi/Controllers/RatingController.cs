using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingController> _logger;

        public RatingController(IRatingRepository ratingRepository, ILogger<RatingController> logger)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
        }

        /// <summary>
        /// R�cup�ration de tous les Ratings       
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> GetAllRatings()
        {
            _logger.LogInformation("Tentative de r�cup�ration de tous les Ratings.");

            try
            {
                var ratings = await _ratingRepository.GetAllRatingsAsync();
                if (ratings == null || !ratings.Any())
                {
                    _logger.LogWarning("Aucun Rating trouv�.");
                    return Ok(new List<Rating>());
                }

                _logger.LogInformation("{RatingCount} Ratings r�cup�r�s avec succ�s.", ratings.Count());
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration de tous les Ratings.");
                return StatusCode(500, "Une erreur est survenue lors de la r�cup�ration de tous les Ratings.");
            }
        }

        /// <summary>
        /// R�cup�ration d'un Rating par son Id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            _logger.LogInformation("Tentative de r�cup�ration du Rating avec ID {RatingId}.", id);

            try
            {
                var rating = await _ratingRepository.GetRatingByIdAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning("Aucun Rating trouv� avec l'ID {RatingId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("Rating avec ID {RatingId} r�cup�r� avec succ�s.", id);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration du Rating avec ID {RatingId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un Rating
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> CreateRating([FromBody] Rating rating)
        {
            _logger.LogInformation("Tentative de cr�ation d'un nouveau Rating.");

            if (rating == null)
            {
                _logger.LogWarning("Objet Rating nul fourni dans la requ�te.");
                return BadRequest("Objet Rating nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la cr�ation d'un Rating.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRating = await _ratingRepository.CreateRatingAsync(rating);
                _logger.LogInformation("Rating cr�� avec succ�s avec l'ID {RatingId}.", createdRating.Id);
                return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la cr�ation du Rating.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un Rating � jour
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] Rating rating)
        {
            _logger.LogInformation("Tentative de mise � jour du Rating avec ID {RatingId}.", id);

            if (rating == null || rating.Id != id)
            {
                _logger.LogWarning("Objet Rating nul ou incompatibilit� d'ID pour la mise � jour du Rating avec ID {RatingId}.", id);
                return BadRequest("Objet Rating nul ou incompatibilit� d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise � jour du Rating avec ID {RatingId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRating = await _ratingRepository.UpdateRatingAsync(rating);
                if (updatedRating == null)
                {
                    _logger.LogWarning("Aucun Rating trouv� avec l'ID {RatingId} pour la mise � jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("Rating avec ID {RatingId} mis � jour avec succ�s.", id);
                return Ok(updatedRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise � jour du Rating avec ID {RatingId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise � jour du Rating");
            }
        }

        /// <summary>
        /// Supprimer un Rating
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            _logger.LogInformation("Tentative de suppression du Rating avec ID {RatingId}.", id);

            try
            {
                var result = await _ratingRepository.DeleteRatingAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Aucun Rating trouv� avec l'ID {RatingId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("Rating avec ID {RatingId} supprim� avec succ�s.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la suppression du Rating avec ID {RatingId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
