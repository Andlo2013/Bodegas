using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;
using AutomatizerSQL.Data.SqlClient;

namespace AutomatizerSQL.Bodega
{
    public class TipoTraspaso
    {
        [Column("TraspCod")]
        public byte Codigo { get; set; }

        [Column("TraspDsc")]
        public string Descripcion { get; set; }

        [Column("TrnCodEgr" ), ConstructorParameter("trnCod",null, true ), ConstructorParameter("modulo", (byte)2, false)]
        public TransaccionSistema TransaccionEgreso { get; set; }

        [Column("TrnCodIng"), ConstructorParameter("trnCod", null, true), ConstructorParameter("modulo", (byte)2, false)]
        public TransaccionSistema TransaccionIngreso { get; set; }

        [Column("BodCodOrig")]
        public Core.Bodega BodegaOrigen { get; set; }

        [Column("BodCodDest")]
        public Core.Bodega BodegaDestino{ get; set; }

        [Column ("RecupTodos")]
        public bool RecuperarTodos { get; set; }

        public bool Devuelve { get; set; }

        [IgnoreDefaultValue((Int16)0)]
        [Column("EplCod")]
        public Empleado Empleado { get; set; }

        [IgnoreDefaultValue((byte)0)]
        [Column("CodSuc")]
        public SucursalEmpresa Sucursal { get; set; }

        public bool ImpresionAutomatica { get; set; }

        public bool ConfirmaTraspaso { get; set; }

        public string EplAutorizadosConfirmacion { get; set; }

        public TipoTraspaso() { }

        public TipoTraspaso(byte codigo, Sesion sesion, bool usaCache=false )
        {
            var datos =
                sesion.Data.CargarDatatable(@"SELECT TraspCod, TraspDsc, TrnCodEgr, TrnCodIng, BodCodOrig, BodCodDest, RecupTodos, Devuelve, EplCod, CodSuc, ImpresionAutomatica, ConfirmaTraspaso, EplAutorizadosConfirmacion
                FROM TiposTraspasos
                WHERE (EmpCod = @EmpCod) AND (TraspCod = @TraspCod)", new DbParameter[]
                {
                    sesion.Empresa.ToSqlParameter() ,
                    new SqlParameter("TraspCod", codigo )  
                });

            datos.OnErrorThrow();
            datos.OnEmptyDatasetThrow(GetType(), codigo.ToString());
            datos.resultado.Cargar(this,sesion );

        }


        public static IEnumerable<TipoTraspaso> ObtenerTodos(Sesion sesion, bool usaCache = false)
        {
            var datos =
                sesion.Data.CargarDatatable(@"SELECT EmpCod, TraspCod, TraspDsc, TrnCodEgr, TrnCodIng, BodCodOrig, BodCodDest, RecupTodos, Devuelve, EplCod, CodSuc, ImpresionAutomatica, ConfirmaTraspaso, EplAutorizadosConfirmacion
                FROM TiposTraspasos
                WHERE(EmpCod = @EmpCod) ", new DbParameter[]
                {
                    sesion.Empresa.ToSqlParameter()
                });

            datos.OnErrorThrow();

            var resultado = datos.resultado.CargarConjunto<TipoTraspaso>(sesion);

            //var r = new List<TipoTraspaso>(datos.resultado.Rows.Count );

            //foreach (DataRow row in datos.resultado.Rows)
            //{
                
            //}

            return resultado;

        }
    }
}
