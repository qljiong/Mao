using Mao.Infrastructure.DB.Base;
using Mao.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Core.Util
{
    /// <summary>
    /// 实体工厂与数据集工厂Util; 
    /// 实体工厂命名: Ef{DBUser}; 
    /// 数据集工厂命名: Dsf{DBUser}; 
    /// </summary>
    public static class EntityFactoryUtil
    {
        public static IEntityFactory EfPub
        {
            get { return _getEf("pub"); }
        }
        public static IDataSetFactory DsfPub
        {
            get { return _getDsf("pub"); }
        }

        private static IEntityFactory _getEf(string dbName)
        {
            return TxObjectPool.GetEf(dbName);
        }

        private static IDataSetFactory _getDsf(string dbName)
        {
            return TxObjectPool.GetDsf(dbName);
        }
    }
}
