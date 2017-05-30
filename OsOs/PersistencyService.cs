using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OsOs.Utilities;

namespace OsOs
{
    class PersistencyService
    {
        private const string ServerURL = "http://ososwebservice.azurewebsites.net/";
//        private const string ServerURL = "http://localhost:3004/";

        public static async Task<T> PostToDatabase<T>(T itemToPost, string api, bool displayDialog=true)
        {
            HttpClientHandler handlerSave = new HttpClientHandler();
            using (var clientSave = new HttpClient(handlerSave))
            {
                clientSave.BaseAddress = new Uri(ServerURL);
                var response = await clientSave.PostAsJsonAsync("api/"+api, itemToPost);
                var temp = response.Content.ReadAsAsync<T>().Result;
//                itemToPost.? = ?;
                if (displayDialog)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        MessageDialogHelper.Show($"?", "Done");
                    }
                    else
                    {
                        MessageDialogHelper.Show(response.ReasonPhrase, $"{response.StatusCode}");
                    }
                }
                return temp;
            }
        }

        public static async Task<ObservableCollection<T>>  GetFromDatabase<T>(string api)
        {
            HttpClientHandler handlerLoad = new HttpClientHandler();
            handlerLoad.UseDefaultCredentials = true;
            ObservableCollection<T> read = new ObservableCollection<T>();

            using (var clientLoad = new HttpClient(handlerLoad))
            {
                clientLoad.BaseAddress = new Uri(ServerURL);
                clientLoad.DefaultRequestHeaders.Clear();
                clientLoad.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var response = clientLoad.GetAsync("api/"+api).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        IEnumerable<T> x = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                        foreach (var t in x)
                        {
                            read.Add(t);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageDialogHelper.Show(e.Message, "Fejl");
                }
            }
            return read;
        }
//
        public static async Task PutToDatabase<T>(T itemToUdate, string api, string id, bool displayDialog = true)
        {
            HttpClientHandler handlerPut = new HttpClientHandler();
            using (var clientPut = new HttpClient(handlerPut))
            {
                clientPut.BaseAddress = new Uri(ServerURL);
                var response = await clientPut.PutAsJsonAsync($"api/{api}/{id}", itemToUdate);
                if (displayDialog)
                {
                    if (response.IsSuccessStatusCode)
                    {
                MessageDialogHelper.Show($"{itemToUdate.GetType()} med ID {id} er blevet opdateret", "Done");

                    }
                    else
                    {
                        MessageDialogHelper.Show($"Fejl :{response.ReasonPhrase} ",$"{response.StatusCode}");
                    }
                }
                
            }
        }
//
        public static async Task DeleteFromDatabase(string api, string id, bool displayDialog = true)
        {
            HttpClientHandler handlerDelete = new HttpClientHandler();
            using (var clientDelete = new HttpClient(handlerDelete))
            {
                clientDelete.BaseAddress = new Uri(ServerURL);
                var response = await clientDelete.DeleteAsync($"/api/{api}/{id}");

                if (api == "employees")
                    api = "Ansat";
                
                if (displayDialog)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        MessageDialogHelper.Show($"{api} med ID {id} Er blevet slettet", "Godkendt");

                    }
                    else
                    {
                        MessageDialogHelper.Show(response.ReasonPhrase, "Fejl");
                    }
                }
                // Alternativ
                //await client3.DeleteAsync($"api/events/{Event.ID}");
            }
        }
    }
}
