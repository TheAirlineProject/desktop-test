﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using TheAirline.Infrastructure;
using WPFLocalizeExtension.Engine;

namespace TheAirline.ViewModels.Game
{
    [Export]
    public class PageSelectLanguageViewModel : BindableBase
    {
        private readonly LocalizeDictionary _dictionary = LocalizeDictionary.Instance;
        private readonly IRegionManager _regionManager;
        private readonly AppState _state;

        [ImportingConstructor]
        public PageSelectLanguageViewModel(IRegionManager regionManager, AppState state)
        {
            _state = state;
            _regionManager = regionManager;

            SelectLanguage = new DelegateCommand<string>(SetLanguage);
        }

        private void SetLanguage(string languageName)
        {
            _state.Language = languageName;
            _dictionary.SetCultureCommand.Execute(languageName);
            _regionManager.RequestNavigate("MainContentRegion", new Uri("/PageStartMenu", UriKind.Relative));
        }

        public ObservableCollection<CultureInfo> Languages => _dictionary.MergedAvailableCultures;
        public DelegateCommand<string> SelectLanguage { get; }
    }
}
