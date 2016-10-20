using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Contabilidad;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;
using AutomatizerSQL.Utilidades;


namespace AutomatizerSQL.Bodega
{
    public enum EstadosEgresoBodega
    {
        Egreso = 1,
        PendienteEgresor = 2,
        Anulado = 3
    }

    [DebuggerDisplay("Codigo = {TransaccionSistema.Codigo} Numero= {Numero} Total = {Total}")]
    public class EgresoBodega:ITransaccionManejaContabilidad
    {
        [Column("TrnCod"), ConstructorParameter("trnCod", null, true), ConstructorParameter("modulo", (byte)2, false)]
        public TransaccionSistema TransaccionSistema { get; private set; }
        [Column("TrnNum")]
        public int Numero { get; set; }
        [Column("BodCod")]
        public Core.Bodega Bodega { get; set; }
        [Column("EgrFec")]
        public DateTime Fecha { get; set; }

        [Column("EstBod"), DataParseParameter( DataParseParameter.TiposConvertidores.ToByte ) ]
        public EstadosEgresoBodega EstadoEgresoBodega { get; private set; }

        [Column("EstCon"), DataParseParameter( DataParseParameter.TiposConvertidores.ToByte ) ]
        public EstadosContabilidad EstadoContabilidad { get; private set; }

        [Column("EplSec")]
        public Empleado Bodeguero { get; set; }

        string _fueTip = "";
        int _fueNum = 0;

        [NotMapped]
        public decimal Total => Detalles.Sum(d => d.Total);

        [Column("EgrObs")]
        public string Observaciones { get; set; }

        [Column("EplSecRec")]
        public Empleado EmpleadoRecibe { get; set; }

        [Column("Venta")]
        public bool EsVenta { get; set; }

        [Column("CliSec")]
        public Cliente Cliente { get; set; }

        [Column("SucNum")]
        public byte SucursalCliente { get; set; }

        [Column("EstVen"), DataParseParameter( DataParseParameter.TiposConvertidores.ToBool ) ]
        public bool EstadoVenta { get; set; }

        [Column("FueIng")]
        public bool EsFuenteDeUnIngreso { get; set; }

        [Column("FueNumIng")]
        public int NumeroIngresoQueEsFuente { get; set; }

        [Column("FueTipIng")]
        public int TipoIngresoQueEsFuente { get; set; }

        [Column("ObsExt")]
        public string ObservacionesExtra { get; set; }

        private string _estado = "C";
        private bool _esNuevo;
        //private TransaccionSistema _transaccionParaTrnVsCmp;
        //private int _numeroParaTrnVsCmp;

        [Column("ModIde")]
        public byte Modulo { get; set; }

        [Column("SrvCodCombo")]
        public string CodigoComboServicio { get; set; }

        [Column("ComboCant")]
        public short CantidadComboServicio { get; set; }

        [Column("ComboFacCod")]
        public string CodigoFacturaCombo { get; set; }

        [Column("ComboFacNum")]
        public int NumeroFacturaCombo { get; set; }


        [Column("ComboNombre")]
        public string NombreComboServicios { get; set; }


        public int ProSec { get; set; }

        public short LotSec { get; set; }

        public string ClienteSec { get; set; }

        public int PrySec { get; set; }

        public byte NumImp { get; set; }

        [Column("OTTrnCod")]
        public string OrdenTrabajoTrnCod { get; set; }
        [Column("OTSec")]
        public int OrdenTrabajoSecuencial { get; set; }

        [Column("CtaCod")]
        public CuentaContable CuentaContableEgreso { get; set; }

        [Column("CodProveedor")]
        public Proveedor Proveedor { get; set; }

        public string TrnCodNCProv { get; set; }

        public int TrnNumNCProv { get; set; }


        public ManejaDetalle Detalles { get; set; }


        public EgresoBodega([NotNull]TransaccionSistema transaccion)
        {
            TransaccionSistema = transaccion;
            Detalles = new ManejaDetalle();
            _esNuevo = true;
            Bodega = transaccion.Bodega;
        }

        public EgresoBodega([NotNull]string codigo, int numero, [NotNull]Sesion sesion, bool usaCache = false)
        {
            _esNuevo = false;
            CargarDatos(codigo, numero, sesion);

        }

