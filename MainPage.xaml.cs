using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using RestSharp;

namespace MauiAPITest
{
    public partial class MainPage : ContentPage
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        string RestUrl = "https://api2.mondoiberica.com/apitap/ordenes?trabajador=EGIL";
        

        public static string BaseAddress =
    DeviceInfo.Platform == DevicePlatform.Android ? "http://192.168.1.20:40048" : "http://localhost:40048";
        public static string Url = $"{BaseAddress}/apitap/ordenes?trabajador=EGIL";
        public MainPage()
        {
            InitializeComponent();

            /*_client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

             LoadDataAsync();*/

            GetApiResponse();
        }
        private async void GetApiResponse()
        {
            try
            {
                var options = new RestClientOptions("http://api2.mondoiberica.com:40048")
                {
                    ThrowOnAnyError = true,
                    MaxRedirects = 5 // 10 segundos de tiempo de espera
                };
                var client = new RestClient(options);
                var request = new RestRequest("/apitap/estructura?producto=AC001&niveles=2", Method.Get);

                // Registro del request antes de ejecutarlo
                Console.WriteLine("URL de la solicitud: " + client.BuildUri(request));

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    Console.WriteLine("Respuesta de la API: " + response.Content);
                    await App.Current.MainPage.DisplayAlert("Msg", response.Content, "OK");
                }
                else
                {
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode} - {response.ErrorMessage}");
                    await App.Current.MainPage.DisplayAlert("Error", $"Error en la solicitud: {response.StatusCode} - {response.ErrorMessage}", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine("Error de solicitud HTTP: " + httpEx.Message);
                await App.Current.MainPage.DisplayAlert("HTTP Request Exception", httpEx.Message, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepción: " + ex.Message);
                await App.Current.MainPage.DisplayAlert("Exception", ex.Message, "OK");
            }
        }



        private async Task LoadDataAsync()
        {
            var data = await GetModelosAsync();
            listView.ItemsSource = data;
        }

        public async Task<List<Modelo>> GetModelosAsync()
        {
           var Items = new List<Modelo>();

            Uri uri = new Uri(RestUrl);

            
                var response = await _client.GetAsync(uri);               
                var content = await response.Content.ReadAsStringAsync();
                JsonNode nodos = JsonNode.Parse(content);
                JsonNode results = nodos["Ordenes"];
                await App.Current.MainPage.DisplayAlert("Msg", content, "OK");
                Items = JsonSerializer.Deserialize<List<Modelo>>(results.ToString());
            

            return Items;
        }
    }

}
