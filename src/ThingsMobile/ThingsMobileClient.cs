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
        public ThingsMobileClient(ThingsMobileClientOptions options, HttpClient? httpClient = null)
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

            // set the base address
            this.httpClient.BaseAddress = options.Endpoint ?? throw new ArgumentNullException(nameof(options.Endpoint));

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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["simBarcode"] = barcode
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/activateSim", parameters, cancellationToken);
        }

        /// <summary>
        /// Block a sim card
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> BlockSimCardAsync(string msisdn, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/blockSim", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["Id"] = modifyCustomSimPlan.Id,
                ["name"] = modifyCustomSimPlan.Name,
                ["simAutoRechargeEnabled"] = $"{modifyCustomSimPlan.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{modifyCustomSimPlan.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{modifyCustomSimPlan.AutoRechargeAmount}"
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/modifyCustomPlan", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["name"] = simPlanModel.Name,
                ["simAutoRechargeEnabled"] = $"{simPlanModel.EnableAutoRecharge}",
                ["simAutoRechargeCreditThreshold"] = $"{simPlanModel.AutoRechargeThreshold}",
                ["simAutoRechargeAmount"] = $"{simPlanModel.AutoRechargeAmount}"
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/createCustomPlan", parameters, cancellationToken);
        }

        /// <summary>
        /// Gets all sim cards
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        [Obsolete("Use the 'GetSimCardsLiteAsync' method instead")]
        public async Task<ThingsMobileResponse<SimCollection>> ListSimCardsAsync(CancellationToken cancellationToken = default)
        {
            return await PostAsync<SimCollection>("/services/business-api/simList", cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Get a sim card with call detail records
        /// </summary>
        /// <param name="msisdn">MSISDN of the sim card</param>
        /// <param name="iccid">ICCID for the SIM card</param>
        /// <param name="page">Page number for SIM CDRs</param>
        /// <param name="pageSize">CDR number for page, maximum 2,000</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardAsync(string? msisdn = null,
                                                                               string? iccid = null,
                                                                               int? page = null,
                                                                               int? pageSize = null,
                                                                               CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string?>();
            if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
            if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
            if (page != null) parameters["page"] = page.Value.ToString();
            if (pageSize != null) parameters["pageSize"] = pageSize.Value.ToString();

            return await PostAsync<SimCollection>("/services/business-api/simStatus", parameters, cancellationToken);
        }

        /// <summary>
        /// Get sim card(s) without call detail records
        /// </summary>
        /// <param name="name">Name of the sim</param>
        /// <param name="tag">Tag of the sim</param>
        /// <param name="page">Page number for user’s SIM</param>
        /// <param name="pageSize">SIM number per page, maximum 500</param>>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimCollection>> GetSimCardsLiteAsync(string? name = null,
                                                                                    string? tag = null,
                                                                                    int? page = null,
                                                                                    int? pageSize = null,
                                                                                    CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string?>();
            if (!string.IsNullOrWhiteSpace(name)) parameters["name"] = name;
            if (!string.IsNullOrWhiteSpace(tag)) parameters["tag"] = tag;
            if (page != null) parameters["page"] = page.Value.ToString();
            if (pageSize != null) parameters["pageSize"] = pageSize.Value.ToString();

            return await PostAsync<SimCollection>("/services/business-api/simListLite", parameters, cancellationToken);
        }

        /// <summary>
        /// Retrieve credit details for the user
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<Credit>> GetCreditDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await PostAsync<Credit>("/services/business-api/credit", cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Get custom sim plan lists
        /// </summary>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<SimPlanCollection>> GetSimPlansAsync(CancellationToken cancellationToken = default)
        {
            return await PostAsync<SimPlanCollection>("/services/business-api/customPlanList", cancellationToken: cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["expirationDate"] = expiryDateString,
                ["blockSim"] = blockSimAfterExpiry ? "1" : "0"
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/setupSimExpirationDate", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = threshold.MSISDN,
                ["dailyLimit"] = $"{threshold.DailyLimit}",
                ["blockSimDaily"] = threshold.BlockSimDaily ? "1" : "0",
                ["monthlyLimit"] = $"{threshold.MonthlyLimit}",
                ["blockSimMonthly"] = threshold.BlockSimMonthly ? "1" : "0",
                ["totalLimit"] = $"{threshold.TotalLimit}",
                ["blockSimTotal"] = threshold.BlockSimTotal ? "1" : "0"
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/setupSimTrafficThreshold", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/unblockSim", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["name"] = name
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/updateSimName", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["message"] = message
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/sendSms", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["customPlanId"] = customPlanId
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/changeSimPlan", parameters, cancellationToken);
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
            var parameters = new Dictionary<string, string?>
            {
                ["msisdn"] = msisdn,
                ["tag"] = tag
            };

            return await PostAsync<BaseResponseModel>("/services/business-api/updateSimTag", parameters, cancellationToken);
        }

        /// <summary>
        /// Recharge a sim
        /// </summary>
        /// <param name="amount">The amount in MB (Mega Bytes)</param>
        /// <param name="msisdn">MSISDN for the SIM card</param>
        /// <param name="iccid">ICCID for the SIM card</param>
        /// <param name="cancellationToken">The token for cancelling the task</param>
        /// <returns></returns>
        public async Task<ThingsMobileResponse<BaseResponseModel>> RechargeSimAsync(int amount,
                                                                                    string? msisdn = null,
                                                                                    string? iccid = null,
                                                                                    CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string?>
            {
                ["amount"] = amount.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
            else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
            else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

            return await PostAsync<BaseResponseModel>("/services/business-api/rechargeSim", parameters, cancellationToken);
        }

        private async Task<ThingsMobileResponse<T>> PostAsync<T>(string path, Dictionary<string, string?>? parameters = null, CancellationToken cancellationToken = default)
            where T : BaseResponseModel
        {
            // ensure there are parameters
            parameters ??= new Dictionary<string, string?>();

            // add authentication parameters
            parameters.Add("username", options.Username!);
            parameters.Add("token", options.Token!);

            // form the content and request
            var request = new HttpRequestMessage(HttpMethod.Post, path)
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
