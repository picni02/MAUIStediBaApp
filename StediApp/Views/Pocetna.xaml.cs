using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using StediApp.ViewModels;
using System.Windows.Input;

namespace StediApp.Views;

public partial class Pocetna : ContentPage
{
    private readonly INavigation _navigation;
    private readonly IFingerprint _fingerprint;

    public Pocetna(IFingerprint fingerprint, INavigation navigation)
	{
        InitializeComponent();
        _fingerprint = fingerprint;
        _navigation = navigation;
    }

    public Pocetna(IFingerprint fingerprint)
    {
        InitializeComponent();
        _fingerprint = fingerprint;
        _navigation = Navigation;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void Biometrija(object sender, EventArgs e)
    {
        var request = new AuthenticationRequestConfiguration("Provjera biometrije", "Potrebno je dati otisak prsta za ulazak radi sigurnosti!");
        var result = await _fingerprint.AuthenticateAsync(request);
        if (result.Authenticated)
        {
            await _navigation.PushAsync(new StavkaListPage());
        }
    }

}