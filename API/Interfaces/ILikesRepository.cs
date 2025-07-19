using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
    Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams);
    Task<IReadOnlyList<string>> GetCurrentMemberLikesIds(string memberId);
    void DeleteLike(MemberLike memberLike);
    void AddLike(MemberLike memberLike);
    Task<bool> SaveAllChanges();
}
