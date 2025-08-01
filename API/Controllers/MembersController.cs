using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<Member>>> GetMembers([FromQuery] MemberParams memberParams)
    {
        memberParams.CurrentMemberId = User.GetMemberId();

        return Ok(await memberRepository.GetMembersAsync(memberParams));
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

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm] IFormFile file)
    {
        var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

        if (member is null) return BadRequest("Can not update member");

        var result = await photoService.UploadPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            MemberId = User.GetMemberId()
        };

        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        member.Photos.Add(photo);

        if (await memberRepository.SaveAllAsync()) return photo;

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

        if (member is null) return BadRequest("Can not get member from token");

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

        if (member.ImageUrl == photo?.Url || photo is null) return BadRequest("Can not set this as main photo");

        member.ImageUrl = photo.Url;
        member.User.ImageUrl = photo.Url;

        if (await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

        if (member is null) return BadRequest("Can not get member from token");

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

        if (photo?.Url == member.ImageUrl || photo is null) return BadRequest("Can not delete main photo");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        member.Photos.Remove(photo);

        if (await memberRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }
}

