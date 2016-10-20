using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;
using AutomatizerSQL.Utilidades;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using AutomatizerSQL.Contabilidad;
using Cliente = AutomatizerSQL.Core.Cliente;
using Empleado = AutomatizerSQL.Core.Empleado;

namespace AutomatizerSQL.Bodega
{
    public enum EstadosIngresoBodega
    {
        Ingresado =1,
        PendienteIngresar =2,
        Anulado =3
    }


    public enum EstadosContabilidad
    {
        Contabilizado =1,
        PendienteContabilizar = 2,
        Anulado =3
    }

    [DebuggerDisplay("Codigo = {TransaccionSistema.Codigo} Numero= {Numero} Total = {Total}")]
    public class IngresoBodega:ITransaccionManejaContabilidad
    {
        [Column("TrnCod"), ConstructorParameter("trnCod", null, true), ConstructorParameter("modulo", (byte)2, false)]
        public TransaccionSistema TransaccionSistema { get; private set; }
        [Column("TrnNum")]
        public int Numero { get;  set; }
        [Column("BodCod")]
        public Core.Bodega Bodega { get; set; }
        [Column("IngFec")]
        public DateTime Fecha { get; set; }
        //ProSec
        [Column("ProSec")]
        public Proveedor Proveedor { get; set; }

        //sucnum
        [Column("SucNum")]
        public byte SucursalProveedor { get; set; }

        [Column("ProNom")]
        public string NombeProveedor { get; set; }
        [Column("ProDat")]
        public string DatosProvedor { get; set; }

        [Column("EstBod"), DataParseParameter(DataParseParameter.TiposConvertidores.ToByte )]
        public EstadosIngresoBodega EstadoIngresoBodega { get; private set; }

        [Column("EstCon"), DataParseParameter(DataParseParameter.TiposConvertidores.ToByte )]
        public EstadosContabilidad EstadoContabilidad { get;  private set; }

        [Column("EplSec")]
        public Empleado Bodeguero { get; set; }


        public decimal TotalIngreso{ get; set; }

        [Column("IngObs")]
        public string Observaciones { get; set; }

        [NotMapped]
        public decimal Total => Detalles.Sum(d => d.Total);

        private decimal _totalI=0, _totalR=0, _totalD=0;

        private string _trnCodEg = string.Empty,
            _cliSec = string.Empty,
            _estado = "C",
            _tipFue = string.Empty,
            _fueTip2 = string.Empty,
            _fueTipSc = string.Empty,
            _trnCodSc= string.Empty;

        private int _trnNumEg = 0,
            _sucNumCli = 0,
            _fueNum2 = 0,
            _prProSec = 0,
            _prCntDev = 0,
            _numImp = 0,
            _fueNumSc = 0,
            _nroDocumentoCompra,
            _trnNumSc;

        private bool _esNuevo;


        [Column("AfeCos")]
        public bool AfectaCosto{ get; set; }

        [Column("ObsExt")]
        public string ObservacionesExtra { get; set; }

        [Column("ModIde")]
        public byte Modulo { get; set; }

        [Column("Venta")]
        public bool EsVenta { get; set; }

        [Column("CliSecV")]
        public Cliente ClienteVenta { get; set; }

        [Column("SucNumV")]
        public byte SucursalClienteVenta { get; set; }

        [Column("GuiRem")]
        public string GuiaDeRemision { get; set; }

        [Column("CtaCod")]
        public CuentaContable CuentaContableInventario { get; set; }


        public ManejaDetalle Detalles { get; set; }


        /// <summary>
        /// Constructor para crear nuevo ingreso.
        /// </summary>
        /// <param name="transaccion"></param>
        public IngresoBodega([NotNull]TransaccionSistema transaccion)
        {
            TransaccionSistema = transaccion;
            Detalles = new ManejaDetalle();
            _esNuevo = true;
            Bodega = transaccion.Bodega;
        }

        public IngresoBodega([NotNull]string codigo, int numero, [NotNull]Sesion sesion, bool usaCache = false)
        {
            _esNuevo = false;
            CargarDatos(codigo, numero,sesion);
            
        }

