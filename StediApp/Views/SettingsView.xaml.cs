using Microsoft.Maui.Storage;
using System.Globalization;

namespace StediApp.Views;

public partial class SettingsView : ContentPage
{
 
	public SettingsView()
	{
		InitializeComponent();
        string cultureName = "en-GB";
        var culture = new CultureInfo(cultureName);
        DateTime payDate = Preferences.Default.Get("Rok plaćanja", DateTime.Today);
        DateTime date = Preferences.Get("Rok", DateTime.Today);
        lblNextResetDate.Text = date.ToString("dd/MM/yyyy");
        txtDayOfTheMonth.Text = payDate.Day.ToString();
    }

    private async void OnSave_Clicked(object sender, EventArgs e)
    {
        try
        {
            int numberOfDays = Convert.ToInt32(txtDayOfTheMonth.Text);
            if (numberOfDays > 31)
            {
                await DisplayAlert("Invalid day", "Vrijednost dana ne može biti preko 31", "OK");

                var today = DateTime.Today;
                var firstDay = new DateTime(today.Year, today.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                var day = lastDay.Day;
                Preferences.Default.Set("Rok", lastDay);
                Preferences.Default.Set("ClearedPaid", false);
                txtDayOfTheMonth.Text = $"{day}";
            }
            else if (numberOfDays < 1)
            {
                await DisplayAlert("Invalid day", "Vrijednost dana ne može biti ispod 1", "OK");
                Preferences.Default.Set("Rok", 1);
                Preferences.Default.Set("ClearedPaid", false);
                txtDayOfTheMonth.Text = "1";
            }else if (DateTime.Today.Month == 2 && numberOfDays > 29)
            {
                await DisplayAlert("Invalid day", "Vrijednost dana u februaru ne može biti iznad 29", "OK");
                Preferences.Default.Set("Rok", 28);
                Preferences.Default.Set("ClearedPaid", false);
                txtDayOfTheMonth.Text = "28";
            }
            else
            {
                var today = DateTime.Today;
                var firstDayOfThisMonth = new DateTime(today.Year, today.Month, 1);
                var lastDayOfCurrentMonth = firstDayOfThisMonth.AddMonths(1).AddDays(-1);
                var day = firstDayOfThisMonth.AddDays(numberOfDays - 1);
                if ((day < today) && day.Month == DateTime.Today.Month)
                {
                    day = day.AddMonths(1);
                }
                Preferences.Default.Set("Rok", day);
                Preferences.Default.Set("ClearedPaid", false);
                await DisplayAlert("Ažurirano!", "'Rok plaćanja' je ažuriran.", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async void btnClearSettings_Clicked(object sender, EventArgs e)
    {
        bool res = await DisplayAlert("UPOZORENJE!!!", "Postavke će biti resetirane. Želite li nastaviti?", "Da", "De nemoj");
        if (res)
        {
            if (await DisplayAlert("Molimo potvrdite", "Jesi siguran?", "Da, resetuj", "De nemoj"))
            {
                Preferences.Clear();
                await DisplayAlert("Obrisano", "Postavke su resetirane", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}