using Mao.Infrastructure.Util;
using Mao.Models;
using NHibernate;
using System;

namespace Mao.Core
{
    public class EmployeeData
    {
        /// <summary>
        /// 添加雇员对象
        /// </summary>
        /// <param name="Employee"></param>
        /// <returns></returns>
        public bool AddEmployee(Employee employee)
        {
            ISession session = FluentNhibernateHelper.GetSession();
            using (var trans = session.BeginTransaction())
            {
                try
                {
                    session.SaveOrUpdate(employee);
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return false;
                }
            }

        }
    }
}
