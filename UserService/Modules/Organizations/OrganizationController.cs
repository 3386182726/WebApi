using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Modules.Organizations.Dto;
using UserService.Modules.Organizations.Repository;
using UserService.Modules.Organizations.Service;

namespace UserService.Modules.Organizations
{
    [Route("api/user/organization")]
    [ApiController]
    [Authorize]
    public class OrganizationController(
        IOrganizationRepository organizationRepository,
        IOrganizationService organizationService
    ) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var organizations = await organizationService.GetTreeWithImagesAsync();
            return Ok(organizations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var org = await organizationService.GetByIdWithImagesAsync(id);
            return org == null ? NotFound("组织不存在") : Ok(org);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(OrganizationRequest dto)
        {
            if (!string.IsNullOrEmpty(dto.Id))
            {
                var organization = await organizationRepository.GetAsync(dto.Id);
                if (organization == null)
                    return NotFound("组织不存在");
                organization.Name = dto.Name;
                organization.Description = dto.Description;
                organization.ParentId = dto.ParentId;
                organizationRepository.Update(organization);
            }
            else
            {
                var organization = new Organization()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ParentId = dto.ParentId,
                };
                organizationRepository.Create(organization);
            }
            await organizationRepository.SaveChangesAsync();
            return Ok("保存成功");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var organization = await organizationRepository.GetAsync(id);
            if (organization == null)
                return NotFound("组织不存在");

            organizationRepository.Delete(organization);
            await organizationRepository.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}
