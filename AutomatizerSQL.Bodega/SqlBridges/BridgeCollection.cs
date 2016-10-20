using System.Collections.Generic;
using System.Linq;
using AutomatizerSQL.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace AutomatizerSQL.Bodega.SqlBridges
{
    public class BridgeCollection<T> : List<T>,  IEnumerable<SqlDataRecord>
    {
        public  IEnumerator<SqlDataRecord> GetEnumerator()
        {
            var sqlDataRecord = new SqlDataRecord(typeof(T).GetSqlMetaData().ToArray());
            var lista = (List<T>) this;
            foreach (T item in lista)
            {
                sqlDataRecord.PopulateSqlDataRecord(item);
                yield return sqlDataRecord;
            }

            //foreach (var item in this)
            //{
            //    sqlDataRecord.PopulateSqlDataRecord(item);
            //    //sqlDataRecord.SetInt32(0, multiSelectionItem.Id);
            //    yield return sqlDataRecord;
            //}
        }
    }

    //public class x
    //{
    //    public void y()
    //    {
    //        var coll = new BridgeCollection<List<int>>();
            
            
    //    }
    //}
}
