   M S B A M L   `   `   `  ����T  PAutomatizerSQL.Bodega.GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulltPclr-namespace:AutomatizerSQL.Bodega.GUI.Views;assembly=AutomatizerSQL.Bodega.GUIAutomatizerSQL.Bodega.GUI.Views  5      G CPruebaBodega, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PruebaBodegaPruebaBodega 5   K      PruebaBodega.MainWindow   5      ZViewsPclr-namespace:AutomatizerSQL.Bodega.GUI.Views;assembly=AutomatizerSQL.Bodega.GUI  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35a ]DevExpress.Xpf.Core.v16.1, Version=16.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  5   	   c _DevExpress.Xpf.Ribbon.v16.1, Version=16.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxr4http://schemas.devexpress.com/winfx/2008/xaml/ribbon  5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	   ] YDevExpress.Mvvm.v16.1, Version=16.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aAdxmvvm2http://schemas.devexpress.com/winfx/2008/xaml/mvvm   5   	   (localclr-namespace:PruebaBodega   6K     � Title$  
MainWindow��5	   	   $	��450��6   $	��725��6)   2  ,DevExpress.Mvvm.UI.Interactivity.Interaction   	Behaviors 5
      '  !DevExpress.Mvvm.UI.EventToCommand  5   
   �  �DevExpress.Mvvm.UI.Interactivity.EventTriggerBase`1[[System.Windows.DependencyObject, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]   	EventName$ Loaded��6    +  %DevExpress.Mvvm.UI.EventToCommandBase   Command 63   �� *IniciarSubsidiosEditorCommand+6g   -  'DevExpress.Xpf.Core.DXMessageBoxService  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   )  #DevExpress.Xpf.Ribbon.RibbonControl  $��0��6      RibbonStyle$ 
Office2010=�6)      ToolbarItems 5      +  %DevExpress.Xpf.Bars.BarButtonItemLink  5      %  DevExpress.Xpf.Bars.BarItemLink   BarItemName$ newItem��6(      
Categories. 5      5	  /DevExpress.Xpf.Ribbon.RibbonDefaultPageCategory	  2
  ,DevExpress.Xpf.Ribbon.RibbonPageCategoryBase	 
  Pages.	 5      &   DevExpress.Xpf.Ribbon.RibbonPage  
   Caption)  #DevExpress.Xpf.Core.ObjectConverter$

 Home 6!      Groups. 5      +  %DevExpress.Xpf.Ribbon.RibbonPageGroup     Caption$
 File��6*   5      5       5!      5"      5   /AutomatizerSQL.Bodega.GUI.Views.SubsidiosEditor  5$   
   -   5        $��SubsidiosEditor��5&      $��1��5%       � DataContext 5'      �� *SubsidiosEditorViewModel+ �� UpdateSourceTrigger$ PropertyChanged=�2  ,DevExpress.Xpf.Ribbon.RibbonStatusBarControl  5(   
   $��2��6%   5*      5+      5,      