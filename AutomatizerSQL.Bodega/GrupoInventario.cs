using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;
using AutomatizerSQL.Utilidades;

namespace AutomatizerSQL.Bodega
{
    public sealed class GrupoInventario
    {
        [Column("Secuencial")]
        public int Id { get; set; }
        [Column("ItmCod")]
        public string Codigo { get; set; }

        [Column("ItmDsc")]
        public string Nombre { get; set; }

        [Column("ItmNiv")]
        public byte Nivel { get; set; }

        public byte[] NodeImage { get; set; }

        public GrupoInventario() { }

        public bool TieneExistencia { get; set; }

        public IEnumerable<GrupoInventario> GetChildGroups(Sesion sesion)
        {
            var datosHijos =
                sesion.Data.CargarDatatable(
                    @"SELECT Secuencial, ItmCod, ItmDsc, ItmNiv, CAST(1 as bit) as TieneExistencia FROM ItmMae WHERE (EmpCod = @EmpCod) AND (ItmTip = 'G') AND (ItmNiv = @Nivel) AND (ItmCod LIKE @CodGroup)",
                    new DbParameter[]
                    {
                        new SqlParameter("@EmpCod", sesion.Empresa.Codigo),
                         new SqlParameter("@Nivel", Nivel +1),
                         new SqlParameter(@"CodGroup", Codigo + "%") 
                    });
            datosHijos.OnErrorThrow();
            var hijos = datosHijos.resultado.CargarConjunto<GrupoInventario>();
            return hijos;
        }


        public IEnumerable<int> GetIdsItems(Sesion sesion)
        {
            var datos =
                sesion.Data.CargarDatatable(
                    "SELECT Secuencial FROM ItmMae WHERE (EmpCod = @EmpCod) AND (ItmTip = 'I') AND (ItmNiv = @nivel) AND (ItmCod Like @ItmCod) ORDER BY ItmDsc ",
                    new DbParameter[]
                    {
                        new SqlParameter("@EmpCod", sesion.Empresa.Codigo),
                         new SqlParameter("@Nivel", Nivel +1),
                         new SqlParameter("@ItmCod", Codigo + "%") 
                    });

            datos.OnErrorThrow();
            return from DataRow row in datos.resultado.Rows select row[0].ToInt();
        }


        public IEnumerable<GrupoInventario> GetItemsAsGroupInfoForNodes(Sesion sesion)
        {
            var datos =
                sesion.Data.CargarDatatable(
                    @"SELECT ItmMae.Secuencial, ItmMae.ItmCod, ItmMae.ItmDsc, ItmMae.ItmNiv,  CAST(CASE WHEN c_SaldosActualesItems.ExistenciaTotal > 0 THEN 1 ELSE 0 END AS Bit) 
                         AS TieneExistencia, c_SaldosActualesItems.ExistenciaTotal
                    FROM ItmMae INNER JOIN
                         c_SaldosActualesItems ON ItmMae.Secuencial = c_SaldosActualesItems.Secuencial
                    WHERE(ItmMae.EmpCod = @EmpCod) AND(ItmMae.ItmTip = 'I') AND(ItmMae.ItmNiv = @nivel) AND(ItmMae.ItmCod LIKE @ItmCod)
                    ORDER BY ItmMae.ItmDsc",
                    new DbParameter[]
                    {
                        new SqlParameter("@EmpCod", sesion.Empresa.Codigo),
                         new SqlParameter("@Nivel", Nivel +1),
                         new SqlParameter("@ItmCod", Codigo + "%")
                    });

            datos.OnErrorThrow();

            datos.OnErrorThrow();
            var items = datos.resultado.CargarConjunto<GrupoInventario>();
            return items;

            //return from DataRow row in datos.resultado.Rows select row[0].ToInt();
        }




        public static IEnumerable< GrupoInventario> GetRoots(Sesion sesion )
        {
            var datosRoots =
                sesion.Data.CargarDatatable(
                    @"SELECT Secuencial, ItmCod, ItmDsc, ItmNiv, CAST(1 as Bit) AS TieneExistencia FROM ItmMae WHERE (EmpCod = @EmpCod) AND (ItmTip = 'G') AND (ItmNiv = 1) ORDER BY ItmDsc",
                    new DbParameter[]
                    {
                        new SqlParameter("@EmpCod", sesion.Empresa.Codigo),
                    });
            datosRoots.OnErrorThrow();
            var roots = datosRoots.resultado.CargarConjunto<GrupoInventario>();
            return roots;
        }





    }
}
