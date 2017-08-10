using FluentNHibernate.Mapping;
using Mao.Models;

namespace Mao.Infrastructure.Mapping
{
    public class EmployeeMapping : ClassMap<Employee>
    {
        public EmployeeMapping()
        {
            Table("Employee");
            //创建GUID写法
            //Id<Guid>("Id").GeneratedBy.Guid();

            Id(m => m.Id).Column("Id");
            Map(m => m.FirstName).Column("FirstName");
            Map(m => m.lastName).Column("lastName");
            Map(m => m.Store).Column("Store");


        }
    }
}
