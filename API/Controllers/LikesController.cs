using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.GetMemberId();

            if (sourceMemberId == targetMemberId) return BadRequest("You can not like yourself");

            var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };

                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }
            if (await likesRepository.SaveAllChanges()) return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikesIds()
        {
            return Ok(await likesRepository.GetCurrentMemberLikesIds(User.GetMemberId()));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes(string predicate)
        {
            var members = await likesRepository.GetMemberLikes(predicate, User.GetMemberId());

            return Ok(members);
        }
    }
}

