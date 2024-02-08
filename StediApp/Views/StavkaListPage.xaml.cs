using Plugin.Fingerprint.Abstractions;
using StediApp.Data;
using StediApp.Models;
using StediApp.Services;
using System.ComponentModel;

namespace StediApp.Views;

public partial class StavkaListPage : ContentPage
{
    StavkaService StavkaService;
	private decimal incomeTotal = 0;
	private decimal expenseTotal = 0;
    private List<Stavka> expenseItems;
    private List<Stavka> incomeItems;
    private DateTime resetOnDay;
    private bool clearedPaid;

	public StavkaListPage()
	{
		InitializeComponent();
        this.StavkaService = new StavkaService();
        NavigationPage.SetHasBackButton(this, false);
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        try
        {
            this.expenseItems = await this.StavkaService.GetExpenseItemsAsync();
            this.incomeItems = await this.StavkaService.GetIncomeItemsAsync();

            try
            {
                var firstDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                this.resetOnDay = Preferences.Default.Get("Dan plaćanja", firstDay);
                this.clearedPaid = Preferences.Default.Get("ClearedPaid", true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("GREŠKA ZA GREŠKOM", ex.Message, "DOBRO BA");
            }
            


            if (this.expenseItems.Count > 0)
            {
                expenseListView.ItemsSource = this.expenseItems;
                this.expenseTotal = this.StavkaService.GetTotalFor(this.expenseItems);
                this.txtTotalExpense.IsVisible = true;
                txtTotalExpense.Text = $"{this.expenseTotal}KM";

                var gotLeftToPay = this.StavkaService.GetLeftToPay(this.expenseItems);
                if (gotLeftToPay > 0 && gotLeftToPay != this.expenseTotal)
                {
                    txtExpensesPaid.Text = $"Ostalo je da tišPla {gotLeftToPay}KM ba / ";
                    txtExpensesPaid.IsVisible = true;
                }
                else
                {
                    txtExpensesPaid.IsVisible = false;
                }
            }
            else
            {
                this.txtTotalExpense.IsVisible = false;
                this.expenseTotal = 0;
                expenseListView.ItemsSource = new List<Stavka>();
            }

            if (this.incomeItems.Count > 0)
            {
                incomeListView.ItemsSource = this.incomeItems;
                this.incomeTotal = this.StavkaService.GetTotalFor(this.incomeItems);
                txtTotalIncome.IsVisible = true;
                txtTotalIncome.Text = $"{this.incomeTotal}KM";            
            }
            else
            {
                txtTotalIncome.IsVisible = false;
                incomeListView.ItemsSource = new List<Stavka>();
            }

            UpdateDashboard();

           

            if ((DateTime.Today >= this.resetOnDay) && !this.clearedPaid)
            {
                Preferences.Default.Set("Rok plaćanja", this.resetOnDay.AddMonths(1));
                this.resetOnDay = Preferences.Default.Get("Rok plaćanja", DateTime.Today);
                bool cleared = await this.ClearPaidValues(expenseItems);
                if (cleared)
                {
                    Preferences.Default.Set("Otplaćeno", true);
                    this.clearedPaid = true;
                    Preferences.Default.Set("Rok plaćanja", DateTime.Today.AddMonths(1));
                }

            }

            //var res = await CheckIfPaidNeedsClearing(resetOnDay);
        }
        catch (Exception ex)
        {
            await DisplayAlert("GREŠKA ZA GREŠKOM", ex.Message, "DOBRO BA");
        }
    }

    private async Task<bool> CheckIfPaidNeedsClearing(DateTime resetOn)
    {
        var today = DateTime.Today;
        
        if (resetOn >= today)
        {
            if (await DisplayAlert("Ažuriraj troškove stavke?","Želiš li da ukloniš stavke za plaćanje u tvojim troškovima?","Da","De nemoj"))
            {
                foreach (var item in expenseItems)
                {
                    item.IsPaid = false;
                    await this.StavkaService.SaveItemAsync(item);
                }
            }
        }

        return true;
    }

    async void OnItemAdded(object sender, EventArgs eventArgs)
	{
        
        await AddItem();
        
    }

	async void OnIncomeItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
        if (e.SelectedItem != null)
        {
            await Navigation.PushAsync(new StavkaPage()
            {
            BindingContext = e.SelectedItem as Stavka
            });
        }
        
    }


