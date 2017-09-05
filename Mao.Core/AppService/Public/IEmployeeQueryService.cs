using Mao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Core.AppService.Public
{
    public interface IEmployeeQueryService
    {
        List<Employee> GetList();
    }
}