        private void CargarDatos(string codigo, int numero, Sesion sesion )
        {
            CargarCabecera(codigo, numero, sesion);
            CargarDetalles(sesion);
            
        }


        private void CargarCabecera(string codigo, int numero, Sesion sesion)
        {
            var datos = sesion.Data.CargarDatatable("SP_GET_IngresoBodegaCab", new DbParameter[]
            {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("@TrnCod", codigo),
                new SqlParameter("@TrnNum", numero),
            });
            datos.OnErrorThrow();
            if (datos.resultado.HasRows())
            {
                datos.resultado.Cargar<IngresoBodega>(this, sesion);
            }
            else
            {
                throw new InvalidOperationException(
                    $"No se encuentra ningún ingreso a bodega con código {codigo} y numero {numero}"

                    );
            }

        }


        private void CargarDetalles(Sesion sesion )
        {
            Detalles = new ManejaDetalle();
            var datos = sesion.Data.CargarDatatable("SP_GET_IngresoBodegaDet", new DbParameter[]
           {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("@TrnCod", TransaccionSistema.Codigo),
                new SqlParameter("@TrnNum", Numero )
           });
            datos.OnErrorThrow();
            //var detalles = datos.resultado.CargarConjunto<DetalleIngresoBodega>(sesion);
            Detalles.AddRange(datos.resultado.CargarConjunto<DetalleIngresoBodega>(sesion));
        }



