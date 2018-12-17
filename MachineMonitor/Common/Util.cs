using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Util
    {
        /// <summary>
        /// dataTable转list对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable)
        {
            string jsonString = JsonConvert.SerializeObject(dataTable, new DataTableConverter());
            return JsonConvert.DeserializeObject<List<T>>(jsonString);
        }
        /// <summary>
        /// list对象集转dataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return JsonConvert.DeserializeObject<DataTable>(jsonString, new DataTableConverter());
        }
    }
}