        private void CargarDatos(string codigo, int numero, Sesion sesion)
        {
            CargarCabecera(codigo, numero, sesion);
            CargarDetalles(sesion);
        }
        private void CargarCabecera(string codigo, int numero, Sesion sesion)
        {
            var datos = sesion.Data.CargarDatatable("SP_GET_EgresoBodegaCab", new DbParameter[]
            {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("TrnCod", codigo),
                new SqlParameter("TrnNum", numero),
            });
            datos.OnErrorThrow();
            if (datos.resultado.HasRows())
            {
                datos.resultado.Cargar(this, sesion);
            }
            else
            {
                throw new InvalidOperationException(
                    $"No se encuentra ningún egreso a bodega con código {codigo} y numero {numero}"

                    );
            }

        }

        private void CargarDetalles(Sesion sesion)
        {
            Detalles = new ManejaDetalle();
            var datos = sesion.Data.CargarDatatable("SP_GET_EgresoBodegaDet", new DbParameter[]
           {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("@TrnCod", TransaccionSistema.Codigo),
                new SqlParameter("@TrnNum", Numero )
           });
            datos.OnErrorThrow();
            //var detalles = datos.resultado.CargarConjunto<DetalleIngresoBodega>(sesion);
            Detalles.AddRange(datos.resultado.CargarConjunto<DetalleIngresoBodega>(sesion));
        }


        public void Guardar(Sesion sesion)
        {
            if (_esNuevo)
            {
                Numero = TransaccionSistema.CalcularSecuencial(sesion);
                EstadoEgresoBodega = EstadosEgresoBodega.Egreso;
                EstadoContabilidad = EstadosContabilidad.Contabilizado;
            }
            var detallesBridge = new SqlBridges.BridgeDetalleIngresoBodegaCollection(Detalles);
            SqlParameter paramDetalle = new SqlParameter("Detalles", SqlDbType.Structured)
            {
                TypeName = "DetalleIngresoBodega",
                Value = detallesBridge
            };
            sesion.Data.Ejecutar(@"SP_GuardaEgresoBodega", new DbParameter[]
            {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("SucNro", "01"), 
                new SqlParameter("TrnCod", TransaccionSistema.Codigo),
                new SqlParameter("TrnNum", Numero),
                new SqlParameter("BodCod", (Bodega?.Codigo).IfIsNullReplace("")),
                new SqlParameter("EgrFec", Fecha.Date.Date),
                new SqlParameter("Hora", new DateTime(1899, 12, 30, Fecha.Hour, Fecha.Minute, Fecha.Second)),
                new SqlParameter("EstBod", EstadoEgresoBodega),
                new SqlParameter("EstCon", EstadoContabilidad),
                new SqlParameter("EplSec", Bodeguero.Codigo),
                new SqlParameter("FueTip", ""),
                new SqlParameter("FueNum", (object) 0),
                new SqlParameter("EgrTot", Total),
                new SqlParameter("EgrObs", Observaciones),
                new SqlParameter("EplSecRec", (EmpleadoRecibe?.Codigo).IfIsNullReplace("")    ),
                new SqlParameter("Venta", EsVenta),
                new SqlParameter("CliSec",  (Cliente?.Codigo).IfIsNullReplace("")   ),
                new SqlParameter("SucNum", SucursalCliente),
                new SqlParameter("EstVen", EstadoVenta.ToInt()),
                new SqlParameter("FueIng", false),
                new SqlParameter("FueNumIng", (object) 0),
                new SqlParameter("FueTipIng", ""),
                new SqlParameter("ObsExt", ObservacionesExtra.IfIsNullReplace("")),
                new SqlParameter("Estado", "C"),
                new SqlParameter("ModIde", 2 ),
                new SqlParameter("SrvCodCombo", ""),
                new SqlParameter("ComboCant", (object) 0),
                new SqlParameter("ComboFacCod", ""),
                new SqlParameter("ComboFacNum", (object) 0),
                new SqlParameter("ComboNombre", ""),
                new SqlParameter("ProSec", (object) 0),
                new SqlParameter("LotSec", (object) 0),
                new SqlParameter("ClienteSec", ""),
                new SqlParameter("PrySec", (object) 0),
                new SqlParameter("NumImp", (object) 0),
                new SqlParameter("OTTrnCod", ""),
                new SqlParameter("OTSec", (object) 0),
                new SqlParameter("CtaCod", (TransaccionSistema.Cuenta?.Codigo).IfIsNullReplace("")),
                new SqlParameter("CodProveedor", (Proveedor?.Codigo).IfIsNullReplace("") ),
                new SqlParameter("TrnCodNCProv", ""),
                new SqlParameter("TrnNumNCProv", (object) 0),
                paramDetalle
            });

            string cadenaEdicion = HelperBodega.GeneraCadenaEdicion(Detalles);
            //ProcesarKardex(sesion, s, cadenaEdicion);
            RecargarCostos(sesion);
            if (TransaccionSistema.Modulo.LigadoContabilidad(sesion) && TransaccionSistema.AfectaContabilidad)
            {
                var cmp = ComprobanteContable.CrearNuevoComprobante(Fecha,
                    TransaccionSistema.TipoComprobanteContable, TransaccionSistema.Modulo, sesion, true);
                cmp.MotivoComprobante = $"{TransaccionSistema.Codigo }-{Numero} {Observaciones}";
                var asiento = AsientoContable.CrearNuevoAsiento(cmp);
                asiento.Descripcion = cmp.MotivoComprobante;
                foreach (IDetalle detalle in Detalles)
                {

                    asiento.Cuentas.Add(new CuentaAsientoContable(asiento, detalle.Item.CuentaInventario)
                    {
                        TipoSaldo = CuentaAsientoContable.TiposSaldo.Haber,
                        Haber = decimal.Round(detalle.Cantidad * detalle.Costo, 2, MidpointRounding.ToEven),
                    });
                }
                if (TransaccionSistema.Cuenta == null)
                {
                    throw new InvalidOperationException("No se a especificado la cuenta contable de la transaccion " +
                                                        TransaccionSistema.Codigo);
                }
                var cuenta = new CuentaAsientoContable(asiento, TransaccionSistema.Cuenta)
                {
                    TipoSaldo = CuentaAsientoContable.TiposSaldo.Debe,
                    Debe = decimal.Round(Total, 2, MidpointRounding.ToEven),
                };
                asiento.Cuentas.Add(cuenta);
                asiento.Tipo = AsientoContable.TiposAsiento.Automatico;
                if (!_esNuevo)
                {
                    Contabilidad.HelperContabilidad.EliminaContabilidadDeTransaccion(2, TransaccionSistema.Codigo,
                        Numero, sesion);
                }
                cmp.Asientos.Add(asiento);
                cmp.Guardar(sesion, this);
                _esNuevo = true;

            }
        }

