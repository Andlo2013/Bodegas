using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutomatizerSQL.Bodega.GUI.Common;
using AutomatizerSQL.Bodega.GUI.Model;
using AutomatizerSQL.Core;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace AutomatizerSQL.Bodega.GUI.ViewModels
{
    [POCOViewModel]
    public class TreeInventarioViewModel
    {
        public event SelectedItemChangedEventHandler SelectedNodeChanged;

        private readonly Sesion _sesion;
        private byte[] _nodeGroupImgBytes;
        private byte[] _nodeItemImgBytesExistencia;
        private byte[] _nodeItemImgBytesNoExistencia;

        public virtual bool IsLoading { get; set; }



        public virtual bool IsReady { get; set; }


        //public virtual ObservableCollection<NodoInventario> NodosInventario { get; set; }
        //public virtual FastObservableCollection NodosInventario { get; set; }

        public virtual FastObservableCollection NodosInventario { get; set; }

        public bool IsButtonVisible { get; set; } = true;


        public virtual NodoInventario SelectedNode { get; set; }

        public TreeInventarioViewModel()
        {
        }

        public TreeInventarioViewModel(Sesion sesion)
        {
            _sesion = sesion.Clone(false);
            IsLoading = false;
            NodosInventario = new FastObservableCollection();
            _nodeGroupImgBytes = Utilidades.Helper.LeerArchivoBinario(new FileInfo(@"AutomatizerSQL.Core.Wpf.Bodega.Icons\MaestroItems\GroupNode.png"));
            _nodeItemImgBytesExistencia = Utilidades.Helper.LeerArchivoBinario(new FileInfo(@"AutomatizerSQL.Core.Wpf.Bodega.Icons\MaestroItems\ItemNodeExistencia.png"));
            _nodeItemImgBytesNoExistencia = Utilidades.Helper.LeerArchivoBinario(new FileInfo(@"AutomatizerSQL.Core.Wpf.Bodega.Icons\MaestroItems\ItemNodeNoExistencia.png"));
        }


        public void TreeSelectedItemChanged(object args)
        {
            IsLoading = true;
            var eventArgs = args as FocusedRowChangedEventArgs;
            var nodo = eventArgs.NewRow as NodoInventario;
            if (nodo.Nombre == "%$&&%$!") return;

            var previous = SelectedNode;
            SelectedNode = nodo;
            SelectedNodeChanged?.Invoke(this, new SelectedItemChangedEventArgs(eventArgs.Source.DataControl, previous, nodo));
            IsLoading = false;
        }


        public async Task TreeNodeExpanding(object args)
        {
            IsLoading = true;
            var eventArgs =
                args as TreeListNodeAllowEventArgs;
            var nodo = eventArgs.Row as NodoInventario;
            await Task.Delay(100);
            var childsGroup = await Task.Run(() => nodo.Grupo.GetChildGroups(_sesion));
            var factory =
                ViewModelSource.Factory(
                    (int id, int idPadre, string nombre, bool esGrupo) =>
                        new NodoInventario(id, idPadre, nombre, esGrupo));

            var treeList = eventArgs.OriginalSource as TreeListView;
            treeList.DataControl.BeginDataUpdate();
            NodosInventario.BeginUpdate();
            foreach (GrupoInventario grupoInventario in childsGroup)
            {
                if (NodosInventario.Any(n => n.Id == grupoInventario.Id)) continue;

                var childNode = factory(grupoInventario.Id, nodo.Id, grupoInventario.Nombre, true);
                childNode.Grupo = grupoInventario;
                childNode.NodeImage = _nodeGroupImgBytes;
                NodosInventario.Add(childNode);


                var nodoNada = factory(grupoInventario.Id + 1000000, grupoInventario.Id, "%$&&%$!", true);
                NodosInventario.Add(nodoNada);
            }
            var childItems = await Task.Run(() => nodo.Grupo.GetItemsAsGroupInfoForNodes(_sesion));

            foreach (GrupoInventario grupoInventario in childItems)
            {
                if (NodosInventario.Any(n => n.Id == grupoInventario.Id)) continue;

                var childNode = factory(grupoInventario.Id, nodo.Id, grupoInventario.Nombre, false);
                childNode.Grupo = grupoInventario;
                childNode.NodeImage = grupoInventario.TieneExistencia
                    ? _nodeItemImgBytesExistencia
                    : _nodeItemImgBytesNoExistencia;
                NodosInventario.Add(childNode);
            }




            NodosInventario.EndUpdate();
            treeList.DataControl.EndDataUpdate();

            var nodoInventario = NodosInventario.FirstOrDefault(n => n.Id == nodo.Id + 1000000);
            if (nodoInventario != null) NodosInventario.Remove(nodoInventario);
            IsLoading = false;
        }



        public async Task CargarGruposPadres()
        {
            try
            {
                IsLoading = true;
                await Task.Delay(1000);
                var roots = await Task.Run(() => GrupoInventario.GetRoots(_sesion));
                var factory = ViewModelSource.Factory((int id, int idPadre, string nombre, bool esGrupo) => new NodoInventario(id, idPadre, nombre, esGrupo));

                foreach (GrupoInventario grupoInventario in roots)
                {
                    var nodo = factory(grupoInventario.Id, 0, grupoInventario.Nombre, true); //new NodoInventario(grupoInventario.Id, 0, grupoInventario.Nombre, true);
                    nodo.Grupo = grupoInventario;
                    nodo.NodeImage = _nodeGroupImgBytes;
                    NodosInventario.Add(nodo);

                    var nodoNada = factory(grupoInventario.Id + 1000000, grupoInventario.Id, "%$&&%$!", true); //new NodoInventario(grupoInventario.Id, 0, grupoInventario.Nombre, true);
                    NodosInventario.Add(nodoNada);
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
            }

        }







    }
}