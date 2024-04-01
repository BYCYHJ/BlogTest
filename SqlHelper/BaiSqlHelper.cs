using System.Reflection;
using System.Text;

namespace SqlHelper
{
    public class BaiSqlHelper
    {
        /// <summary>
        /// 创建表SQL语句
        /// </summary>
        /// <typeparam name="T">要创建的表的实体类</typeparam>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static string CreateTableSql<T>(string tableName) where T : class {
            PropertyInfo[] properties = typeof(T).GetProperties();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("CREATE TABLE " + tableName + " (");
            foreach (PropertyInfo property in properties)
            {
                string typeStr = property.PropertyType.ToString();
                string refectionType = string.Empty;
                switch (typeStr) 
                {
                    //short
                    case "System.Int16":
                        refectionType = "SMALLINT";
                        break;
                    //int
                    case "System.Int32":
                        refectionType = "INTEGER";
                        break;
                    //long
                    case "System.Int64":
                        refectionType = "BIGINT";
                        break;
                    //float
                    case "System.Single":
                        refectionType = "FLOAT";
                        break;
                    //double
                    case "System.Double":
                        refectionType = "DOUBLE";
                        break;
                    //decimal
                    case "System.Decimal":
                        refectionType = "DECIMAL";
                        break;
                    //bool
                    case "System.Boolean":
                        refectionType = "TINYINT";
                        break;
                    //string
                    case "System.String":
                        refectionType = "TEXT";
                        break;
                    //DateTime
                    case "System.DateTime":
                        refectionType = "DATETIME";
                        break;
                    //TimeSpan
                    case "System.TimeSpan":
                        refectionType = "BIGINT";
                        break;
                    //Guid
                    case "System.Guid":
                        refectionType = "CHAR(36)";
                        break;
                    //其他类型
                    default:
                        refectionType = "TEXT";
                        break;
                }
                stringBuilder.Append(property.Name + " " + refectionType);
                if(property.GetCustomAttribute<MyPrimaryKeyAttribute>()  != null)
                {
                    stringBuilder.Append(" PRIMARY KEY");
                }
                if(property == properties.Last()) { break; }
                stringBuilder.Append(",");
            }
            stringBuilder.Append(") ENGINE = InnoDB DEFAULT CHARSET = utf8;");
            return stringBuilder.ToString();
        }

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
        /// 返回更新语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string UpdateSql<T>(T data,string tableName) where T : class
        {
            string pkName = GetPrimaryKeyName<T>(data);
            if (pkName == string.Empty) {  return string.Empty; }
            string pkValue = "";
            var strBuild = new StringBuilder();
            strBuild.Append("UPDATE ");
            strBuild.Append(tableName);
            strBuild.Append(" SET ");
            var properties = data.GetType().GetProperties();
            foreach (var property in properties)
            {
                //如果该列为主键
                if(property.Name == pkName)
                {
                    //记录主键值
                    pkValue = property.GetValue(data)!.ToString()!;
                }
                else
                {
                    string value;
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data)!;
                        value = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    }
                    else if(property.PropertyType == typeof(bool))
                    {
                        bool bValue = (bool)property.GetValue(data)!;
                        value = bValue ? "1" : "0";
                    }
                    else
                    {
                        value = property.GetValue(data)?.ToString()!;
                    }
                    //不为主键则拼接为 column = value
                    strBuild.Append($" {property.Name} = '{value}'");
                    if(property != properties.Last())
                    {
                        strBuild.Append(',');
                    }
                }
            }
            strBuild.Append($" WHERE {pkName} = '{pkValue}'");
            return strBuild.ToString();
        }

        /// <summary>
        /// 获取主键名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetPrimaryKeyName<T>(T data) where T : class 
        {
            var attr = data.GetType().GetCustomAttribute<GetPrimaryKeyAttribute>();
            if(attr == null) return string.Empty;
            if(attr.PrimaryKeyName == null) return string.Empty;
            return attr.PrimaryKeyName;
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
                }else if(value != null && value!.GetType() == typeof(bool))
                {
                    bool v = (bool)value;
                    int result = v ? 1 : 0;
                    return $"'{result}'";
                }
                return $"'{value}'";
            }).ToArray());
        }
    }
}
