using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "AuthenticatedOnly")]
    public class BidListController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;

        public BidListController(IBidRepository bidRepository, ILogger<BidListController> logger)
        {
            _bidRepository = bidRepository;
        }


        // Récupération de tous les Bid
        [HttpGet]
        public async Task<IActionResult> GetAllBids()
        {
            try
            {
                var bids = await _bidRepository.GetAllBidsAsync();
                if (bids == null || !bids.Any())
                {
                    return NotFound();
                }
                return Ok(bids);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving all Bids");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        [Route("validate")]
        public IActionResult Validate([FromBody] BidList bidList)
        {
            // TODO: check data valid and save to db, after saving return bid list
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        [Route("update/{id}")]
        public IActionResult UpdateBid(int id, [FromBody] BidList bidList)
        {
            // TODO: check required fields, if valid call service to update Bid and return list Bid
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "User, Admin")]
        [Route("{id}")]
        public IActionResult DeleteBid(int id)
        {
            return Ok();
        }
    }
}