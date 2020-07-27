using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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

            // populate the User-Agent header
            var productVersion = typeof(ThingsMobileClient).Assembly.GetName().Version.ToString();
            var userAgent = new ProductInfoHeaderValue("thingsmobile-dotnet", productVersion);
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(userAgent);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["simBarcode"] = barcode
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/activateSim");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
        }

        /// <summary>
        /// Block a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> BlockSimCardAsync(string msisdn, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/blockSim");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["Id"] = modifyCustomSimPlan.Id,
                ["name"] = modifyCustomSimPlan.Name,
                ["simAutoRechargeEnabled"] = $"{modifyCustomSimPlan.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{modifyCustomSimPlan.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{modifyCustomSimPlan.AutoRechargeAmount}"
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/modifyCustomPlan");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["name"] = simPlanModel.Name,
                ["simAutoRechargeEnabled"] = $"{simPlanModel.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{simPlanModel.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{simPlanModel.AutoRechargeAmount}"
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/createCustomPlan");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
        }

        /// <summary>
        /// Gets all sim cards
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        [Obsolete("Use the 'GetSimCardsLiteAsync' method instead")]
        public async Task<ThingsMobileResponse<SimCollection>> ListSimCardsAsync(CancellationToken cancellationToken = default)
        {
            var url = new Uri(options.BaseUrl, "/services/business-api/simList");
            return await PostAsync<SimCollection>(url, cancellationToken);
        }

        /// <summary>
        /// Get a sim card with call detail records
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardAsync(string msisdn, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/simStatus");
            return await PostAsync<SimCollection>(url, parameters, cancellationToken);
        }

        /// <summary>
        /// Get sim card(s) without call detail records
        /// </summary>
        /// <param name="name">Name of the sim</param>
        /// <param name="tag">Tag of the sim</param>>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardsLiteAsync(string name, string tag, CancellationToken cancellationToken = default) 
        {
            var parameters = new Dictionary<string, string>
            {
                ["name"] = name,
                ["tag"] = tag
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/simListLite");
            return await PostAsync<SimCollection>(url, parameters, cancellationToken);
        }

        /// <summary>
        /// Retrieve credit details for the user
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<Credit>> GetCreditDetailsAsync(CancellationToken cancellationToken = default)
        {
            var url = new Uri(options.BaseUrl, "/services/business-api/credit");
            return await PostAsync<Credit>(url, cancellationToken);
        }

        /// <summary>
        /// Get custom sim plan lists
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimPlanCollection>> GetSimPlansAsync(CancellationToken cancellationToken = default)
        {
            var url = new Uri(options.BaseUrl, "/services/business-api/customPlanList");
            return await PostAsync<SimPlanCollection>(url, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["expirationDate"] = expiryDateString,
                ["blockSim"] = blockSimAfterExpiry ? "1" : "0"
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/setupSimExpirationDate");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = threshold.MSISDN,
                ["dailyLimit"] = $"{threshold.DailyLimit}",
                ["blockSimDaily"] = threshold.BlockSimDaily ? "1" : "0",
                ["monthlyLimit"] = $"{threshold.MonthlyLimit}",
                ["blockSimMonthly"] = threshold.BlockSimMonthly ? "1" : "0",
                ["totalLimit"] = $"{threshold.TotalLimit}",
                ["blockSimTotal"] = threshold.BlockSimTotal ? "1" : "0"
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/setupSimTrafficThreshold");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/unblockSim");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["name"] = name
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/updateSimName");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["message"] = message
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/sendSms");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["customPlanId"] = customPlanId
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/changeSimPlan");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string>
            {
                ["msisdn"] = msisdn,
                ["tag"] = tag
            };

            var url = new Uri(options.BaseUrl, "/services/business-api/updateSimTag");
            return await PostAsync<BaseResponseModel>(url, parameters, cancellationToken);
        }

        /// <summary>
        /// Send actual request to things mobile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The url to make the request to</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        private Task<ThingsMobileResponse<T>> PostAsync<T>(Uri url, CancellationToken cancellationToken = default)
            where T : BaseResponseModel
        {
            return PostAsync<T>(url, null, cancellationToken);
        }

        /// <summary>
        /// Send actual request to things mobile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The url to make the request to</param>
        /// <param name="parameters">The body parameters</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        private async Task<ThingsMobileResponse<T>> PostAsync<T>(Uri url, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
            where T : BaseResponseModel
        {
            // ensure there are parameters
            parameters = parameters ?? new Dictionary<string, string>();

            // add authentication parameters
            parameters.Add("username", options.Username);
            parameters.Add("token", options.Token);

            // form the content and request
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            // send the request
            var response = await httpClient.SendAsync(request, cancellationToken);

            // extract the response
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var error = default(ThingsMobileErrorResponse);
                var resource = default(T);

                if (response.IsSuccessStatusCode)
                {
                    var serializer = new XmlSerializer(typeof(T));
                    resource = (T)serializer.Deserialize(stream);
                }
                else
                {
                    error = (ThingsMobileErrorResponse)errorSerializer.Deserialize(stream);
                }

                return new ThingsMobileResponse<T>
                {
                    StatusCode = response.StatusCode,
                    IsSuccessful = response.IsSuccessStatusCode,
                    Error = error,
                    Resource = resource,
                };
            }
        }
    }
}