        //private void ProcesarKardex(Sesion sesion, Acceso.Session s, string cadenaEdicion)
        //{
        //    var cn = sesion.Data.Conexion;
        //    s.Data = new Acceso.Session.DataAcces(ref cn) { Transaccion = sesion.Data.Transaccion };
        //    //ClsFunciones.Automatizer.Bodega.Funciones. ProcesarKardexOptimizada( cadenaEdicion, Fecha,"" ,true, ref s);
        //}



        private void RecargarCostos(Sesion sesion)
        {
            const int colItmCod = 0;
            const int colISec = 1;
            const int colCostoUnitario = 2;
            
            var cmd = sesion.Data.Conexion.CreateCommand();
            cmd.Transaction = sesion.Data.Transaccion;
            cmd.CommandText =
                @"SELECT ItmCod, ISec, CAST(EgrCosUni AS DECIMAL(14, 6)) AS CostoUnitario FROM BodEgrDet WHERE (EmpCod = @EmpCod) AND (TrnCod = @TrnCod) AND (TrnNum = @TrnNum)";
            cmd.Parameters.Add(new SqlParameter("EmpCod", sesion.Empresa.Codigo));
            cmd.Parameters.Add(new SqlParameter("TrnCod", TransaccionSistema.Codigo));
            cmd.Parameters.Add(new SqlParameter("TrnNum", Numero));
            Exception error = null;
            DbDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var detalle =
                        Detalles.First(
                            d => d.Item.Codigo == reader.GetString(colItmCod) && d.Secuencial == reader.GetByte(colISec));
                    detalle.Costo = reader.GetDecimal(colCostoUnitario);
                }
            }
            catch (Exception exception) {
                error = exception;
            }
            finally
            {
                if( reader != null && !reader.IsClosed  ) reader.Close();
            }
            var itemsCostoCero = Detalles.Where(d => d.Costo <= 0).ToList();
            if(!itemsCostoCero.Any()) return;
            var mensaje = "SE ENCONTRARON LOS SIGUIENTES ITEMS CON COSTO <= 0" +  Environment.NewLine ;
            mensaje = itemsCostoCero.Aggregate(mensaje, (current, b) => current + (b.Item.Codigo + ","));
            mensaje = mensaje.Substring(0, mensaje.Length - 1);
            throw new InvalidOperationException(mensaje);
        }





        public TransaccionSistema TransaccionParaTrnVsCmp => TransaccionSistema;

        public int NumeroParaTrnVsCmp => Numero;
    }
}
