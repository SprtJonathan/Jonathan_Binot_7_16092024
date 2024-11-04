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
        /// Récupération de tous les RuleNames       
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRuleNames()
        {
            _logger.LogInformation("Tentative de récupération de tous les RuleNames.");

            try
            {
                var ruleNames = await _ruleNameRepository.GetAllRuleNamesAsync();
                if (ruleNames == null || !ruleNames.Any())
                {
                    _logger.LogWarning("Aucun RuleName trouvé.");
                    return NotFound();
                }

                _logger.LogInformation("{RuleNameCount} RuleNames récupérés avec succès.", ruleNames.Count());
                return Ok(ruleNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération de tous les RuleNames.");
                return StatusCode(500, "Une erreur est survenue lors de la récupération de tous les RuleNames.");
            }
        }

        /// <summary>
        /// Récupération d'un RuleName par son Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleNameById(int id)
        {
            _logger.LogInformation("Tentative de récupération du RuleName avec ID {RuleNameId}.", id);

            try
            {
                var ruleName = await _ruleNameRepository.GetRuleNameByIdAsync(id);
                if (ruleName == null)
                {
                    _logger.LogWarning("Aucun RuleName trouvé avec l'ID {RuleNameId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} récupéré avec succès.", id);
                return Ok(ruleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération du RuleName avec ID {RuleNameId}.", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Ajout d'un RuleName
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRuleName([FromBody] RuleName ruleName)
        {
            _logger.LogInformation("Tentative de création d'un nouveau RuleName.");

            if (ruleName == null)
            {
                _logger.LogWarning("Objet RuleName nul fourni dans la requête.");
                return BadRequest("Objet RuleName nul");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la création d'un RuleName.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRuleName = await _ruleNameRepository.CreateRuleNameAsync(ruleName);
                _logger.LogInformation("RuleName créé avec succès avec l'ID {RuleNameId}.", createdRuleName.Id);
                return CreatedAtAction(nameof(GetRuleNameById), new { id = createdRuleName.Id }, createdRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la création du RuleName.");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre un RuleName à jour
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleName ruleName)
        {
            _logger.LogInformation("Tentative de mise à jour du RuleName avec ID {RuleNameId}.", id);

            if (ruleName == null || ruleName.Id != id)
            {
                _logger.LogWarning("Objet RuleName nul ou incompatibilité d'ID pour la mise à jour du RuleName avec ID {RuleNameId}.", id);
                return BadRequest("Objet RuleName nul ou incompatibilité d'ID");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState non valide pour la mise à jour du RuleName avec ID {RuleNameId}.", id);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRuleName = await _ruleNameRepository.UpdateRuleNameAsync(ruleName);
                if (updatedRuleName == null)
                {
                    _logger.LogWarning("Aucun RuleName trouvé avec l'ID {RuleNameId} pour la mise à jour.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} mis à jour avec succès.", id);
                return Ok(updatedRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la mise à jour du RuleName avec ID {RuleNameId}.", id);
                return StatusCode(500, "Une erreur est survenue lors de la mise à jour du RuleName");
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
                    _logger.LogWarning("Aucun RuleName trouvé avec l'ID {RuleNameId} pour la suppression.", id);
                    return NotFound();
                }

                _logger.LogInformation("RuleName avec ID {RuleNameId} supprimé avec succès.", id);
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
