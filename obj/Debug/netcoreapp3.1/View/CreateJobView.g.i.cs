﻿#pragma checksum "..\..\..\..\View\CreateJobView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0053CB8EF50E90E5749949746EA38D5FD0C2AAF3"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using EasySave;
using EasySave.Resources;
using MahApps.Metro.IconPacks;
using MahApps.Metro.IconPacks.Converter;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EasySave {
    
    
    /// <summary>
    /// CreateJobView
    /// </summary>
    public partial class CreateJobView : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 89 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox label;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSelectSourcePath;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxSourcePath;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSelectDestinationPath;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxDestinationPath;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox type;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\View\CreateJobView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnValid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.13.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EasySave;V1.0.0.0;component/view/createjobview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\View\CreateJobView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.13.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.label = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.btnSelectSourcePath = ((System.Windows.Controls.Button)(target));
            
            #line 91 "..\..\..\..\View\CreateJobView.xaml"
            this.btnSelectSourcePath.Click += new System.Windows.RoutedEventHandler(this.btnSelectSourcePath_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtBoxSourcePath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.btnSelectDestinationPath = ((System.Windows.Controls.Button)(target));
            
            #line 95 "..\..\..\..\View\CreateJobView.xaml"
            this.btnSelectDestinationPath.Click += new System.Windows.RoutedEventHandler(this.btnSelectDestinationPath_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtBoxDestinationPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.type = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 106 "..\..\..\..\View\CreateJobView.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnValid = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

