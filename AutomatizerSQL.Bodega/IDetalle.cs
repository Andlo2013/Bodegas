using System;
using AutomatizerSQL.Core;

namespace AutomatizerSQL.Bodega
{
    public interface IDetalle:IEquatable<IDetalle>
    {
        Item Item { get; set; }

        byte Secuencial { get; set; }

        Unidad Unidad { get; set; }

        decimal Cantidad { get; set; }

        decimal Costo { get; set; }

        Core.Bodega Bodega { get; set; }

        short Orden { get; set; }

        decimal Total { get; }


        string Observaciones { get; set; }

    }

}
