using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace dm106.CRMClient
{
    public class CRMRestClient
    {
        private HttpClient client;

        public CRMRestClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://siecolacrm.azurewebsites.net/api/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            byte[] str1Byte = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", "crmwebapi", "crmwebapi"));
            String plaintext = Convert.ToBase64String(str1Byte);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", plaintext);
        }

        public Customer GetCustomerByEmail(string email)
        {

            HttpResponseMessage response = client.GetAsync("customers/byemail?email=" + email).Result;
            if (response.IsSuccessStatusCode)
            {
                Customer customer = (Customer)response.Content.ReadAsAsync<Customer>().Result;
                return customer;
            }
            return null;
        }
    }
}