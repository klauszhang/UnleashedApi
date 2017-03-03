using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace UnleashedApi
{
  class Program
  {
    static void Main(string[] args)
    {

      Task.WaitAll(
        RunMain()
        );

      Console.WriteLine("success!");
      Console.ReadLine();
    }

    private static async Task CreateNewCustomer(HttpClient client, Customer newCustomer)
    {

      var request = new HttpRequestMessage(HttpMethod.Post, $"/Customers/{newCustomer.Guid}");
      request.Content = new StringContent(JsonConvert.SerializeObject(newCustomer), Encoding.UTF8, "application/json");
      request.Headers.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
      request.Headers.Add("api-auth-id", "9dcf114f-702d-49da-9455-ad95e7a7d241");
      request.Headers.Add("api-auth-signature", GetSignature(string.Empty, Key));

      var response = await client.SendAsync(request);
      Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
    }


    static async Task RunMain()
    {
      var client = new HttpClient();
      client.BaseAddress=new Uri("https://api.unleashedsoftware.com");
      var newCustomer = new Customer
      {
        CustomerName = "Foo"
      };
      await CreateNewCustomer(client, newCustomer);

     var retrieved= await GetCreatedCustomer(client, newCustomer.Guid);

      Assert.AreEqual(newCustomer.Guid, retrieved.Guid);
      Assert.AreEqual(newCustomer.CustomerName, retrieved.CustomerName);
    }

    private static async Task<Customer> GetCreatedCustomer(HttpClient client, Guid id)
    {
      var request = new HttpRequestMessage(HttpMethod.Get, $"/Customers/{id}");
      request.Headers.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
      request.Headers.Add("api-auth-id", "9dcf114f-702d-49da-9455-ad95e7a7d241");
      request.Headers.Add("api-auth-signature", GetSignature(string.Empty, Key));

      var response = await client.SendAsync(request);

      Assert.AreNotEqual(response.StatusCode, HttpStatusCode.NotFound);
      return JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
    }


    private static string GetSignature(string args, string privatekey)
    {
      var encoding = new System.Text.ASCIIEncoding();
      byte[] key = encoding.GetBytes(privatekey);
      var myhmacsha256 = new HMACSHA256(key);
      byte[] hashValue = myhmacsha256.ComputeHash(encoding.GetBytes(args));
      string hmac64 = Convert.ToBase64String(hashValue);
      myhmacsha256.Clear();
      return hmac64;
    }

    private static string Key => "fnCCsYqby0RTFHyouYNGjhqkKhaW2BprneElNcDmCW06EpOsZ0dq1o9HcwaoNFib6biP2ce72jGlgN1yZF6oQ==";
  }
}