        public void Guardar( Sesion sesion )
        {
            if (_esNuevo)
            {
                Numero = TransaccionSistema.CalcularSecuencial(sesion);
                EstadoIngresoBodega = EstadosIngresoBodega.Ingresado;
                EstadoContabilidad = EstadosContabilidad.Contabilizado;
            }

            var detallesBridge = new SqlBridges.BridgeDetalleIngresoBodegaCollection(Detalles);
            SqlParameter paramDetalle = new SqlParameter("Detalles", SqlDbType.Structured)
            {
                TypeName = "DetalleIngresoBodega",
                Value = detallesBridge
            };

            sesion.Data.Ejecutar("SP_GuardaIngesoBodega", new DbParameter[]
            {
                new SqlParameter("EmpCod", sesion.Empresa.Codigo),
                new SqlParameter("TrnCod", TransaccionSistema.Codigo),
                new SqlParameter("TrnNum", Numero),
                new SqlParameter("BodCod", (Bodega?.Codigo).IfIsNullReplace("")),
                new SqlParameter("IngFec", Fecha.Date),
                new SqlParameter("Hora", Fecha.TimeOfDay),
                new SqlParameter("ProSec", (Proveedor?.Codigo).IfIsNullReplace("")),
                new SqlParameter("SucNum", SucursalProveedor),
                new SqlParameter("ProNom", NombeProveedor.IfIsNullReplace("")),
                new SqlParameter("ProDat", DatosProvedor.IfIsNullReplace("")),
                new SqlParameter("EstBod", EstadoIngresoBodega),
                new SqlParameter("EstCon", EstadoContabilidad),
                new SqlParameter("EplSec", sesion.Usuario.Empleado.Codigo),
                new SqlParameter("FueTip", ""),
                new SqlParameter("FueNum", (object) 0),
                new SqlParameter("IngTot", Total),
                new SqlParameter("IngObs", Observaciones.IfIsNullReplace("")),
                new SqlParameter("ModCodCom", (object) 1),
                new SqlParameter("Total", Total),
                new SqlParameter("TotalI", _totalI),
                new SqlParameter("TotalR", _totalR),
                new SqlParameter("TotalD", _totalD),
                new SqlParameter("AfeCos", AfectaCosto),
                new SqlParameter("TrnCodEG", _trnCodEg.IfIsNullReplace("")),
                new SqlParameter("TrnNumEG", _trnNumEg),
                new SqlParameter("CliSec", _cliSec.IfIsNullReplace("")),
                new SqlParameter("SucNumCli", _sucNumCli),
                new SqlParameter("ObsExt", ObservacionesExtra.IfIsNullReplace("")),
                new SqlParameter("TipFue", _tipFue.IfIsNullReplace("")),
                new SqlParameter("Estado", "C"),
                new SqlParameter("FueTip2", _fueTip2.IfIsNullReplace("")),
                new SqlParameter("FueNum2", _fueNum2),
                new SqlParameter("ModIde", 2),
                new SqlParameter("Venta", EsVenta),
                new SqlParameter("CliSecV", (ClienteVenta?.Codigo).IfIsNullReplace("")),
                new SqlParameter("SucNumV", SucursalClienteVenta),
                new SqlParameter("PRProSec", _prProSec),
                new SqlParameter("PRCntDev", _prCntDev),
                new SqlParameter("NumImp", _numImp),
                new SqlParameter("FueTipSC", _fueTipSc.IfIsNullReplace("")),
                new SqlParameter("FueNumSC", _fueNumSc),
                new SqlParameter("GuiRem", GuiaDeRemision.IfIsNullReplace("")),
                new SqlParameter("CtaCod", (TransaccionSistema?.Cuenta.Codigo).IfIsNullReplace("")),
                new SqlParameter("NroDocumentoCompra", _nroDocumentoCompra),
                new SqlParameter("TrnCodSC", _trnCodSc.IfIsNullReplace("")),
                new SqlParameter("TrnNumSC", _trnNumSc),
                new SqlParameter("FechaTrn", Fecha),
                paramDetalle
            }).OnErrorThrow();

            string cadenaEdicion = HelperBodega.GeneraCadenaEdicion(Detalles);
            //var s = new Acceso.Session(sesion.Empresa.Codigo, sesion.Usuario.Nombre,
            //    new SqlConnection(sesion.Data.CadenaConeccion));
           

            //ProcesarKardex(sesion, s, cadenaEdicion);

            if (TransaccionSistema.Modulo.LigadoContabilidad(sesion ) && TransaccionSistema.AfectaContabilidad  )
            {
                var cmp = ComprobanteContable.CrearNuevoComprobante(Fecha,
                    TransaccionSistema.TipoComprobanteContable, TransaccionSistema.Modulo, sesion, true);
                cmp.MotivoComprobante = $"{TransaccionSistema.Codigo }-{Numero} {Observaciones}" ;
                var asiento = AsientoContable.CrearNuevoAsiento(cmp);
                asiento.Descripcion = cmp.MotivoComprobante;
                foreach (IDetalle detalle in Detalles)
                {

                    asiento.Cuentas.Add(new CuentaAsientoContable(asiento, detalle.Item.CuentaInventario)
                    {
                        TipoSaldo = CuentaAsientoContable.TiposSaldo.Debe,
                        Debe = decimal.Round(detalle.Cantidad * detalle.Costo, 2, MidpointRounding.ToEven),
                    });
                }
                if (TransaccionSistema.Cuenta == null)
                {
                    throw new InvalidOperationException("No se a especificado la cuenta contable de la transaccion " +
                                                        TransaccionSistema.Codigo);
                }
                var cuenta = new CuentaAsientoContable(asiento, TransaccionSistema.Cuenta )
                {
                    TipoSaldo = CuentaAsientoContable.TiposSaldo.Haber,
                    Haber = decimal.Round(Total , 2, MidpointRounding.ToEven),
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


            }
        }

        //private void ProcesarKardex(Sesion sesion, Acceso.Session s, string cadenaEdicion)
        //{
        //    var cn = sesion.Data.Conexion;
        //    s.Data = new Acceso.Session.DataAcces(ref cn) {Transaccion = sesion.Data.Transaccion};
        //    //ClsFunciones.Automatizer.Bodega.Funciones.ProcesarKardexOptimizada(cadenaEdicion, Fecha, "",true, ref s);
        //}


        #region ImplementacionInterfazITransaccionManejaContabilidad
        public TransaccionSistema TransaccionParaTrnVsCmp => TransaccionSistema;
        public int NumeroParaTrnVsCmp => Numero;
        
        


        #endregion
    }
}