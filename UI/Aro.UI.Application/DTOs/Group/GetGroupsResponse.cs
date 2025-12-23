namespace Aro.UI.Application.DTOs.Group;
public record GetGroupsResponse(
    List<GroupDto> Groups,
    int TotalCount
);

