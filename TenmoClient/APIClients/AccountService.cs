﻿using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.APIClients
{
    public class AccountService
    {
        private const string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public decimal GetBalance(int userId, string token)
        {
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(API_BASE_URL + "balance");

            IRestResponse<API_Balance> response = client.Get<API_Balance>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return 0;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("Response was unsuccessful. ");
                return 0;
            }
            else
            {
                return response.Data.Balance;
            }
        }
    }
}
