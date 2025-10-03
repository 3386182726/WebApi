using UserService.Modules.Menus.Dto;
using UserService.Modules.Menus.Model;
using UserService.Modules.Menus.Repository;

namespace UserService.Modules.Menus.Service
{
    public static class MenuMapper
    {
        public static List<MenuResultDto> BuildTree(List<Menu> menus)
        {
            var dtoLookup = menus.ToDictionary(
                m => m.Id,
                m => new MenuResultDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Path = m.Path,
                    ParentId = m.ParentId,
                }
            );

            List<MenuResultDto> roots = new();

            foreach (var dto in dtoLookup.Values)
            {
                if (!string.IsNullOrEmpty(dto.ParentId) && dtoLookup.ContainsKey(dto.ParentId))
                {
                    dtoLookup[dto.ParentId].Children.Add(dto);
                }
                else
                {
                    roots.Add(dto);
                }
            }

            return roots;
        }
    }
}
