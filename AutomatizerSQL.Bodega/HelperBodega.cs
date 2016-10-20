using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AutomatizerSQL.Bodega.SqlBridges;
using AutomatizerSQL.Core;
using AutomatizerSQL.Utilidades;

namespace AutomatizerSQL.Bodega
{
    public static class HelperBodega
    {


        public static bool ProcesaKardex(IEnumerable<IDetalle> detalles, DateTime fechaTransaccion, bool calcularPrecios, Sesion sesion, bool actualizarTotales = true)
        {
            var fechaCorreccion = fechaTransaccion.AddMinutes(-5);
            var itemsAPocesar = RecuperaItemsAProcesar(detalles, sesion);
            EstablecerValoresCeroEnBodegaParaItemsAProcesarKardex(sesion, itemsAPocesar);
            var regeneraKardexSi = RecuperaSaldosItemsARegenerar(sesion, itemsAPocesar, fechaCorreccion);
            


            return true;
        }

        private static void EstablecerValoresCeroEnBodegaParaItemsAProcesarKardex(Sesion sesion, BridgeCollection<BridgeResumenItemAsignadoBodega> itemsAPocesar)
        {
            var sqlParamItems = new SqlParameter("Items", SqlDbType.Structured);
            sqlParamItems.TypeName = "T_ResumenItemAsignadoBodega";
            sqlParamItems.Value = itemsAPocesar;

            sesion.Data.Ejecutar("SP_EstablecerSaldoCeroParaProcesoKardex", new DbParameter[] {sqlParamItems});
        }

        private static BridgeCollection<BridgeResumenItemAsignadoBodega> RecuperaItemsAProcesar(IEnumerable<IDetalle> detalles, Sesion sesion)
        {
            var itemsAPocesar =
                new SqlBridges.BridgeCollection<SqlBridges.BridgeResumenItemAsignadoBodega>();
            foreach (IDetalle detalle in detalles)
            {
                itemsAPocesar.Add(new BridgeResumenItemAsignadoBodega()
                {
                    BodCod = detalle.Bodega.Codigo,
                    EmpCod = sesion.Empresa.Codigo,
                    ItmCod = detalle.Item.Codigo,
                    Saldo = 0
                });
            }
            return itemsAPocesar;
        }

        private static EstRegeneraKardexBodegaSi[] RecuperaSaldosItemsARegenerar<T>(Sesion sesion, BridgeCollection<T> itemsEditados,
            DateTime fechaCorreccion)
        {
            var itemsParam = new SqlParameter("@items", SqlDbType.Structured);
            itemsParam.TypeName = "T_ResumenItemAsignadoBodega";
            itemsParam.Value = itemsEditados;

            var datos =
                sesion.Data.CargarDatatable(
                    @"SP_GET_SaldosItemsAFecha",
                    new DbParameter[]
                    {
                        new SqlParameter("Fecha", fechaCorreccion),
                        itemsParam
                    });
            var regeneraKardexSi = new EstRegeneraKardexBodegaSi[datos.resultado.Rows.Count];
            var i = 0;
            foreach (var estRegenera in from DataRow row in datos.resultado.Rows
                select new EstRegeneraKardexBodegaSi
                {
                    BodCod = row["BodCod"].DbString(),
                    ItmCod = row["ItmCod"].DbString(),
                    Saldo = row["Saldo"].ToDecimal(),
                    Costo = 0
                })
            {
                regeneraKardexSi[i] = estRegenera;

                i++;
            }
            
            return regeneraKardexSi;
        }

        public static string GeneraCadenaEdicion(IEnumerable<IDetalle> detalles)
        {
            var sb = new StringBuilder();
            foreach (IDetalle detalle in detalles)
            {
                sb.Append($"'{detalle.Item.Codigo}{detalle.Bodega.Codigo}',");
            }
            var cadenaEdicion = sb.ToString(0, sb.Length - 1);
            return cadenaEdicion;
        }
    }
}
