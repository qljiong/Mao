using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Reflection;

namespace Mao.Infrastructure.Util
{
    public static class DbUtil
    {
        public static List<T> LoadEntities<T>(DataSet ds)
        {
            if (!(ds != null && ds.Tables.Count > 0))
                return null;

            return LoadEntities<T>(ds.Tables[0]);
        }

        public static List<T> LoadEntities<T>(DataTable dt)
        {
            if (dt == null)
                return null;

            var entities = new List<T>();

            var t = typeof(T);
            var ps = t.GetProperties();

            foreach (DataRow row in dt.Rows)
            {
                entities.Add(LoadEntity<T>(row, ps));
            }

            return entities;
        }

        public static T LoadEntity<T>(DataRow row, PropertyInfo[] ps = null)
        {
            if (row == null)
                return default(T);

            if (ps == null)
            {
                var t = typeof(T);
                ps = t.GetProperties();
            }

            var cols = row.Table.Columns;

            var obj = Activator.CreateInstance<T>();
            foreach (var p in ps)
            {
                if (!cols.Contains(p.Name)) continue;
                if (DBNull.Value == row[p.Name]) continue;
                if (p.PropertyType.FullName == "System.Boolean" || p.PropertyType.FullName == typeof(bool?).FullName)
                {
                    var val = DBConvert.ChangeType<string>(row[p.Name]);
                    if (val == "Y" || val == "1" || val.ToLower() == "true")
                    {
                        p.SetValue(obj, true, null);
                    }
                    else if (string.IsNullOrEmpty(val))
                    {
                        try
                        {
                            p.SetValue(obj, null, null);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("将" + p.Name + "转换为null类型失败", e);
                        }
                    }
                    else
                    {
                        p.SetValue(obj, false, null);
                    }
                    continue;
                }
                if (p.PropertyType.FullName == typeof(DateTime).FullName || p.PropertyType.FullName == typeof(DateTime?).FullName)
                {
                    if (row[p.Name] != null && string.IsNullOrEmpty(row[p.Name].ToString()))
                    {
                        p.SetValue(obj, null, null);
                    }
                    else
                    {
                        var val = DBConvert.ChangeType<DateTime>(row[p.Name]);
                        p.SetValue(obj, new DateTime(val.Ticks, DateTimeKind.Local), null);
                    }
                    continue;
                }

                try
                {
                    p.SetValue(obj, DBConvert.ChangeType(row[p.Name], p.PropertyType), null);
                }
                catch (Exception e)
                {
                    throw new Exception("将" + p.Name + "转换为" + p.PropertyType + "类型失败", e);
                }
            }
            return obj;
        }

        public static DataTable ToDataTable<T>(IList<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names
            var oProps = typeof(T).GetProperties();
            foreach (PropertyInfo pi in oProps)
            {
                Type colType = pi.PropertyType;
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
            }
            // Could add a check to verify that there is an element 0
            foreach (var rec in varlist)
            {
                var dr = dtReturn.NewRow();
                foreach (var pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                }
                dtReturn.Rows.Add(dr);
            }
            return (dtReturn);
        }


        public static string NewGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
