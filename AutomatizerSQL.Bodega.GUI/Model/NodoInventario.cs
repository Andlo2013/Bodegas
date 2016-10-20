using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatizerSQL.Core;
using DevExpress.Mvvm.DataAnnotations;

namespace AutomatizerSQL.Bodega.GUI.Model
{

    [POCOViewModel]
    public class NodoInventario
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        public virtual bool IsGroup { get; set; }

        public virtual string Nombre { get; set; }

        public virtual bool ExpandState { get; set; }

        public virtual bool TieneExistencia { get; set; }
        public virtual GrupoInventario Grupo { get; set; }

        public byte[] NodeImage { get; set; }


        public NodoInventario(int id, int idPadre  ,string nombre, bool isGroup  )
        {
            Id = id;
            ParentId = idPadre;
            Nombre = nombre;
            IsGroup = isGroup;
            ExpandState = false;
        }






       



    }
}
