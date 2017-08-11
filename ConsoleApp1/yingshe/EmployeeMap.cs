using ConsoleApp1.shiti;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.yingshe
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        { 
            //指定id列
            Id(s => s.Id);
            Map(s => s.Name);
            Map(s => s.Age);
        }
    }
}
