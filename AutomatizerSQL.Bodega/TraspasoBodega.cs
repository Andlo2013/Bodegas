using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;

namespace AutomatizerSQL.Bodega
{
    public class TraspasoBodega
    {
        private EgresoBodega _egresoBodega;
        private IngresoBodega _ingresoBodega;

        [Column("TraspNro")]
        public int Numero { get; set; }

        [Column("TraspFec")]
        public DateTime Fecha { get; set; }

        [Column("TraspObs")]
        public string Observaciones { get; set; }


        [Column("TrnCodEgr")]
        public string CodigoTransaccionEgreso { get; set; }

        [Column("TrnNumEgr")]
        public int NumeroTransaccionEgreso { get; set; }

        [Column("TrnCodIng")]
        public string CodigoTransaccionIngreso { get; set; }


        [Column("TrnNumIng")]
        public int NumeroTransaccionIngreso { get; set; }

       [Column("BodCodOrigen")]
       public string CodigoBodegaOrigen { get; set; }

        [Column("BodCodDestino")]
        public string CodigoBodegaDestino { get; set; }

        [Column("EplSecOrigen")]
        public Empleado EmpleadoEntrega { get; set; }


        [Column("EplSecDestino")]
        public Empleado EmpleadoRecibe { get; set; }



        [Column("TraspEst")]
        public byte Estado { get; set; }

        [Column("ObsExt")]
        public string ObservacionesAdicionales { get; set; }


        [Column("Venta")]
        public bool EsVenta { get; set; }


        [Column("CliSecV")]
        public Cliente ClienteVenta { get; set; }

        [Column("SucNumV")]
        public byte SucursalClienteVenta { get; set; }


        [Column("TraspCod")]
        public byte CodigoTraspaso { get; set; }


        private readonly Lazy<EgresoBodega> _egresoBodegaLazy;
        public EgresoBodega EgresoBodega
        {
            get { return _egresoBodega ?? (_egresoBodega = _egresoBodegaLazy.Value); }
            set
            {
                _egresoBodega = value;
            }
        }


        private readonly Lazy<IngresoBodega> _ingresoBodegaLazy;
        public IngresoBodega IngresoBodega
        {
            get { return _ingresoBodega   ?? (_ingresoBodega = _ingresoBodegaLazy.Value)  ; }
            set { _ingresoBodega = value; }
        }


        public TraspasoBodega(int numero, Sesion sesion, bool usaCache = false)
        {
            var datos = sesion.Data.CargarDatatable("GET_TraspasoBodegaCab", new DbParameter[]
            {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("TraspNro", numero)
            });
            datos.OnErrorThrow();
            datos.resultado.Cargar(this, sesion);
            _egresoBodegaLazy =
                new Lazy<EgresoBodega>(
                    () => new EgresoBodega(CodigoTransaccionEgreso, NumeroTransaccionEgreso, sesion, true));
            _ingresoBodegaLazy =
                new Lazy<IngresoBodega>(
                    () => new IngresoBodega(CodigoTransaccionIngreso, NumeroTransaccionIngreso, sesion, true));
        }






    }
}
