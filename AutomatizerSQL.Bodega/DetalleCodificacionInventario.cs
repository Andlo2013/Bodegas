using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using AutomatizerSQL.Bodega.Annotations;

namespace AutomatizerSQL.Bodega
{
    public sealed class DetalleCodificacionInventario:INotifyPropertyChanged
    {
        private byte _numeroNivel;
        private byte _longitudCodigoNivel;
        private string _nombreDetalle;

        [Column("NivNro")]
        public byte NumeroNivel
        {
            get { return _numeroNivel; }
            set
            {
                if (value == _numeroNivel) return;
                _numeroNivel = value;
                OnPropertyChanged();
            }
        }

        [Column("NivLon")]
        public byte LongitudCodigoNivel
        {
            get { return _longitudCodigoNivel; }
            set
            {
                if (value == _longitudCodigoNivel) return;
                _longitudCodigoNivel = value;
                OnPropertyChanged();
            }
        }

        [Column("NivDsc")]
        public string NombreDetalle
        {
            get { return _nombreDetalle; }
            set
            {
                if (value == _nombreDetalle) return;
                _nombreDetalle = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
