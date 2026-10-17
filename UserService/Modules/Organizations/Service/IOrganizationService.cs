using UserService.Modules.Organizations.Dto;

namespace UserService.Modules.Organizations.Service;

public interface IOrganizationService
{
    Task<OrganizationResponse?> GetByIdWithImagesAsync(string id);
    Task<List<OrganizationResponse>> GetTreeWithImagesAsync();
}
