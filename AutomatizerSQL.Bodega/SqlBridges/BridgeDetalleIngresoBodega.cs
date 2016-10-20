using System.Collections.Generic;
using System.Linq;
using AutomatizerSQL.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace AutomatizerSQL.Bodega.SqlBridges
{
    public class BridgeDetalleIngresoBodega
    {
        public string Codigo { get; set; }
        public byte Secuencial { get; set; }

        public int Unidad { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Costo { get; set; }

        public decimal CostoPromedio { get; set; }

        public decimal CostoUltimo { get; set; }

        public int ModCodIng { get; set; }

        public int ModCodCom { get; set; }

        public string Observaciones { get; set; }

        public string Bodega { get; set; }

        public int Orden { get; set; }
    }


    public class BridgeDetalleIngresoBodegaCollection : List<BridgeDetalleIngresoBodega>, IEnumerable<SqlDataRecord>
    {
        //private IEnumerable<DetalleIngresoBodega> detalles;

        public BridgeDetalleIngresoBodegaCollection(IEnumerable<IDetalle> detalles)
        {
            foreach (var detalleIngresoBodega in detalles.Cast<IDetalle>())
            {
                this.Add(new SqlBridges.BridgeDetalleIngresoBodega()
                {
                    Secuencial  = detalleIngresoBodega.Secuencial,
                    Unidad = detalleIngresoBodega.Item.Unidad.Codigo,
                    Bodega = detalleIngresoBodega.Bodega.Codigo,
                    Cantidad = detalleIngresoBodega.Cantidad,
                    Codigo = detalleIngresoBodega.Item.Codigo,
                    Costo = detalleIngresoBodega.Costo,
                    CostoPromedio = 0m,
                    CostoUltimo = 0m,
                    ModCodCom = 1,
                    ModCodIng = 1,
                    Observaciones = detalleIngresoBodega.Observaciones,
                    Orden = detalleIngresoBodega.Orden
                });
            }
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sqlDataRecord = new SqlDataRecord( typeof (BridgeDetalleIngresoBodega).GetSqlMetaData().ToArray() );
            foreach (BridgeDetalleIngresoBodega item in this)
            {
                sqlDataRecord.PopulateSqlDataRecord(item);
                //sqlDataRecord.SetInt32(0, multiSelectionItem.Id);
                yield return sqlDataRecord;
            }

        }


    }


}
