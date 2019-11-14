﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PathwayGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage() : base()
        {
            InitializeComponent();
            // Set navigation bar icon
            //NavigationPage.SetTitleIconImageSource(this, ImageSource.FromFile("logo.png"));
        }
        public CustomNavigationPage(Page root) : base(root)
        {
            InitializeComponent();
            // Set navigation bar icon
            //NavigationPage.SetTitleIconImageSource(this, ImageSource.FromFile("logo.png"));
        }
    }
}