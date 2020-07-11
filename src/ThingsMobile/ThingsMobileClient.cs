using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ActivateSimCardAsync(string msisdn,
                                                                                        string barcode,
                                                                                        CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["simBarcode"] = barcode
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/activateSim");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Block a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> BlockSimCardAsync(string msisdn, CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/blockSim");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Modify an existing sim plan
        /// </summary>
        /// <param name="modifyCustomSimPlan">Details for the sim plan modifications</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ModifyCustomSimPlanAsync(ModifyCustomSimPlanModel modifyCustomSimPlan,
                                                                                            CancellationToken cancellationToken = default)
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

            var url = new Uri(options.BaseUrl, "/modifyCustomPlan");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Create a custom sim plan
        /// </summary>
        /// <param name="simPlanModel">Details of the custom sim plan</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> CreateCustomSimPlanAsync(AddCustomSimPlanModel simPlanModel,
                                                                                            CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["name"] = simPlanModel.Name,
                ["simAutoRechargeEnabled"] = $"{simPlanModel.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{simPlanModel.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{simPlanModel.AutoRechargeAmount}"
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/createCustomPlan");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Gets all sim cards
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> ListSimCardsAsync(CancellationToken cancellationToken = default)
        {
            var formContent = CreateFormUrlContent();

            var url = new Uri(options.BaseUrl, "/simList");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<SimCollection>(request, cancellationToken);
        }

        /// <summary>
        /// Get a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardAsync(string msisdn, CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/simStatus");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<SimCollection>(request, cancellationToken);
        }

        /// <summary>
        /// Retrieve credit details for the user
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<Credit>> GetCreditDetailsAsync(CancellationToken cancellationToken = default)
        {
            var formContent = CreateFormUrlContent();

            var url = new Uri(options.BaseUrl, "/credit");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<Credit>(request, cancellationToken);
        }

        /// <summary>
        /// Get custom sim plan lists
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimPlanCollection>> GetSimPlansAsync(CancellationToken cancellationToken = default)
        {
            var formContent = CreateFormUrlContent();
            var url = new Uri(options.BaseUrl, "/customPlanList");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<SimPlanCollection>(request, cancellationToken);
        }

        /// <summary>
        /// Sets an expiry date for a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN for the SIM card</param>
        /// <param name="expiryDateString">Date which the sim card expires. Format (yyyy-MM-dd)</param>
        /// <param name="blockSimAfterExpiry">Whether to  block the sim card after expiry</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SetSimExpiryDateAsync(string msisdn,
                                                                                         string expiryDateString,
                                                                                         bool blockSimAfterExpiry,
                                                                                         CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["expirationDate"] = expiryDateString,
                ["blockSim"] = blockSimAfterExpiry ? "1" : "0"
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/setupSimExpirationDate");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Sets Sim thresholds
        /// </summary>
        /// <param name="threshold">Details of the thresholds to be set</param>
        /// <returns></returns>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SetSimThresholdsAsync(SimThreshold threshold,
                                                                                         CancellationToken cancellationToken = default)
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

            var url = new Uri(options.BaseUrl, "/setupSimTrafficThreshold");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Unblock a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UnblockSimCardAsync(string msisdn,
                                                                                       CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/unblockSim");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Updates the name of the sim
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="name">Name of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimNameAsync(string msisdn,
                                                                                      string name,
                                                                                      CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["name"] = name
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/updateSimName");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Send an SMS to an active Things Mobile SIM 
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="message">sms message (160 characters maximum)</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> SendSmsToSimAsync(string msisdn,
                                                                                     string message,
                                                                                     CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["message"] = message
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/sendSms");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Associates a custom sim plan to specified sim card
        /// </summary>
        /// <param name="msisdn">MSISDN for the sim card</param>
        /// <param name="customPlanId">Unique identifier for the sim plan</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> ChangeSimPlanAsync(string msisdn,
                                                                                      string customPlanId,
                                                                                      CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["customPlanId"] = customPlanId
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/changeSimPlan");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Updates the sim tag
        /// </summary>
        /// <param name="msisdn">MSISDN for the SIM card</param>
        /// <param name="tag">Tag for the SIM card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimTagAsync(string msisdn,
                                                                                     string tag,
                                                                                     CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["tag"] = tag
            };

            var formContent = CreateFormUrlContent(dict);

            var url = new Uri(options.BaseUrl, "/updateSimTag");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = formContent
            };

            return await SendAsync<BaseResponseModel>(request, cancellationToken);
        }

        /// <summary>
        /// Send actual request to things mobile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        private async Task<ThingsMobileResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
            where T : BaseResponseModel
        {
            using (var response = await httpClient.SendAsync(request, cancellationToken))
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
