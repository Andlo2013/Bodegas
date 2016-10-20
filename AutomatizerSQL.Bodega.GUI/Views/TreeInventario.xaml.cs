using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Grid.TreeList;

namespace AutomatizerSQL.Bodega.GUI.Views
{
    /// <summary>
    /// Interaction logic for TreeInventario.xaml
    /// </summary>
    public partial class TreeInventario : UserControl
    {
        public TreeInventario()
        {
            InitializeComponent();
        }

        private void TreeGrupos_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TreeGrupos.Visibility == Visibility.Visible)
            {
                TreeGrupos.Focus();
            }
        }


        //private void TreeListView_OnNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
