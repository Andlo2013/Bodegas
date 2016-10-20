using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;

namespace AutomatizerSQL.Bodega
{
    public class AsignacionBodega:INotifyPropertyChanged
    {
        private int _idItem;
        private Core.Bodega _bodega;
        private double _existencia;
        private double _existenciaMaxima;
        private double _existenciaMinima;
        private double _costo;
        private double _costoPromedio;
        private double _costoUltimo;
        private double _cantidadComprometida;
        private bool _asignado;

        [Column("Secuencial")]
        public int IdItem
        {
            get { return _idItem; }
            set
            {
                if (value == _idItem) return;
                _idItem = value;
                OnPropertyChanged();
            }
        }

        [Column("BodCod")]
        public AutomatizerSQL.Core.Bodega Bodega
        {
            get { return _bodega; }
            set
            {
                if (Equals(value, _bodega)) return;
                _bodega = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmExs")]
        public double Existencia
        {
            get { return _existencia; }
            set { _existencia = value; }
        }

        [Column("ItmExsMax")]
        public double ExistenciaMaxima
        {
            get { return _existenciaMaxima; }
            set
            {
                if (value == _existenciaMaxima) return;
                _existenciaMaxima = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmExsMin")]
        public double ExistenciaMinima
        {
            get { return _existenciaMinima; }
            set
            {
                if (value == _existenciaMinima) return;
                _existenciaMinima = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmCos")]
        public double Costo
        {
            get { return _costo; }
            set
            {
                if (value == _costo) return;
                _costo = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmCosPro")]
        public double CostoPromedio
        {
            get { return _costoPromedio; }
            set
            {
                if (value == _costoPromedio) return;
                _costoPromedio = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmCosUlt")]
        public double CostoUltimo
        {
            get { return _costoUltimo; }
            set
            {
                if (value == _costoUltimo) return;
                _costoUltimo = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmPedido")]
        public double CantidadComprometida
        {
            get { return _cantidadComprometida; }
            set
            {
                if (value == _cantidadComprometida) return;
                _cantidadComprometida = value;
                OnPropertyChanged();
            }
        }

        public bool Asignado
        {
            get { return _asignado; }
            set
            {
                if (value == _asignado) return;
                _asignado = value;
                OnPropertyChanged();
            }
        }

        public AsignacionBodega() { }


        public static IEnumerable<AsignacionBodega> ObtenerAsignacionesBodegaItem(int secuencial, Sesion sesion)
        {
            var datosAsignaciones =
                sesion.Data.CargarDatatable(
                    @"SELECT        CAST(1 AS bit) AS Asignado, ItmBod.BodCod, ItmBod.ItmExs, ItmBod.ItmExsMax, ItmBod.ItmExsMin, ItmBod.ItmCos, ItmBod.ItmCosPro, ItmBod.ItmCosUlt, ItmBod.ItmCosEst, ItmBod.ItmPedido, ItmBod.Ubicacion
                FROM ItmBod INNER JOIN Bodega ON ItmBod.EmpCod = Bodega.EmpCod AND ItmBod.BodCod = Bodega.BodCod
                WHERE (ItmBod.EmpCod = @EmpCod) AND (ItmBod.Secuencial = @Secuencial)
                UNION 
                SELECT  CAST(0 AS bit) AS Asignado, BodCod, 0 as ItmExs, 0 as ItmExsMax, 0 as ItmExsMin, 0.0 as ItmCos ,0.0 as ItmCosPro, 0  as ItmCosUlt, 0 as ItmCosEst, 0 as ItmPedido, '' as Ubicacion
                FROM ItmBod Where (EmpCod = @EmpCod) AND BodCod NOT IN (SELECT BodCod FROM ItmBod Where  (ItmBod.EmpCod = @EmpCod) AND (ItmBod.Secuencial = @Secuencial))",
                    new DbParameter[]
                    {
                        new SqlParameter("@EmpCod", sesion.Empresa.Codigo),
                        new SqlParameter("@Secuencial", secuencial)  
                    });
            datosAsignaciones.OnErrorThrow();
            var asignaciones = datosAsignaciones.resultado.CargarConjunto<AsignacionBodega>(sesion);
            

            //IEnumerable<AsignacionBodega> rEnumerable = new List<AsignacionBodega>(5);



            return asignaciones;
        }
        public static IEnumerable<AsignacionBodega> ObtenerAsignacionesBodegaItem(Item item, Sesion sesion)
        {
            return ObtenerAsignacionesBodegaItem(item.ID, sesion);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
