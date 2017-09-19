using Mao.Infrastructure.Db.Base;
using Mao.Infrastructure.Db.Impl;

namespace Xal.Op.Foc3.Infrastructure.Db.Impl
{
    public class DataSetFactoryMsSql : AbstractDataSetFactory
    {
        private readonly AbstractDbUtil _dbUtil;

        public DataSetFactoryMsSql()
        {
            _dbUtil = new DbUtilMsSql();
        }

        public override AbstractDbUtil DbUtil
        {
            get { return _dbUtil; }
        }
    }
}
