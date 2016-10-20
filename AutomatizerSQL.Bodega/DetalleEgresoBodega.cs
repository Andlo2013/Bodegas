using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Core;
using AutomatizerSQL.Data;

namespace AutomatizerSQL.Bodega
{
    public class DetalleEgresoBodega:IDetalle, INotifyPropertyChanged
    {
        private Item _item;
        private byte _secuencial;
        private Unidad _unidad;
        private decimal _cantidad;
        private decimal _costo;
        private Core.Bodega _bodega;
        private short _orden;
        private decimal _total;
        private decimal _cantidadPedida;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(IDetalle other)
        {
            throw new NotImplementedException();
        }
        [Column("ItmCod")]
        public Item Item
        {
            get { return _item; }
            set { _item = value; }
        }

        [Column("ISec")]
        public byte Secuencial
        {
            get { return _secuencial; }
            set { _secuencial = value; }
        }

        [Column("ItmUD")]
        public Unidad Unidad
        {
            get { return _unidad; }
            set { _unidad = value; }
        }

        [Column("EgrCanPed")]
        public decimal CantidadPedida
        {
            get { return _cantidadPedida; }
            set { _cantidadPedida = value; }
        }


        [Column("EgrCanEnt")]
        public decimal Cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }


        [Column("EgrCosUni")]
        public decimal Costo
        {
            get { return _costo; }
            set { _costo = value; }
        }

        
        [Column("EgrDetObs")]
        public string Observaciones { get; set; }

        [Column("BodCod")]
        public Core.Bodega Bodega
        {
            get { return _bodega; }
            set { _bodega = value; }
        }

        public short Orden
        {
            get { return _orden; }
            set { _orden = value; }
        }

        public decimal Total => Cantidad* Costo;


        [Column("PrecioUnit" ), DataParseParameter( DataParseParameter.TiposConvertidores.ToDecimal )]
        public decimal PrecioUnitario { get; set; }



    }
}
