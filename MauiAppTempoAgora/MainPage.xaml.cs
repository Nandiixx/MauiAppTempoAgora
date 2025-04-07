using System;
using System.Threading.Tasks;
using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = $"Latitude: {t.lat}\n" +
                                                $"Longitude: {t.lon}\n" +
                                                $"Nascer do Sol: {t.sunrise}\n" +
                                                $"Por do Sol: {t.sunset}\n" +
                                                $"Temp Máx: {t.temp_max}°C\n" +
                                                $"Temp Mín: {t.temp_min}°C\n" +
                                                $"Descrição: {t.description}\n" +
                                                $"Vento: {t.speed} m/s\n" +
                                                $"Visibilidade: {t.visibility} metros";

                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de previsão.";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}
