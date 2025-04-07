using System;
using System.Net.Http;
using System.Threading.Tasks;
using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;
            string chave = "4c683af059ebcd0b4f121b980bf2514a";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception("Cidade não encontrada.");
                    }

                    resp.EnsureSuccessStatusCode(); // Lança exceção se status for erro

                    string json = await resp.Content.ReadAsStringAsync();
                    var rascunho = JObject.Parse(json);

                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    t = new Tempo()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],

                        // Traduzindo a descrição do clima, que vinha em inglês, utilizando o método TradutorClima.
                        description = TradutorClima.Traduzir((string)rascunho["weather"][0]["description"]),
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        
                        // Exibe a hora do por do sol e do nascer, sem o erro de data.
                        sunrise = sunrise.ToString("HH:mm"),
                        sunset = sunset.ToString("HH:mm"),
                    };
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception("Sem conexão com a internet.");
            }

            return t;
        }

        // TradutorClima embutido aqui mesmo
        public static class TradutorClima
        {
            private static readonly Dictionary<string, string> traducoes = new()
        {
            {"clear sky", "céu limpo"},
            {"few clouds", "poucas nuvens"},
            {"scattered clouds", "nuvens dispersas"},
            {"broken clouds", "nuvens fragmentadas"},
            {"shower rain", "chuva forte"},
            {"rain", "chuva"},
            {"thunderstorm", "trovoada"},
            {"snow", "neve"},
            {"mist", "névoa"},
            {"overcast clouds", "nublado"},
        };

            public static string Traduzir(string descricao)
            {
                return traducoes.TryGetValue(descricao.ToLower(), out var traducao)
                    ? traducao
                    : descricao;
            }
        }

    }
}
