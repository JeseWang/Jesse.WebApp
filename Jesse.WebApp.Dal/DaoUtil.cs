using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jesse.WebApp.Dal
{
    public class DaoUtil<T>
    {
        private static readonly Type type = typeof(T);

        public static ISqlMapper SqlMapper
        {
            get
            {
                ISqlMapper SqlMapper = null;
                //return SqlMapperFactory.GetSqlMap("SqlMap.config", "true");
                return SqlMapper;
            }
        }

        /// <summary>
        /// 根据Id删除数据
        /// </summary>
        /// <param name="id">数据id</param>
        public static void Delete(int id)
        {
            SqlMapper.Delete(string.Format("{0}.Delete", type.Name), id);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="statementName">脚本名称</param>
        /// <param name="query">脚本所用参数</param>
        /// <returns></returns>
        public static void Delete(string statementName, object query)
        {
            SqlMapper.Delete(string.Format("{0},{1}", type.Name, statementName), query);
        }

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        public static T FindById(int id)
        {
            return SqlMapper.QueryForObject<T>(string.Format("{0}.FindById", type), id);
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="statement">脚本名称</param>
        /// <param name="query">脚本所用参数</param>
        /// <returns></returns>
        public static T QueryForObject(string statementName, object query)
        {
            return SqlMapper.QueryForObject<T>(string.Format("{0}.{1}", type.Name, statementName), query);
        }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="statement">脚本名称</param>
        /// <param name="query">脚本所用参数</param>
        /// <returns></returns>
        public static List<T> QueryForList(string statementName, object query)
        {
            return SqlMapper.QueryForList<T>(string.Format("{0}.{1}", type.Name, statementName), query).ToList();
        }

        /// <summary>
        /// 获取其他返回值类型的单条数据
        /// </summary>
        /// <typeparam name="R">返回值类型</typeparam>
        /// <param name="statement">脚本名称</param>
        /// <param name="query">脚本所用参数</param>
        /// <returns></returns>
        public static R QueryForObject<R>(string statementName, object query)
        {
            return SqlMapper.QueryForObject<R>(string.Format("{0}.{1}", type.Name, statementName), query);
        }

        /// <summary>
        /// 获取其他返回值类型的多条数据
        /// </summary>
        /// <typeparam name="R">返回值类型</typeparam>
        /// <param name="statement">脚本名称</param>
        /// <param name="query">脚本所用参数</param>
        /// <returns></returns>
        public static List<R> QueryForList<R>(string statementName, object query)
        {
            return SqlMapper.QueryForList<R>(string.Format("{0}.{1}", type.Name, statementName), query).ToList();
        }

        public static void CheckCount(int count)
        {
            if(count == 0)
            {
                throw new Exception("数据已过时，请重新加载");
            }
            else
            {
                throw new Exception("更新或删除时存在重复数据");
            }
        }

    }
}
