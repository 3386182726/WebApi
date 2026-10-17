namespace UserService.Modules.Organizations.Dto;

public class OrganizationRequest
{
    public string? Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ParentId { get; set; }
}
