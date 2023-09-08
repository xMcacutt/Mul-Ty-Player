using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using Riptide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class KoalaSelectViewModel : IViewModel
    {
        public KoalaSelectEntryViewModel Boonie { get; set; }
        public KoalaSelectEntryViewModel Mim { get; set; }
        public KoalaSelectEntryViewModel Gummy { get; set; }
        public KoalaSelectEntryViewModel Snugs { get; set; }
        public KoalaSelectEntryViewModel Katie { get; set; }
        public KoalaSelectEntryViewModel Kiki { get; set; }
        public KoalaSelectEntryViewModel Elizabeth { get; set; }
        public KoalaSelectEntryViewModel Dubbo { get; set; }

        public bool BlockKoalaSelect { get; set; }

        KoalaSelectModel model;

        public KoalaSelectViewModel()
        {
            model = ModelController.KoalaSelect;
            model.MakeAllAvailable();
            Boonie = new (model.Boonie);
            Mim = new (model.Mim);
            Gummy = new (model.Gummy);
            Snugs = new (model.Snugs);
            Katie = new (model.Katie);
            Kiki = new (model.Kiki);
            Elizabeth = new (model.Elizabeth);
            Dubbo = new (model.Dubbo);
        }

        public void OnEntered()
        {
        }

        public void OnExited()
        {
        }
    }
}
