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
        /// R�cup�ration de tous les CurvePoints       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            _logger.LogInformation("Tentative de r�cup�ration de tous les CurvePoints.");

            try
            {
                var curvePoints = await _curvePointRepository.GetAllCurvePointsAsync();
                if (curvePoints == null || !curvePoints.Any())
                {
                    _logger.LogWarning("Aucun CurvePoint trouv�.");
                    return Ok(new List<CurvePoint>());
                }

                _logger.LogInformation("{CurvePointCount} CurvePoints r�cup�r�s avec succ�s.", curvePoints.Count());
                return Ok(curvePoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration de tous les CurvePoints.");
                return StatusCode(500, "Une erreur est survenue lors de la r�cup�ration de tous les CurvePoints.");
            }
        }

        /// <summary>
        /// R�cup�ration d'un CurvePoint par son Id
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetCurvePointById(int id)
        {
            _logger.LogInformation("Tentative de r�cup�ration du CurvePoint avec ID {CurvePointId}.", id);

            try
            {
                var curvePoint = await _curvePointRepository.GetCurvePointByIdAsync(id);
                if (curvePoint == null)
                {
                    _logger.LogWarning("Aucun CurvePoint trouv� avec l'ID {CurvePointId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} r�cup�r� avec succ�s.", id);
                return Ok(curvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration du CurvePoint avec ID {CurvePointId}.", id);
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
            _logger.LogInformation("Tentative de cr�ation d'un nouveau CurvePoint.");

            if (curvePoint == null)
            {
                _logger.LogWarning("Objet CurvePoint nul fourni dans la requ�te.");
                return BadRequest("Objet CurvePoint nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la cr�ation d'un CurvePoint.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdCurvePoint = await _curvePointRepository.CreateCurvePointAsync(curvePoint);
                _logger.LogInformation("CurvePoint cr�� avec succ�s avec l'ID {CurvePointId}.", createdCurvePoint.Id);
                return CreatedAtAction(nameof(GetCurvePointById), new { id = createdCurvePoint.Id }, createdCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la cr�ation du CurvePoint.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un CurvePoint � jour
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCurvePoint(int id, [FromBody] CurvePoint curvePoint)
        {
            _logger.LogInformation("Tentative de mise � jour du CurvePoint avec ID {CurvePointId}.", id);

            if (curvePoint == null || curvePoint.Id != id)
            {
                _logger.LogWarning("Objet CurvePoint nul ou incompatibilit� d'ID pour la mise � jour du CurvePoint avec ID {CurvePointId}.", id);
                return BadRequest("Objet CurvePoint nul ou incompatibilit� d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise � jour du CurvePoint avec ID {CurvePointId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCurvePoint = await _curvePointRepository.UpdateCurvePointAsync(curvePoint);
                if (updatedCurvePoint == null)
                {
                    _logger.LogWarning("Aucun CurvePoint trouv� avec l'ID {CurvePointId} pour la mise � jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} mis � jour avec succ�s.", id);
                return Ok(updatedCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise � jour du CurvePoint avec ID {CurvePointId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise � jour du CurvePoint");
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
                    _logger.LogWarning("Aucun CurvePoint trouv� avec l'ID {CurvePointId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("CurvePoint avec ID {CurvePointId} supprim� avec succ�s.", id);
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
