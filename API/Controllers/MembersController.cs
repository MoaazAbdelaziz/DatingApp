using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
    {
        var members = await memberRepository.GetMembersAsync();

        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var member = await memberRepository.GetMemberByIdAsync(id);

        if (member is null) return NotFound("User not found");

        return member;
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosForMember(string id)
    {
        var photos = await memberRepository.GetPhotosForMemberAsync(id);

        return Ok(photos);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(UpdateMemberDto updateMemberDto)
    {
        var memberId = User.GetMemberId();

        var member = await memberRepository.GetMemberForUpdate(memberId);

        if (member is null) return BadRequest("Could not get member");

        member.DisplayName = updateMemberDto.DisplayName ?? member.DisplayName;
        member.Description = updateMemberDto.Description ?? member.Description;
        member.City = updateMemberDto.City ?? member.City;
        member.Country = updateMemberDto.Country ?? member.Country;

        member.User.DisplayName = updateMemberDto.DisplayName ?? member.User.DisplayName;

        memberRepository.Update(member);

        if (await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update member");
    }
}

