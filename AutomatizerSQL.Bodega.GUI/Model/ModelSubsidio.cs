using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;

namespace AutomatizerSQL.Bodega.GUI.Model
{
     [POCOViewModel(ImplementIDataErrorInfo = true)]
    public class ModelSubsidio
    {


        public static void BuildMetadata(MetadataBuilder<ModelSubsidio> builder)
        {
            builder.Property(x => x.FechaInicioSubsidio)
                .Required(() => "Debe de ingresar la fecha de inicio.");
                //.MatchesInstanceRule((inicio, subsidio) => inicio < subsidio.FechaFinSubsidio, () => "La fecha de inicio debe de ser menor a la fecha fin.");

            builder.Property(x => x.FechaFinSubsidio)
                .Required(() => "Debe de ingresar la fecha Fin.")
                .MatchesInstanceRule((fin, subsidio) => fin > subsidio.FechaInicioSubsidio,
                    () => "La fecha de inicio debe de ser menor a la fecha fin.");

            builder.Property(x => x.ValorSubsidio)
                .Required(() => "Debe de ingresar el valor del ModelSubsidio")
                .MatchesRule((valor => valor > 0));
        }


        public virtual int Id { get; set; }
        public virtual DateTime? FechaInicioSubsidio { get; set; }
        public virtual DateTime? FechaFinSubsidio { get; set; }

        public virtual decimal? ValorSubsidio { get; set; }
    }
}
