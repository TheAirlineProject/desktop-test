﻿using System;
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
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.Model.AirlineModel;
using TheAirline.GraphicsModel.PageModel.PageAirlineModel;
using TheAirline.Model.GeneralModel;
using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;

namespace TheAirline.GraphicsModel.PageModel.PageAlliancesModel.PanelAlliancesModel
{
    /// <summary>
    /// Interaction logic for PanelAlliance.xaml
    /// </summary>
    public partial class PanelAlliance : Page
    {
        private Alliance Alliance;
        private StandardPage ParentPage;
        public PanelAlliance(StandardPage parent, Alliance alliance)
        {
            this.ParentPage = parent;
            this.Alliance = alliance;

            InitializeComponent();

            ScrollViewer scroller = new ScrollViewer();
            scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.MaxHeight = GraphicsHelpers.GetContentHeight() - 50;
            scroller.Margin = new Thickness(0, 0, 50, 0);
            
            //header, join, type (codeshare

            StackPanel panelAlliance = new StackPanel();
            
            ContentControl txtHeader = new ContentControl();
            txtHeader.ContentTemplate = this.Resources["AirlinesHeader"] as DataTemplate;
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            panelAlliance.Children.Add(txtHeader);


            ListBox lbMembers = new ListBox();
            lbMembers.ItemTemplate = this.Resources["AirlineItem"] as DataTemplate;
            lbMembers.MaxHeight = GraphicsHelpers.GetContentHeight() - 75;
            lbMembers.ItemContainerStyleSelector = new ListBoxItemStyleSelector();

            List<Airline> airlines = this.Alliance.Members;
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));

            foreach (Airline airline in airlines)
                lbMembers.Items.Add(airline);

            panelAlliance.Children.Add(lbMembers);

            panelAlliance.Children.Add(createButtonsPanel());

            scroller.Content = panelAlliance;
            
            this.Content = scroller;
       
        }
        //creates the button panel
        private WrapPanel createButtonsPanel()
        {
            WrapPanel buttonsPanel = new WrapPanel();
            buttonsPanel.Margin = new Thickness(0, 5, 0, 0);

            Button btnJoin = new Button();
            btnJoin.Uid = "200";
            btnJoin.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnJoin.Height = Double.NaN;
            btnJoin.Width = Double.NaN;
            btnJoin.Content = Translator.GetInstance().GetString("PanelAlliance", btnJoin.Uid);
            btnJoin.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnJoin.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnJoin.Click += new RoutedEventHandler(btnJoin_Click);
            btnJoin.Visibility = this.Alliance.Members.Contains(GameObject.GetInstance().HumanAirline) ? Visibility.Collapsed : System.Windows.Visibility.Visible;

            buttonsPanel.Children.Add(btnJoin);

            Button btnInvite = new Button();
            btnInvite.Uid = "201";
            btnInvite.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnInvite.Height = Double.NaN;
            btnInvite.Width = Double.NaN;
            btnInvite.Content = Translator.GetInstance().GetString("PanelAlliance", btnInvite.Uid);
            btnInvite.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnInvite.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnInvite.Click += new RoutedEventHandler(btnInvite_Click);
            btnInvite.Visibility = this.Alliance.Members.Contains(GameObject.GetInstance().HumanAirline) ? Visibility.Visible : Visibility.Collapsed;
            btnInvite.IsEnabled = false;

            buttonsPanel.Children.Add(btnInvite);

            Button btnDelete = new Button();
            btnDelete.Uid = "202";
            btnDelete.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnDelete.Height = Double.NaN;
            btnDelete.Width = Double.NaN;
            btnDelete.Content = Translator.GetInstance().GetString("PanelAlliance", btnDelete.Uid);
            btnDelete.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnDelete.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnDelete.Margin = new Thickness(5, 0, 0, 0);
            btnDelete.Click += new RoutedEventHandler(btnDelete_Click);
            btnDelete.Visibility = this.Alliance.Members.Contains(GameObject.GetInstance().HumanAirline) && this.Alliance.Members.Count == 1 ? Visibility.Visible : Visibility.Collapsed;

            buttonsPanel.Children.Add(btnDelete);

            Button btnExit = new Button();
            btnExit.Uid = "203";
            btnExit.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnExit.Height = Double.NaN;
            btnExit.Width = Double.NaN;
            btnExit.Content = Translator.GetInstance().GetString("PanelAlliance", btnExit.Uid);
            btnExit.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnExit.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnExit.Margin = new Thickness(5, 0, 0, 0);
            btnExit.Visibility = this.Alliance.Members.Contains(GameObject.GetInstance().HumanAirline) && this.Alliance.Members.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            btnExit.Click += new RoutedEventHandler(btnExit_Click);

            buttonsPanel.Children.Add(btnExit);



            return buttonsPanel;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
             WPFMessageBoxResult result = WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2602"), string.Format(Translator.GetInstance().GetString("MessageBox", "2602", "message"), this.Alliance.Name), WPFMessageBoxButtons.YesNo);

             if (result == WPFMessageBoxResult.Yes)
             {
                 this.Alliance.removeMember(GameObject.GetInstance().HumanAirline);
             }


            this.ParentPage.updatePage();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
             WPFMessageBoxResult result = WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2603"), string.Format(Translator.GetInstance().GetString("MessageBox", "2603", "message"), this.Alliance.Name), WPFMessageBoxButtons.YesNo);
            
             if (result == WPFMessageBoxResult.Yes)
             {
                 this.Alliance.removeMember(GameObject.GetInstance().HumanAirline);
                 Alliances.RemoveAlliance(this.Alliance);
             }

            this.ParentPage.updatePage();
    
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e)
        {

            throw new NotImplementedException();
        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
             WPFMessageBoxResult result = WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2601"), string.Format(Translator.GetInstance().GetString("MessageBox", "2601", "message"), this.Alliance.Name), WPFMessageBoxButtons.YesNo);

             if (result == WPFMessageBoxResult.Yes)
             {
                 this.Alliance.addMember(GameObject.GetInstance().HumanAirline);
             }

            this.ParentPage.updatePage();
        }
      
        private void lnkAirline_Click(object sender, RoutedEventArgs e)
        {
            Airline airline = (Airline)((Hyperlink)sender).Tag;

            PageNavigator.NavigateTo(new PageAirline(airline));

    

        }

    }
}