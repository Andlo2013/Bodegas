using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Runtime.CompilerServices;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Data;
using AutomatizerSQL.Data.SqlClient;

namespace AutomatizerSQL.Bodega
{
    public sealed class CodificacionInventario:INotifyPropertyChanged
    {
        private byte _numeroNiveles;
        private byte _maximaLongitudCodigoNiveles;
        private IEnumerable<DetalleCodificacionInventario> _detalleCodificacion;

        [Column("NumNiv")]
        public byte NumeroNiveles
        {
            get { return _numeroNiveles; }
            set
            {
                if (value == _numeroNiveles) return;
                _numeroNiveles = value;
                OnPropertyChanged();
            }
        }

        [Column("LonNiv")]
        public byte MaximaLongitudCodigoNiveles
        {
            get { return _maximaLongitudCodigoNiveles; }
            set
            {
                if (value == _maximaLongitudCodigoNiveles) return;
                _maximaLongitudCodigoNiveles = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<DetalleCodificacionInventario> DetalleCodificacion
        {
            get { return _detalleCodificacion; }
            set
            {
                if (Equals(value, _detalleCodificacion)) return;
                _detalleCodificacion = value;
                OnPropertyChanged();
            }
        }

        public CodificacionInventario(Sesion sesion)
        {
            var datos = sesion.Data.CargarDatatable("SELECT NumNiv, LonNiv FROM CodItm Where (EmpCod =@EmpCod)",
                new DbParameter[]
                {
                   sesion.Empresa.ToSqlParameter() 
                });
            datos.OnErrorThrow();

            datos.resultado.Cargar(this);

            datos =
                sesion.Data.CargarDatatable(
                    @"SELECT NivNro, NivLon, NivDsc FROM CodItmDet WHERE (EmpCod = @EmpCod) ORDER BY NivNro",
                    new DbParameter[]
                    {
                        sesion.Empresa.ToSqlParameter()
                    });

            datos.OnErrorThrow();

            var detalleConfiguracion = datos.resultado.CargarConjunto<DetalleCodificacionInventario>();
            DetalleCodificacion = detalleConfiguracion;
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
