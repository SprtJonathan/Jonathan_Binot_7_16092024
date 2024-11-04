using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly ICurvePointRepository _curvePointRepository;
        private readonly ILogger<CurveController> _logger;

        public CurveController(ICurvePointRepository curvePointRepository, ILogger<CurveController> logger)
        {
            _curvePointRepository = curvePointRepository;
            _logger = logger;
        }

        /// <summary>
        /// Récupération de tous les CurvePoints       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            _logger.LogInformation("Tentative de récupération de tous les CurvePoints.");

            try
            {
                var curvePoints = await _curvePointRepository.GetAllCurvePointsAsync();
                if (curvePoints == null || !curvePoints.Any())
                {
                    _logger.LogWarning("Aucun CurvePoint trouvé.");
                    return Ok(new List<CurvePoint>());
                }

                _logger.LogInformation("{CurvePointCount} CurvePoints récupérés avec succès.", curvePoints.Count());
                return Ok(curvePoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération de tous les CurvePoints.");
                return StatusCode(500, "Une erreur est survenue lors de la récupération de tous les CurvePoints.");
            }
        }

        /// <summary>
        /// Récupération d'un CurvePoint par son Id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetCurvePointById(int id)
        {
            _logger.LogInformation("Tentative de récupération du CurvePoint avec ID {CurvePointId}.", id);

            try
            {
                var curvePoint = await _curvePointRepository.GetCurvePointByIdAsync(id);
                if (curvePoint == null)
                {
                    _logger.LogWarning("Aucun CurvePoint trouvé avec l'ID {CurvePointId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} récupéré avec succès.", id);
                return Ok(curvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération du CurvePoint avec ID {CurvePointId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un CurvePoint
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CreateCurvePoint([FromBody] CurvePoint curvePoint)
        {
            _logger.LogInformation("Tentative de création d'un nouveau CurvePoint.");

            if (curvePoint == null)
            {
                _logger.LogWarning("Objet CurvePoint nul fourni dans la requête.");
                return BadRequest("Objet CurvePoint nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la création d'un CurvePoint.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdCurvePoint = await _curvePointRepository.CreateCurvePointAsync(curvePoint);
                _logger.LogInformation("CurvePoint créé avec succès avec l'ID {CurvePointId}.", createdCurvePoint.Id);
                return CreatedAtAction(nameof(GetCurvePointById), new { id = createdCurvePoint.Id }, createdCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la création du CurvePoint.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un CurvePoint à jour
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCurvePoint(int id, [FromBody] CurvePoint curvePoint)
        {
            _logger.LogInformation("Tentative de mise à jour du CurvePoint avec ID {CurvePointId}.", id);

            if (curvePoint == null || curvePoint.Id != id)
            {
                _logger.LogWarning("Objet CurvePoint nul ou incompatibilité d'ID pour la mise à jour du CurvePoint avec ID {CurvePointId}.", id);
                return BadRequest("Objet CurvePoint nul ou incompatibilité d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise à jour du CurvePoint avec ID {CurvePointId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCurvePoint = await _curvePointRepository.UpdateCurvePointAsync(curvePoint);
                if (updatedCurvePoint == null)
                {
                    _logger.LogWarning("Aucun CurvePoint trouvé avec l'ID {CurvePointId} pour la mise à jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} mis à jour avec succès.", id);
                return Ok(updatedCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise à jour du CurvePoint avec ID {CurvePointId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour du CurvePoint");
            }
        }

        /// <summary>
        /// Supprimer un CurvePoint
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            _logger.LogInformation("Tentative de suppression du CurvePoint avec ID {CurvePointId}.", id);

            try
            {
                var result = await _curvePointRepository.DeleteCurvePointAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Aucun CurvePoint trouvé avec l'ID {CurvePointId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} supprimé avec succès.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la suppression du CurvePoint avec ID {CurvePointId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
