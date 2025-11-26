namespace Aro.Admin.Presentation.UI.Models;
public record GetGroupsResponse(
    List<GroupDto> Groups,
    int TotalCount
);

