using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using AutomatizerSQL.Bodega.GUI.Model;
using AutomatizerSQL.Core;
using AutomatizerSQL.Core.HelperClass;
using AutomatizerSQL.Utilidades;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace AutomatizerSQL.Bodega.GUI.ViewModels
{
    [POCOViewModel]
    public class SubsidiosEditorViewModel
    {
        public IMessageBoxService MessageBoxService => this.GetService<IMessageBoxService>();

        private Sesion _sesion;

        public virtual TreeInventarioViewModel TreeInventarioViewModel { get; set; }


        public SubsidiosEditorViewModel()
        {
        }

        public SubsidiosEditorViewModel(Sesion sesion)
        {
            _sesion = sesion;
        }



        public async Task Work()
        {
            var factory = ViewModelSource.Factory((Sesion s) => new TreeInventarioViewModel(s));
            var viewmodel = factory(_sesion);
            TreeInventarioViewModel = viewmodel;
            TreeInventarioViewModel.SelectedNodeChanged += TreeInventarioViewModel_SelectedNodeChanged;

            _timerCargarSubsidios = new Timer(750);
            _timerCargarSubsidios.AutoReset = false;
            _timerCargarSubsidios.Elapsed += _timerCargarSubsidios_Elapsed;

            await TreeInventarioViewModel.CargarGruposPadres();

        }

        private void _timerCargarSubsidios_Elapsed(object sender, ElapsedEventArgs e)
        {
            CargarSubsidios();
        }

        private void CargarSubsidios()
        {
            var subsidiosData =
                _sesion.Data.CargarDatatable(@"SELECT  IdRango, RangoFechaIni, RangoFechaFin, Subsidio
                FROM ItmMae_Subsidios
                WHERE (Secuencial = @Secuencial)
                ORDER BY RangoFechaFin", new DbParameter[]
                {
                    new SqlParameter("Secuencial", _selectedNode.Id)
                });


            var subsidios = (from DataRow row in subsidiosData.resultado.Rows
                select
                new ModelSubsidio
                {
                    Id = row["IdRango"].ToInt(),
                    FechaInicioSubsidio = row["RangoFechaIni"].ToDateTime(),
                    FechaFinSubsidio = row["RangoFechaFin"].ToDateTime(),
                    ValorSubsidio = row["Subsidio"].ToDecimal()
                }).ToList();
            Subsidios = new ObservableCollection<ModelSubsidio>(subsidios);

        }


        public virtual ObservableCollection<ModelSubsidio> Subsidios { get; set; }

        public virtual string Nombre { get; set; }

        public virtual bool EsGrupo { get; set; }

        public virtual bool SubsidiosCargados { get; set; }

        private NodoInventario _selectedNode;

        private Timer _timerCargarSubsidios;

        private void TreeInventarioViewModel_SelectedNodeChanged(object sender,
            DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

            _selectedNode = (NodoInventario) e.NewItem;
            Nombre = _selectedNode.Nombre;
            EsGrupo = _selectedNode.IsGroup;
            _timerCargarSubsidios.Stop();
            Subsidios?.Clear();
            if (!_selectedNode.IsGroup) _timerCargarSubsidios.Start();
        }


        public async Task GuardarSubsidios()
        {
            try
            {
                _sesion.Data.AbrirConexion();

                _sesion.Data.IniciarTransaccion();
                //var cero = 0;
                //var r = 1/cero;

                _sesion.Data.Ejecutar("DELETE FROM ItmMae_Subsidios WHERE Secuencial =@Secuencial",
                    new DbParameter[]
                    {
                        new SqlParameter("Secuencial", _selectedNode.Id)

                    }).OnErrorThrow();
                int i = 1;
                foreach (ModelSubsidio subsidio in Subsidios.OrderBy(s => s.FechaInicioSubsidio))
                {
                    _sesion.Data.Ejecutar(@"INSERT INTO ItmMae_Subsidios (EmpCod, Secuencial, IdRango, RangoFechaIni, RangoFechaFin, Subsidio)
                                VALUES (@EmpCod, @Secuencial, @IdRango, @RangoFechaIni, @RangoFechaFin, @Subsidio)
                            ", new DbParameter[]
                    {
                        new SqlParameter("EmpCod", _sesion.Empresa.Codigo),
                        new SqlParameter("Secuencial", _selectedNode.Id),
                        new SqlParameter("IdRango", i),
                        new SqlParameter("RangoFechaIni", subsidio.FechaInicioSubsidio),
                        new SqlParameter("RangoFechaFin", subsidio.FechaFinSubsidio),
                        new SqlParameter("Subsidio", subsidio.ValorSubsidio)
                    }).OnErrorThrow();
                    i++;
                }

                _sesion.Data.ConfirmarTransaccion();
                MessageBoxService?.Show("Subsidios guardados correctamente", "Guardar", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _sesion.Data.RevertirTransaccion();
                if (MessageBoxService != null)
                {
                    var mensaje = "Error al guardar los datos. ";
                    mensaje += Environment.NewLine + "Mensaje: ";
                    mensaje += ex.Message;
                    mensaje += Environment.NewLine + "Stack Trace: ";
                    mensaje += ex.StackTrace;
                    MessageBoxService.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                _sesion.Data.CerrarConexion();
            }
        }



    }
}