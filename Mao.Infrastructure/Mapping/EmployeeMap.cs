using FluentNHibernate.Mapping;
using Mao.Models;

namespace Mao.Infrastructure.Mapping
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employee");
            //创建GUID写法
            //Id<Guid>("Id").GeneratedBy.Guid();

            Id(m => m.Id).GeneratedBy.Guid();
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.Store);
        }
    }
}
