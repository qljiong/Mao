using FluentNHibernate.Mapping;
using Mao.Models;

namespace Mao.Infrastructure.Mapping
{
    public class SysUserMap : ClassMap<SysUser>
    {
        public SysUserMap()
        {
            Table("SysUser");
            Id(m => m.Id).GeneratedBy.Guid();
            Map(m => m.UsrName);
            Map(m => m.UsrMail);
            Map(m => m.UsrGender);
        }
    }
}
