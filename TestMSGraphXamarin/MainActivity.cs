using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestMSGraphXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string CLIENT_ID = "";
        private string CLIENT_SECRET = "";
        private string TENANT_GUID = "";
        private string SITE_ID = "";
        private string LIST_ID = "";

        private static readonly HttpClient httpClient = new HttpClient();

        private JObject auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitializeGraphClientAsync();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task getToken()
        {
            string authority = "https://login.microsoftonline.com/" + TENANT_GUID + "/oauth2/v2.0/token";
            var values = new Dictionary<string, string>
            {
                { "client_id", CLIENT_ID},
                { "client_secret", CLIENT_SECRET },
                { "scope", "https://graph.microsoft.com/.default"},
                { "grant_type", "client_credentials"}
            };

            var content = new FormUrlEncodedContent(values);

            HttpClient client = new HttpClient();
            var response = await client.PostAsync(authority, content);

            var responseString = await response.Content.ReadAsStringAsync();

            auth = JObject.Parse(responseString);
        }

        private async void InitializeGraphClientAsync()
        {
            await getToken();

            httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + auth["access_token"].ToString());
            //graphClient = new GraphServiceClient(httpClient);
            
            GetUserInfo();
        }
        private async Task GetUserInfo()
        {

            var response =  await httpClient.GetAsync("https://graph.microsoft.com/v1.0/users");
            var responseString = await response.Content.ReadAsStringAsync();


            Console.WriteLine(responseString);


        }

        private async Task AddElementToSPList()
        {
            var content = new StringContent("{ \"fields\": {\"Title\":\"Test\"} }", Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://graph.microsoft.com/v1.0/sites/" + SITE_ID + "/lists/" + LIST_ID, content);
            var responseString = await response.Content.ReadAsStringAsync();


            Console.WriteLine(responseString);
        }
    }
}