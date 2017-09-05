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
        [Obsolete("Xhfoc Old 为二期数据库连接，请修改到对应的数据库连接")]
        public static IEntityFactory EfXhfocOld
        {
            get { return _getEf("xhfoc"); }
        }

        [Obsolete("Xhfoc Old 为二期数据库连接，请修改到对应的数据库连接")]
        public static IDataSetFactory DsfXhfocOld
        {
            get { return _getDsf("xhfoc"); }
        }

        public static IEntityFactory EfMffoc
        {
            get { return _getEf("mffoc"); }
        }
        public static IDataSetFactory DsfMffoc
        {
            get { return _getDsf("mffoc"); }
        }

        public static IEntityFactory EfCap
        {
            get { return _getEf("cap"); }
        }
        public static IDataSetFactory DsfCap
        {
            get { return _getDsf("cap"); }
        }

        public static IEntityFactory EfCms
        {
            get { return _getEf("cms"); }
        }
        public static IDataSetFactory DsfCms
        {
            get { return _getDsf("cms"); }
        }
        public static IEntityFactory EfCws
        {
            get { return _getEf("cws"); }
        }
        public static IDataSetFactory DsfCws
        {
            get { return _getDsf("cws"); }
        }
        public static IEntityFactory EfDsp
        {
            get { return _getEf("dsp"); }
        }
        public static IDataSetFactory DsfDsp
        {
            get { return _getDsf("dsp"); }
        }

        public static IEntityFactory EfQar
        {
            get { return _getEf("qar"); }
        }

        public static IDataSetFactory DsfQar
        {
            get { return _getDsf("qar"); }
        }

        public static IEntityFactory EfFlt
        {
            get { return _getEf("flt"); }
        }
        public static IDataSetFactory DsfFlt
        {
            get { return _getDsf("flt"); }
        }

        public static IEntityFactory EfIfr
        {
            get { return _getEf("ifr"); }
        }
        public static IDataSetFactory DsfIfr
        {
            get { return _getDsf("ifr"); }
        }

        public static IEntityFactory EfLmt
        {
            get { return _getEf("lmt"); }
        }
        public static IDataSetFactory DsfLmt
        {
            get { return _getDsf("lmt"); }
        }

        public static IEntityFactory EfMcc
        {
            get { return _getEf("mcc"); }
        }
        public static IDataSetFactory DsfMcc
        {
            get { return _getDsf("mcc"); }
        }

        public static IEntityFactory EfNav
        {
            get { return _getEf("nav"); }
        }
        public static IDataSetFactory DsfNav
        {
            get { return _getDsf("nav"); }
        }

        public static IEntityFactory EfNavGroup
        {
            get { return _getEf("navgroup"); }
        }
        public static IDataSetFactory DsfNavGroup
        {
            get { return _getDsf("navgroup"); }
        }

        public static IEntityFactory EfPpc
        {
            get { return _getEf("ppc"); }
        }
        public static IDataSetFactory DsfPpc
        {
            get { return _getDsf("ppc"); }
        }

        public static IEntityFactory EfPub
        {
            get { return _getEf("pub"); }
        }
        public static IDataSetFactory DsfPub
        {
            get { return _getDsf("pub"); }
        }

        public static IEntityFactory EfSvc
        {
            get { return _getEf("svc"); }
        }
        public static IDataSetFactory DsfSvc
        {
            get { return _getDsf("svc"); }
        }

        public static IEntityFactory EfWrn
        {
            get { return _getEf("wrn"); }
        }
        public static IDataSetFactory DsfWrn
        {
            get { return _getDsf("wrn"); }
        }

        public static IEntityFactory EfWea
        {
            get { return _getEf("wea"); }
        }
        public static IDataSetFactory DsfWea
        {
            get { return _getDsf("wea"); }
        }

        //wea history data 气象集中数据库
        public static IEntityFactory EfWeaHistory
        {
            get { return _getEf("whd"); }
        }
        public static IDataSetFactory DsfWeaHistory
        {
            get { return _getDsf("whd"); }
        }

        public static IEntityFactory EfTele
        {
            get { return _getEf("tele"); }
        }

        public static IDataSetFactory DsfTele
        {
            get { return _getDsf("tele"); }
        }

        public static IEntityFactory EfTcs
        {
            get { return _getEf("tcs"); }
        }

        public static IDataSetFactory DsfTcs
        {
            get { return _getDsf("tcs"); }
        }

        public static IEntityFactory EfFocWeb
        {
            get { return _getEf("focweb"); }
        }

        public static IDataSetFactory DsfFocWeb
        {
            get { return _getDsf("focweb"); }
        }

        public static IEntityFactory EfAocEme
        {
            get { return _getEf("aoc_eme"); }
        }

        public static IDataSetFactory DsfAocEme
        {
            get { return _getDsf("aoc_eme"); }
        }
        public static IEntityFactory EfAis
        {
            get { return _getEf("ais"); }
        }

        public static IDataSetFactory DsfAis
        {
            get { return _getDsf("ais"); }
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
