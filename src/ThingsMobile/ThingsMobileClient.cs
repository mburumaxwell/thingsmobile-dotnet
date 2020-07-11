using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ThingsMobile.Models;

namespace ThingsMobile
{
    /// <summary>
    /// A client, used to issue requests to ThingsMobile's API and deserialize responses.
    /// </summary>
    public class ThingsMobileClient
    {
        private static readonly XmlSerializer errorSerializer = new XmlSerializer(typeof(ThingsMobileErrorResponse));

        private readonly ThingsMobileClientOptions options;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Creates an instance if <see cref="ThingsMobileClient"/>
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="options">The options for configuring the client</param>
        public ThingsMobileClient(ThingsMobileClientOptions options, HttpClient httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient();
            this.options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.Username))
            {
                throw new ArgumentNullException(nameof(options.Username));
            }

            if (string.IsNullOrWhiteSpace(options.Token))
            {
                throw new ArgumentNullException(nameof(options.Token));
            }

            if (options.BaseUrl == null)
            {
                throw new ArgumentNullException(nameof(options.BaseUrl));
            }
        }

        /// <summary>
        /// Activates a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="barcode">Sim barcode number (19/20 digits long)</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ActivateSimCardAsync(string msisdn, string barcode)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["simBarcode"] = barcode
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/activateSim")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Block a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> BlockSimCardAsync(string msisdn)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/blockSim")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Modify an existing sim plan
        /// </summary>
        /// <param name="modifyCustomSimPlan">Details for the sim plan modifications</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ModifyCustomSimPlanAsync(ModifyCustomSimPlanModel modifyCustomSimPlan)
        {
            var dict = new Dictionary<string, string>
            {
                ["Id"] = modifyCustomSimPlan.Id,
                ["name"] = modifyCustomSimPlan.Name,
                ["simAutoRechargeEnabled"] = $"{modifyCustomSimPlan.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{modifyCustomSimPlan.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{modifyCustomSimPlan.AutoRechargeAmount}"
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/modifyCustomPlan")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Create a custom sim plan
        /// </summary>
        /// <param name="simPlanModel">Details of the custom sim plan</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> CreateCustomSimPlanAsync(AddCustomSimPlanModel simPlanModel)
        {
            var dict = new Dictionary<string, string>
            {
                ["name"] = simPlanModel.Name,
                ["simAutoRechargeEnabled"] = $"{simPlanModel.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{simPlanModel.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{simPlanModel.AutoRechargeAmount}"
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/createCustomPlan")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Gets all sim cards
        /// </summary>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> ListSimCardsAsync()
        {
            var formContent = CreateFormUrlContent();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/simList")
            {
                Content = formContent
            };

            return await SendAsync<SimCollection>(request);
        }

        /// <summary>
        /// Get a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardAsync(string msisdn)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/simStatus")
            {
                Content = formContent
            };

            return await SendAsync<SimCollection>(request);
        }

        /// <summary>
        /// Retrieve credit details for the user
        /// </summary>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<Credit>> GetCreditDetailsAsync()
        {
            var formContent = CreateFormUrlContent();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/credit")
            {
                Content = formContent
            };

            return await SendAsync<Credit>(request);
        }

        /// <summary>
        /// Get custom sim plan lists
        /// </summary>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimPlanCollection>> GetSimPlansAsync()
        {
            var formContent = CreateFormUrlContent();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/customPlanList")
            {
                Content = formContent
            };

            return await SendAsync<SimPlanCollection>(request);
        }

        /// <summary>
        /// Sets an expiry date for a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN for the SIM card</param>
        /// <param name="expiryDateString">Date which the sim card expires. Format (yyyy-MM-dd)</param>
        /// <param name="blockSimAfterExpiry">Whether to  block the sim card after expiry</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SetSimExpiryDateAsync(string msisdn, string expiryDateString, bool blockSimAfterExpiry)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["expirationDate"] = expiryDateString,
                ["blockSim"] = blockSimAfterExpiry ? "1" : "0"
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/setupSimExpirationDate")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Sets Sim thresholds
        /// </summary>
        /// <param name="threshold">Details of the thresholds to be set</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SetSimThresholdsAsync(SimThreshold threshold)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = threshold.MSISDN,
                ["dailyLimit"] = $"{threshold.DailyLimit}",
                ["blockSimDaily"] = threshold.BlockSimDaily ? "1" : "0",
                ["monthlyLimit"] = $"{threshold.MonthlyLimit}",
                ["blockSimMonthly"] = threshold.BlockSimMonthly ? "1" : "0",
                ["totalLimit"] = $"{threshold.TotalLimit}",
                ["blockSimTotal"] = threshold.BlockSimTotal ? "1" : "0"
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/setupSimTrafficThreshold")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Unblock a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UnblockSimCardAsync(string msisdn)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/unblockSim")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Updates the name of the sim
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="name">Name of the sim card</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimNameAsync(string msisdn, string name)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["name"] = name
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/updateSimName")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Send an SMS to an active Things Mobile SIM 
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="message">sms message (160 characters maximum)</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SendSmsToSimAsync(string msisdn, string message)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["message"] = message
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/sendSms")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Associates a custom sim plan to specified sim card
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="customPlanId">Unique identifier for the sim plan</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ChangeSimPlanAsync(string msisdn, string customPlanId)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["customPlanId"] = customPlanId
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/changeSimPlan")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Updates the sim tag
        /// </summary>
        /// <param name="msisdn">MSISDN for the SIM card</param>
        /// <param name="tag">Tag for the SIM card</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimTagAsync(string msisdn, string tag)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["tag"] = tag
            };

            var formContent = CreateFormUrlContent(dict);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{options.BaseUrl}/updateSimTag")
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request);
        }

        /// <summary>
        /// Send actual request to things mobile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<ThingsMobileResponse<T>> SendAsync<T>(HttpRequestMessage request) where T : BaseResponseModel
        {
            using (var response = await httpClient.SendAsync(request))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var result = new ThingsMobileResponse<T>
                {
                    StatusCode = response.StatusCode,
                    IsSuccessful = response.IsSuccessStatusCode
                };

                if (!response.IsSuccessStatusCode)
                {
                    var error = (ThingsMobileErrorResponse)errorSerializer.Deserialize(stream);
                    result.ErrorResponse = error;
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(T));
                    result.Resource = (T)serializer.Deserialize(stream);
                }

                return result;
            }
        }

        /// <summary>
        /// Create form url content
        /// </summary>
        /// <param name="inputs">Items to be inserted into the form</param>
        /// <returns></returns>
        private FormUrlEncodedContent CreateFormUrlContent(IDictionary<string, string> inputs = null)
        {
            var userNamePair = new KeyValuePair<string, string>("username", options.Username);
            var tokenPair = new KeyValuePair<string, string>("token", options.Token);

            if (inputs == null)
            {
                return new FormUrlEncodedContent(new[] { userNamePair, tokenPair });
            }
            else
            {
                // add the common form contents
                inputs.Add(userNamePair);
                inputs.Add(tokenPair);

                return new FormUrlEncodedContent(inputs);
            }
        }
    }
}
