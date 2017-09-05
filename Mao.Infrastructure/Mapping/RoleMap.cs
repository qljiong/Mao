using FluentNHibernate.Mapping;
using Mao.Models;

namespace Mao.Infrastructure.Mapping
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Table("Role");
            Id(m => m.Id).GeneratedBy.Guid();
            Map(m => m.IsRead);
            Map(m => m.IsWrite);
            Map(m => m.RoleName);
        }
    }
}
