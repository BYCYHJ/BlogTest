using System.Text;

namespace SqlHelper
{
    public class BaiSqlHelper
    {
        /// <summary>
        /// 插入单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">单条数据</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static string InsertSql<T>(T data, string tableName) where T : class
        {
            if (data == null || tableName == string.Empty)
            {
                return string.Empty;
            }
            string columns = GetColumns(data);//列名
            if (columns == string.Empty)
            {
                return string.Empty;
            }
            string values = GetColumnValues(data);//值
            if (values == string.Empty)
            {
                return string.Empty;
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO " + tableName);
            sql.Append(" (" + columns + ") ");
            sql.Append("VALUES(" + values + ")");
            return sql.ToString();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datum">数据集</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static string BulkInsertSql<T>(IEnumerable<T> datum, string tableName) where T : class
        {
            if (datum == null || datum.Count() == 0 || tableName == string.Empty)
            {
                return string.Empty;
            }
            string columns = GetColumns(datum.FirstOrDefault());
            if (columns == string.Empty)
            {
                return string.Empty;
            }
            string value = string.Join(",", datum.Select(data =>
            {
                return $"({GetColumnValues(data)}) ";
            }).ToArray());
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO " + tableName);
            sql.Append(" (" + columns + ") ");
            sql.Append("VALUES " + value + "");
            return sql.ToString();
        }

        /// <summary>
        /// 获取所传数据的列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetColumns<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return string.Join(",", obj.GetType().GetProperties().Select(o => o.Name).ToList());
        }

        /// <summary>
        /// 获取每列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetColumnValues<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return string.Join(",", obj.GetType().GetProperties().Select(o =>
            {
                var value = o.GetValue(obj);
                if (value!=null && value!.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)(value!);
                    return "'" + time.ToString("yyyy-MM-dd hh:mm:ss") + "'";
                }
                return $"'{value}'";
            }).ToArray());
        }
    }
}
