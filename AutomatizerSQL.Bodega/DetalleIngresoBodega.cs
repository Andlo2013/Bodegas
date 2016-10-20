using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutomatizerSQL.Core;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Data;

namespace AutomatizerSQL.Bodega
{
    [DebuggerDisplay("Item = {Item.Nombre} Cantidad = {Cantidad} Costo = {Costo} Total = {Cantidad * Costo}")]
    public sealed class DetalleIngresoBodega:IDetalle, INotifyPropertyChanged
    {
        private Item _item;
        private byte _secuencial;
        private Unidad _unidad;
        private decimal _cantidad;
        private decimal _costo;
        private Core.Bodega _bodega;
        private short _orden;
        private string _observaciones;

        public bool Equals(IDetalle other)
        {
            return Item.ID == other.Item.ID && Secuencial == other.Secuencial;
        }


        [Column("ItmCod")]
        public Item Item
        {
            get { return _item; }
            set
            {
                if (Equals(value, _item)) return;
                _item = value;
                OnPropertyChanged();
            }
        }

        [Column("ISec")]
        public byte Secuencial
        {
            get { return _secuencial; }
            set
            {
                if (value == _secuencial) return;
                _secuencial = value;
                OnPropertyChanged();
            }
        }

        [Column("ItmUA")]
        public Unidad Unidad
        {
            get { return _unidad; }
            set
            {
                if (Equals(value, _unidad)) return;
                _unidad = value;
                OnPropertyChanged();
            }
        }

        [Column("IngCan"), DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)  ]
        public decimal Cantidad
        {
            get { return _cantidad; }
            set
            {
                if (value == _cantidad) return;
                _cantidad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }


        


        [Column("IngCosUni"), DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal Costo
        {
            get { return _costo; }
            set
            {
                if (value == _costo) return;
                _costo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        [Column("BodCod")]
        public Core.Bodega Bodega
        {
            get { return _bodega; }
            set
            {
                if (Equals(value, _bodega)) return;
                _bodega = value;
                OnPropertyChanged();
            }
        }

        public short Orden
        {
            get { return _orden; }
            set
            {
                if (value == _orden) return;
                _orden = value;
                OnPropertyChanged();
            }
        }

        [Column("FacCanReg"), DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal FacturacionCantidadRequerida { get; set; }


        [Column("FacCan"), DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal CantidadFactura { get; set; }


        [Column("FacCosUni"), DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal FacturaCostoUnitario { get; set; }


        public decimal Total => Cantidad*Costo;


        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal IngCosEst { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal IngCosPro { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]
        public decimal IngCosUlt { get; set; }
        public short ModCodIng { get; set; }
        public short ModCodCom { get; set; }

        [Column("IngObsDet")]
        public string Observaciones
        {
            get { return _observaciones; }
            set
            {
                if (value == _observaciones) return;
                _observaciones = value;
                OnPropertyChanged();
            }
        }

        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal Conversion { get; set; }

        public string ItmTip { get; set; }

        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal TotDes { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal TotRec { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal PesoUnitario { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal   PesoTotal { get; set; }
        [DataParseParameter(DataParseParameter.TiposConvertidores.ToDecimal)]

        public decimal TotImp { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public DetalleIngresoBodega()
        {
            Observaciones = String.Empty;
        }
    }
}
