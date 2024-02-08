using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using StediApp.Views;
using System;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace StediApp;


public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        var novaStranica = new Pocetna(CrossFingerprint.Current);
        MainPage = new NavigationPage(novaStranica)
        {
            BarTextColor = Color.FromRgb(255, 255, 255),
            BarBackgroundColor = Color.FromArgb("#9A57F2")
        };

    }


}

