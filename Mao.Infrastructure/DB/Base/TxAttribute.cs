using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.DB.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TxAttribute : Attribute
    {
        /// <summary>
        /// 标记事务管理
        /// </summary>
        /// <param name="dbName">数据库名:dsp or svc ...</param>
        public TxAttribute(string dbName)
        {
            this.DbName = dbName;
        }

        /// <summary>
        /// 标记事务管理,多个实现类
        /// </summary>
        /// <param name="dbName">数据库名:dsp or svc ...</param>
        /// <param name="objName">唯一id</param>
        public TxAttribute(string dbName, string objName)
            : this(dbName)
        {
            IocObjectName = objName;
        }


        public string DbName { get; private set; }

        public string IocObjectName { get; private set; }
    }
}
