using API.Entities;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
    Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId);
    Task<IReadOnlyList<string>> GetCurrentMemberLikesIds(string memberId);
    void DeleteLike(MemberLike memberLike);
    void AddLike(MemberLike memberLike);
    Task<bool> SaveAllChanges();
}
