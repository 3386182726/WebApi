using Common.Models;
using System.Net.Http.Json;
using UserService.Modules.Organizations.Dto;
using UserService.Modules.Organizations.Repository;
using UserService.Modules.Users.Service;

namespace UserService.Modules.Organizations.Service;

public class OrganizationService(
    IOrganizationRepository organizationRepository,
    IUserService userService,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration
) : IOrganizationService
{
    private string FileServiceUrl => configuration["FileService:BaseUrl"] ?? "http://fileservice:8080";

    public async Task<OrganizationResponse?> GetByIdWithImagesAsync(string id)
    {
        var org = await organizationRepository.GetAsync(id);
        if (org == null) return null;

        var images = await QueryFilesAsync(
            AppFileConstants.OwnerTypeOrganization, [org.Id]);

        var users = await userService.GetByOrganizationIdsAsync([org.Id]);

        return new OrganizationResponse
        {
            Id = org.Id,
            Name = org.Name,
            Description = org.Description,
            ParentId = org.ParentId,
            Avatars = images,
            Users = users,
        };
    }

    public async Task<List<OrganizationResponse>> GetTreeWithImagesAsync()
    {
        var orgs = await organizationRepository.GetListAsync();

        var orgIds = orgs.Select(o => o.Id).ToList();

        // 批量查组织头像
        var allImages = await QueryFilesAsync(
            AppFileConstants.OwnerTypeOrganization, orgIds);
        var imageGrouped = allImages.GroupBy(f => f.OwnerId);

        // 批量查人员
        var allUsers = await userService.GetByOrganizationIdsAsync(orgIds);
        var userGrouped = allUsers.GroupBy(u => u.OrganizationId!);

        var dtos = orgs.Select(org =>
        {
            var images = imageGrouped.FirstOrDefault(g => g.Key == org.Id)?.ToList() ?? [];
            var users = userGrouped.FirstOrDefault(g => g.Key == org.Id)?.ToList() ?? [];
            return new OrganizationResponse
            {
                Id = org.Id,
                Name = org.Name,
                Description = org.Description,
                ParentId = org.ParentId,
                Avatars = images,
                Users = users,
            };
        }).ToList();

        // 构建树
        var lookup = dtos.ToDictionary(d => d.Id);
        List<OrganizationResponse> roots = [];
        foreach (var dto in lookup.Values)
        {
            if (!string.IsNullOrEmpty(dto.ParentId) && lookup.ContainsKey(dto.ParentId))
                lookup[dto.ParentId].Children.Add(dto);
            else
                roots.Add(dto);
        }
        return roots;
    }

    private async Task<List<AppFileResponse>> QueryFilesAsync(string ownerType, List<string> ownerIds)
    {
        if (ownerIds.Count == 0) return [];

        using var httpClient = httpClientFactory.CreateClient();

        var query = new UserService.Modules.Users.Repository.FileQueryRequest
        {
            OwnerType = ownerType,
            OwnerIds = ownerIds,
            Category = AppFileConstants.CategoryImage,
        };

        var response = await httpClient.PostAsJsonAsync($"{FileServiceUrl}/api/file/query", query);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<AppFileResponse>>() ?? [];
    }
}
