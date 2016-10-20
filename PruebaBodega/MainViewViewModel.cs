using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutomatizerSQL.Bodega.GUI.ViewModels;
using AutomatizerSQL.Core;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace PruebaBodega
{
    [POCOViewModel]
    public class MainViewViewModel
    {

        public Sesion Sesion { get; set; }

        public MainViewViewModel() { }

        public virtual TreeInventarioViewModel TreeInventarioViewModel { get; set; }


        public MainViewViewModel(Sesion sesion )
        {
            Sesion = sesion;
        }

        public async Task IniciarSubsidiosEditor()
        {
            var factory = ViewModelSource.Factory((Sesion s) => new SubsidiosEditorViewModel(s));
            var viewmodel = factory(Sesion);

            SubsidiosEditorViewModel = viewmodel;
            await SubsidiosEditorViewModel.Work();
        }

        public virtual SubsidiosEditorViewModel SubsidiosEditorViewModel { get; set; }


        //var factory = ViewModelSource.Factory((Sesion s) => new TreeInventarioViewModel(s));
        //var viewmodel = factory(sesion);


        //public void MostrarTree()
        //{
        //    var factory = ViewModelSource.Factory((Sesion s) => new TreeInventarioViewModel(s));
        //    var viewmodel = factory(Sesion);

        //    TreeInventarioViewModel = viewmodel;
        //}


        //public bool CanMostrarTree()
        //{
        //    return TreeInventarioViewModel == null;
        //}


        //public async Task IniciarTree()
        //{
        //    await TreeInventarioViewModel.CargarGruposPadres();
        //}


        //public bool CanIniciarTree()
        //{
        //    return !CanMostrarTree();
        //}


    }
}