    async void OnExpenseItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
        if (e.SelectedItem != null)
        {
            await Navigation.PushAsync(new StavkaPage()
            {
            BindingContext = e.SelectedItem as Stavka
            });
        }
	}

   
	async Task OpenNewItemPage(bool isExpense = true)
	{
        await Navigation.PushAsync(new StavkaPage()
        {
            BindingContext = new Stavka(isExpense)
        });
    }

	private void UpdateDashboard()
	{
        int expenseCount = this.expenseItems.Count;
        int incomeCount = this.incomeItems.Count;
        this.HideMainDashboard(false);

        if (incomeCount > 0 && expenseCount == 0 )
        {
            this.SetupDashboardWithIncomeOnly();
        }

        if (expenseCount > 0 && incomeCount == 0 )
        {
            this.SetupDashboardWithExpenseOnly();
        }

        if (expenseCount > 0 && incomeCount > 0)
        {
            this.SetupDashboardMain();
        }

        if (incomeCount == 0 && expenseCount == 0)
        {
            this.HideMainDashboard(true);
        }
    }

    private async void OnSettingsClick(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new SettingsView());
        
    }

    private void SetupDashboardWithIncomeOnly()
    {
        progressMoneyLeft.Progress = 1;
        lblMoneyOutOf.IsVisible = false;
        lblDashboardStart.Text = "Nemaš troškova. Dodaj trošak jarane.";
        lblInitialText.Text = $"{lblDashboardStart.Text}";
        lblMoneyLeftStart.Text = "Preostalo ti je ";
        lblMoneyLeftAmount.Text = $"{this.incomeTotal}KM";
        lblMoneyLeftEnd.Text = " u budžetu.";
    }

    private void SetupDashboardWithExpenseOnly()
    {
        progressMoneyLeft.Progress = 0;
        lblMoneyOutOf.IsVisible = false;
        lblDashboardStart.Text = "Nemaš prihoda, ali imaš troškova u iznosu od ";
        lblDashboardAmount.Text = $"{this.expenseTotal}KM";
        lblDashboardEnd.Text = "";
        lblInitialText.Text = $"{lblDashboardStart.Text}{lblDashboardAmount.Text}{lblDashboardEnd.Text}";

        lblMoneyLeftStart.Text = "Koristiš ";
        lblMoneyLeftAmount.Text = $"{this.expenseTotal}KM";
        lblMoneyLeftEnd.Text = " budžeta.";
    }

    private void SetupDashboardMain()
    {
        decimal moneyLeftAmount = this.incomeTotal - this.expenseTotal;
        string moneyLeftText = $"{moneyLeftAmount}KM";

        if (this.incomeTotal > 0)
        {
            decimal oneProcent = this.incomeTotal / 100;
            var progress = Convert.ToDouble((this.expenseTotal / oneProcent)) * 0.01;
            progressMoneyLeft.Progress = 1 - progress;


            lblMoneyOutOf.IsVisible = true;
            lblMoneyOutOf.Text = $"{txtTotalExpense.Text} / {txtTotalIncome.Text}";

            if (this.expenseTotal > this.incomeTotal)
            {
                lblDashboardStart.Text = "Troškovi su ti ";
                lblDashboardAmount.Text = $"{moneyLeftAmount * -1}KM";
                lblDashboardEnd.Text = $" preko tvog budžeta koji iznosi {this.incomeTotal}KM";
                lblInitialText.Text = $"{lblDashboardStart.Text}{lblDashboardAmount.Text}{lblDashboardEnd.Text}";

                lblMoneyLeftStart.Text = "U minusu si ";
                lblMoneyLeftAmount.Text = moneyLeftText;
                lblMoneyLeftEnd.Text = " .";
            }
            else
            {

                lblDashboardStart.Text = "Na osnovu vaših prihoda i troškova koristite ";
                lblDashboardAmount.Text = txtTotalExpense.Text;
                lblDashboardEnd.Text = $" vašeg budžeta.";
                lblInitialText.Text = $"{lblDashboardStart.Text}{txtTotalExpense.Text}{lblDashboardEnd.Text}";

                lblMoneyLeftStart.Text = "Ostalo ti je ";
                lblMoneyLeftAmount.Text = moneyLeftText;
                lblMoneyLeftEnd.Text = " u budžetu.";
            }
        }
        else
        {
            lblMoneyOutOf.Text = $"{this.expenseTotal}KM / 0KM";
            progressMoneyLeft.Progress = 0;
            lblMoneyLeft.Text = $"Potrošio si {this.expenseTotal}KM više od tvog budžeta!";
        }
    }

    private void HideMainDashboard(bool hide)
    {
            gridDashboardMain.IsVisible = !hide;
            gridDashboardEmpty.IsVisible = hide;
    }

    private async Task AddItem()
    {

        var answer = await DisplayActionSheet("Šta želite dodati?", "Odustani", null, new string[] { "Prihod", "Trošak" });
        switch (answer)
        {
            case "Prihod":
                {
                    await OpenNewItemPage(false);
                    break;
                }
            case "Trošak":
                {
                    await OpenNewItemPage();
                    break;
                }
            default:
                break;
        }
    }

    private async Task AddPrihod()
    {
        
        await OpenNewItemPage(false);
                   
    }


    private async void btnDashboardAddPrihod_Clicked(object sender, EventArgs e)
    {

        await AddPrihod();

    }

    private async Task AddTrosak()
    {

        await OpenNewItemPage();

    }

    private async void btnDashboardAddTrosak_Clicked(object sender, EventArgs e)
    {

        await AddTrosak();

    }

    private async void btnDashboardAddItem_Clicked(object sender, EventArgs e)
    {
       
        await AddItem();
        
    }

    private async Task<bool> ClearPaidValues(List<Stavka> items)
    {
        try
        {
            bool cleared = true;
            foreach (var item in items)
            {
                item.IsPaid = !cleared;
                await this.StavkaService.SaveItemAsync(item);
            }

            Preferences.Default.Set("Otplaćeno", cleared);
            this.clearedPaid = cleared;

            return cleared;

        }
        catch (Exception ex)
        {
            await DisplayAlert("GREŠKA", ex.Message, "DOBRO BA");
            return false;
        }
    }
}