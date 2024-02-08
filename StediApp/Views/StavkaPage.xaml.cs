using StediApp.Models;
using StediApp.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Plugin.Fingerprint.Abstractions;

namespace StediApp.Views;

public partial class StavkaPage : ContentPage
{
	private bool updated = false;
    public StavkaPage()
	{
		InitializeComponent();        
    }

    async void OnSaveClicked(object sender, EventArgs e)
	{
        
        var Stavka = (Stavka)BindingContext;
        if (Stavka != null && Stavka.AmountText != null && Stavka.Amount > 0)
        {
            StavkaItemDatabase database = await StavkaItemDatabase.Instance;
            var res = await database.SaveItemAsync(Stavka);
            await Navigation.PopAsync();
        }
        else
        {
           await DisplayAlert("GREŠKA", "Desila se greška! Greška se desila! TI nisi čovjek ti si greška ba.", "DOBRO BA");
           await Navigation.PopAsync();
        }
        
	}

	async void OnDeleteClicked(object sender, EventArgs e)
	{
        
        var Stavka = (Stavka)BindingContext;
        if (await DisplayAlert("Jesi siguran sto posto?", $"Ho'š obrisat il' ne'š {Stavka.Name}?", "Hoću", "A neću"))
        {
            StavkaItemDatabase database = await StavkaItemDatabase.Instance;
            await database.DeleteItemAsync(Stavka);
            await Navigation.PopAsync();
        }
       
        
	}

    private void HandleUpdate()
    {
        if (!updated)
        {
            updated = true;
        }
    }
    async Task CancelItem()
    {
        try
        {
            var Stavka = (Stavka)BindingContext;

            if (string.IsNullOrWhiteSpace(Stavka.Name))
            {
                await Navigation.PopAsync();
            }

            if (updated)
            {
                bool update = await DisplayAlert("Sačekaj ba...", "Ažurirao si neke informacije za ovu stavku. Želiš li da nastaviš sa ažuriranjem ili ne ba?", "Ažuriraj", "De nemoj");
                if (update)
                {
                    //OnSaveClicked(sender, e);
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("GREŠKA", ex.Message, "DOBRO BA");
            await Navigation.PopAsync();
        }
    }

    private void txtName_TextChanged(object sender, TextChangedEventArgs e)
    {
        HandleUpdate();
    }

    public ICommand CancelCommand { get; private set; }

    private async Task OnCancelClicked(object sender, NavigatingFromEventArgs e)
    {
        await CancelItem();
    }
}