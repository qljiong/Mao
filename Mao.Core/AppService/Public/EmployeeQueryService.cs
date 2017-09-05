using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mao.Models;
using Mao.Infrastructure.DB.Base;
using Mao.Core.Util;

namespace Mao.Core.AppService.Public
{
    public class EmployeeQueryService : IEmployeeQueryService
    {
        private readonly IDataSetFactory dsf = null;

        public EmployeeQueryService()
        {
            dsf = EntityFactoryUtil.DsfPub;
        }

        public List<Employee> GetList()
        {
            throw new NotImplementedException();
        }
    }
}
