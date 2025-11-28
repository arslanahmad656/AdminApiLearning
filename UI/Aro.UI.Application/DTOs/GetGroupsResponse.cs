namespace Aro.UI.Application.DTOs;
public record GetGroupsResponse(
    List<GroupDto> Groups,
    int TotalCount
);

