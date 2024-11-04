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
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleRepository _ruleNameRepository;
        private readonly ILogger<RuleNameController> _logger;

        public RuleNameController(IRuleRepository ruleNameRepository, ILogger<RuleNameController> logger)
        {
            _ruleNameRepository = ruleNameRepository;
            _logger = logger;
        }

        /// <summary>
        /// R�cup�ration de tous les RuleNames       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRuleNames()
        {
            _logger.LogInformation("Tentative de r�cup�ration de tous les RuleNames.");

            try
            {
                var ruleNames = await _ruleNameRepository.GetAllRuleNamesAsync();
                if (ruleNames == null || !ruleNames.Any())
                {
                    _logger.LogWarning("Aucun RuleName trouv�.");
                    return NotFound();
                }

                _logger.LogInformation("{RuleNameCount} RuleNames r�cup�r�s avec succ�s.", ruleNames.Count());
                return Ok(ruleNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration de tous les RuleNames.");
                return StatusCode(500, "Une erreur est survenue lors de la r�cup�ration de tous les RuleNames.");
            }
        }

        /// <summary>
        /// R�cup�ration d'un RuleName par son Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleNameById(int id)
        {
            _logger.LogInformation("Tentative de r�cup�ration du RuleName avec ID {RuleNameId}.", id);

            try
            {
                var ruleName = await _ruleNameRepository.GetRuleNameByIdAsync(id);
                if (ruleName == null)
                {
                    _logger.LogWarning("Aucun RuleName trouv� avec l'ID {RuleNameId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} r�cup�r� avec succ�s.", id);
                return Ok(ruleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la r�cup�ration du RuleName avec ID {RuleNameId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un RuleName
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRuleName([FromBody] RuleName ruleName)
        {
            _logger.LogInformation("Tentative de cr�ation d'un nouveau RuleName.");

            if (ruleName == null)
            {
                _logger.LogWarning("Objet RuleName nul fourni dans la requ�te.");
                return BadRequest("Objet RuleName nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la cr�ation d'un RuleName.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRuleName = await _ruleNameRepository.CreateRuleNameAsync(ruleName);
                _logger.LogInformation("RuleName cr�� avec succ�s avec l'ID {RuleNameId}.", createdRuleName.Id);
                return CreatedAtAction(nameof(GetRuleNameById), new { id = createdRuleName.Id }, createdRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la cr�ation du RuleName.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un RuleName � jour
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleName ruleName)
        {
            _logger.LogInformation("Tentative de mise � jour du RuleName avec ID {RuleNameId}.", id);

            if (ruleName == null || ruleName.Id != id)
            {
                _logger.LogWarning("Objet RuleName nul ou incompatibilit� d'ID pour la mise � jour du RuleName avec ID {RuleNameId}.", id);
                return BadRequest("Objet RuleName nul ou incompatibilit� d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise � jour du RuleName avec ID {RuleNameId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRuleName = await _ruleNameRepository.UpdateRuleNameAsync(ruleName);
                if (updatedRuleName == null)
                {
                    _logger.LogWarning("Aucun RuleName trouv� avec l'ID {RuleNameId} pour la mise � jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} mis � jour avec succ�s.", id);
                return Ok(updatedRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise � jour du RuleName avec ID {RuleNameId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise � jour du RuleName");
            }
        }

        /// <summary>
        /// Supprimer un RuleName
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            _logger.LogInformation("Tentative de suppression du RuleName avec ID {RuleNameId}.", id);

            try
            {
                var result = await _ruleNameRepository.DeleteRuleNameAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Aucun RuleName trouv� avec l'ID {RuleNameId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} supprim� avec succ�s.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la suppression du RuleName avec ID {RuleNameId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
