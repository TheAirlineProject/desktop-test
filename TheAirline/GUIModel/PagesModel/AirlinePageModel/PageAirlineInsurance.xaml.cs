﻿namespace TheAirline.GUIModel.PagesModel.AirlinePageModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;
    using TheAirline.Model.AirlineModel;
    using TheAirline.Model.AirlinerModel;
    using TheAirline.Model.GeneralModel;
    using TheAirline.Model.GeneralModel.Helpers;

    /// <summary>
    ///     Interaction logic for PageAirlineInsurance.xaml
    /// </summary>
    public partial class PageAirlineInsurance : Page
    {
        #region Constructors and Destructors

        public PageAirlineInsurance(AirlineMVVM airline)
        {
            this.Airline = airline;
            this.DataContext = this.Airline;
            this.AvailableCenters = new ObservableCollection<MaintenanceCenter>();

            foreach (MaintenanceCenter center in MaintenanceCenters.GetCenters())
                if (!this.Airline.MaintenanceCenters.Contains(center))
                    this.AvailableCenters.Add(center);

            this.InitializeComponent();

            this.Loaded += this.PageAirlineInsurance_Loaded;
        }

        #endregion

        #region Public Properties

        public AirlineMVVM Airline { get; set; }

        public ObservableCollection<MaintenanceCenter> AvailableCenters { get; set; }

        #endregion

        #region Methods

        private void PageAirlineInsurance_Loaded(object sender, RoutedEventArgs e)
        {
            this.setValues();
        }

        //sets the values

        private void btnCreateInsurance_Click(object sender, RoutedEventArgs e)
        {
            var type = (AirlineInsurance.InsuranceType)this.cbType.SelectedItem;
            var scope = (AirlineInsurance.InsuranceScope)this.cbScope.SelectedItem;
            var terms = (AirlineInsurance.PaymentTerms)this.cbTerms.SelectedItem;
            Boolean allAirliners = this.cbAllAirliners.IsChecked.Value;
            int lenght = Convert.ToInt16(this.cbLength.SelectedItem);
            int amount = Convert.ToInt32(this.slAmount.Value);

            AirlineInsurance insurance = AirlineInsuranceHelpers.CreatePolicy(
                this.Airline.Airline,
                type,
                scope,
                terms,
                allAirliners,
                lenght,
                amount);
        }
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            WPFMessageBoxResult result =
                       WPFMessageBox.Show(
                           Translator.GetInstance().GetString("MessageBox", "2133"),
                           string.Format(Translator.GetInstance().GetString("MessageBox", "2133", "message")),
                           WPFMessageBoxButtons.YesNo);

            if (result == WPFMessageBoxResult.Yes)
            {
                this.Airline.setMaintenance();
            }
        }

        private void btnApplyAll_Click(object sender, RoutedEventArgs e)
        {
            WPFMessageBoxResult result =
                        WPFMessageBox.Show(
                            Translator.GetInstance().GetString("MessageBox", "2134"),
                            string.Format(Translator.GetInstance().GetString("MessageBox", "2134", "message")),
                            WPFMessageBoxButtons.YesNo);

            if (result == WPFMessageBoxResult.Yes)
            {
                this.Airline.setMaintenance();

                foreach (FleetAirliner airliner in this.Airline.Airline.Fleet)
                {
                    foreach (AirlineMaintenanceMVVM maintenance in this.Airline.Maintenances)
                    {
                        var m = airliner.Maintenance.Checks.Find(f => f.Type == maintenance.Type);

                        if (m != null)
                        {
                            if (m.CheckCenter == null)
                            {
                                m.CheckCenter = new AirlinerMaintenanceCenter(maintenance.Type);
                                if (maintenance.SelectedType.Airport == null)
                                    m.CheckCenter.Center = maintenance.SelectedType.Center;
                                else
                                    m.CheckCenter.Airport = maintenance.SelectedType.Airport;
                            }
                            else
                                if (maintenance.SelectedType.Airport == null)
                                {

                                    m.CheckCenter.Airport = null;
                                    m.CheckCenter.Center = maintenance.SelectedType.Center;
                                }
                                else
                                {
                                    m.CheckCenter.Center = null;
                                    m.CheckCenter.Airport = maintenance.SelectedType.Airport;
                                }
                        }
                        else
                        {
                            string s;
                        }


                    }
                }
            }
        }
        private void btnSetAdvertisement_Click(object sender, RoutedEventArgs e)
        {
            this.Airline.saveAdvertisements();
        }
        private void btnAddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            MaintenanceCenter center = (MaintenanceCenter)((Button)sender).Tag;

            WPFMessageBoxResult result =
                        WPFMessageBox.Show(
                            Translator.GetInstance().GetString("MessageBox", "2131"),
                            string.Format(Translator.GetInstance().GetString("MessageBox", "2131", "message"), center.Name),
                            WPFMessageBoxButtons.YesNo);

            if (result == WPFMessageBoxResult.Yes)
            {

                this.Airline.addMaintenanceCenter(center);
                this.AvailableCenters.Remove(center);
            }
        }
        private void btnDeleteMaintenance_Click(object sender, RoutedEventArgs e)
        {
            MaintenanceCenter center = (MaintenanceCenter)((Button)sender).Tag;

            Boolean inUse = false;

            foreach (FleetAirliner airliner in this.Airline.Airline.Fleet)
            {
                if (airliner.Maintenance.Checks.Exists(c => c.CheckCenter != null && c.CheckCenter.Center != null && c.CheckCenter.Center == center))
                    inUse = true;
            }

            if (!inUse)
            {
                foreach (AirlinerMaintenanceType maintenanceType in AirlinerMaintenanceTypes.GetMaintenanceTypes())
                {
                    if (this.Airline.Airline.Maintenances.ContainsKey(maintenanceType))
                    {
                        var maintenance = this.Airline.Airline.Maintenances[maintenanceType];

                        if (maintenance.Center != null && maintenance.Center == center)
                            inUse = true;
                    }
                }
            }
            if (inUse)
            {
                WPFMessageBox.Show(
                                 Translator.GetInstance().GetString("MessageBox", "2135"),
                                 string.Format(Translator.GetInstance().GetString("MessageBox", "2135", "message"), center.Name),
                                 WPFMessageBoxButtons.Ok);
            }
            else
            {
                WPFMessageBoxResult result =
                             WPFMessageBox.Show(
                                 Translator.GetInstance().GetString("MessageBox", "2132"),
                                 string.Format(Translator.GetInstance().GetString("MessageBox", "2132", "message"), center.Name),
                                 WPFMessageBoxButtons.YesNo);

                if (result == WPFMessageBoxResult.Yes)
                {
                    this.Airline.removeMaintenanceCenter(center);
                    this.AvailableCenters.Add(center);
                }
            }
        }

        private void setValues()
        {
            this.cbType.Items.Clear();
            foreach (AirlineInsurance.InsuranceType type in Enum.GetValues(typeof(AirlineInsurance.InsuranceType)))
            {
                this.cbType.Items.Add(type);
            }
            this.cbType.SelectedIndex = 0;

            this.cbTerms.Items.Clear();
            foreach (AirlineInsurance.PaymentTerms term in Enum.GetValues(typeof(AirlineInsurance.PaymentTerms)))
            {
                this.cbTerms.Items.Add(term);
            }
            this.cbTerms.SelectedIndex = 0;

            this.cbScope.Items.Clear();
            foreach (AirlineInsurance.InsuranceScope scope in Enum.GetValues(typeof(AirlineInsurance.InsuranceScope)))
            {
                this.cbScope.Items.Add(scope);
            }
            this.cbScope.SelectedIndex = 0;

            this.cbLength.Items.Clear();
            for (int i = 0; i < 20; i++)
            {
                this.cbLength.Items.Add(i + 1);
            }
            this.cbLength.SelectedIndex = 0;

            foreach (AirlineAdvertisementMVVM advertisement in this.Airline.Advertisements)
            {
                advertisement.SelectedType = this.Airline.Airline.getAirlineAdvertisement(advertisement.Type);
            }

            foreach (AirlinerMaintenanceType maintenanceType in AirlinerMaintenanceTypes.GetMaintenanceTypes())
            {
                var maintenance = this.Airline.Maintenances.Find(m => m.Type == maintenanceType);

                if (this.Airline.Airline.Maintenances.ContainsKey(maintenanceType))
                {

                    maintenance.SelectedType = maintenance.getCenterFromList(this.Airline.Airline.Maintenances[maintenanceType]);

                    var contains = maintenance.Centers.Contains(maintenance.SelectedType);
                }
                else
                    maintenance.SelectedType = null;
            }
        }
        private void cbMaintenance_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            AirlineMaintenanceMVVM o = (AirlineMaintenanceMVVM)cb.Tag;

            if (cb.Items.Contains(o.SelectedType))
            {
                cb.SelectedItem = o.SelectedType;
            }
        }
        #endregion




    }
}