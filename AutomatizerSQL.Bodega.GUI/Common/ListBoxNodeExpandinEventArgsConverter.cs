using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid.TreeList;

namespace AutomatizerSQL.Bodega.GUI.Common
{
    public class ListBoxNodeExpandinEventArgsConverter:EventArgsConverterBase<TreeListNodeAllowEventArgs>
    {
        protected override object Convert(object sender, TreeListNodeAllowEventArgs args)
        {
            return args.Row;
            //throw new NotImplementedException();
        }
    }
}
