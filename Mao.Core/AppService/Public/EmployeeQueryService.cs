﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mao.Models;
using Mao.Infrastructure.Db.Base;
using Mao.Core.Util;
using Mao.Infrastructure.Util;

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
            var getdata = dsf.GetDataSet("select * from SysUser");
            var result = DbUtil.LoadEntities<Employee>(getdata);
            return result;
        }
    }
}
