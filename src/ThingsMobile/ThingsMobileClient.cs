using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using ThingsMobile.Models;

namespace ThingsMobile;

/// <summary>
/// A client, used to issue requests to ThingsMobile's API and deserialize responses.
/// </summary>
public class ThingsMobileClient
{
    private static readonly XmlSerializer errorSerializer = new(typeof(ThingsMobileErrorResponse));

    private readonly HttpClient httpClient;
    private readonly ThingsMobileClientOptions options;

    /// <summary>
    /// Creates an instance if <see cref="ThingsMobileClient"/>
    /// </summary>
    /// <param name="options">The options for configuring the client</param>
    public ThingsMobileClient(ThingsMobileClientOptions options)
        : this(null, Options.Create(options)) { }

    /// <summary>
    /// Creates an instance if <see cref="ThingsMobileClient"/>
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> for making the requests.</param>
    /// <param name="optionsAccessor">The options for configuring the client</param>
    public ThingsMobileClient(HttpClient? httpClient, IOptions<ThingsMobileClientOptions> optionsAccessor)
    {
        this.httpClient = httpClient ?? new HttpClient();
        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

        // set the base address
        this.httpClient.BaseAddress = options.Endpoint ?? throw new ArgumentNullException(nameof(options.Endpoint));

        // populate the User-Agent header
        var productVersion = typeof(ThingsMobileClient).Assembly.GetName().Version!.ToString();
        var userAgent = new ProductInfoHeaderValue("thingsmobile-dotnet", productVersion);
        this.httpClient.DefaultRequestHeaders.UserAgent.Add(userAgent);
    }

    /// <summary>
    /// Activates a sim card
    /// </summary>
    /// <param name="msisdn">MSISDN of the sim card</param>
    /// <param name="barcode">Sim barcode number (19/20 digits long)</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
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
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> BlockSimCardAsync(string? msisdn = null,
                                                                                 string? iccid = null,
                                                                                 CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/blockSim", parameters, cancellationToken);
    }

    /// <summary>
    /// Disconnect a sim card
    /// </summary>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> DisconnectSimCardAsync(string? msisdn = null,
                                                                                      string? iccid = null,
                                                                                      CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/disconnectSim", parameters, cancellationToken);
    }

    /// <summary>
    /// Modify an existing sim plan
    /// </summary>
    /// <param name="modifyCustomSimPlan">Details for the sim plan modifications</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
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
    /// <param name="status">
    /// Status of the SIM visible in the SIM card list or SIM card detail in your Things Mobile portal.
    /// You can only use one of the following strings to indicate the status of the SIM:
    /// <c>to-activate</c>, <c>active</c>, <c>suspended</c>, or <c>deactivated</c>.
    /// </param>
    /// <param name="page">Page number for user’s SIM</param>
    /// <param name="pageSize">SIM number per page, maximum 500</param>>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    /// <remarks>
    /// For this API there is an API call limit. You can make an API call after at least 1 second from
    /// your last simListLite API call.
    /// </remarks>
    public async Task<ThingsMobileResponse<SimCollection>> GetSimCardsLiteAsync(string? name = null,
                                                                                string? tag = null,
                                                                                string? status = null,
                                                                                int? page = null,
                                                                                int? pageSize = null,
                                                                                CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(name)) parameters["name"] = name;
        if (!string.IsNullOrWhiteSpace(tag)) parameters["tag"] = tag;
        if (!string.IsNullOrWhiteSpace(tag)) parameters["status"] = status;
        if (page != null) parameters["page"] = page.Value.ToString();
        if (pageSize != null) parameters["pageSize"] = pageSize.Value.ToString();

        return await PostAsync<SimCollection>("/services/business-api/simListLite", parameters, cancellationToken);
    }

    /// <summary>
    /// Get the credit of a user.
    /// </summary>
    /// <param name="start">Start date of the range</param>
    /// <param name="end">End date of the range</param>
    /// <param name="page">page number for SIM's CDR</param>
    /// <param name="pageSize">CDR number per page</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    /// <remarks>
    /// With this API you can get a maximum of 5000 credit history’s operation. If the amount of
    /// operation is greater you will be notified with an error so you can retry with a more restrictive
    /// value for page and pageSize or date range.
    /// <br/>
    /// For this API there is an API call limit. You can make an API call after at least 5 second from
    /// your last credit API call.
    /// </remarks>
    public async Task<ThingsMobileResponse<Credit>> GetCreditDetailsAsync(DateTimeOffset? start = null,
                                                                          DateTimeOffset? end = null,
                                                                          int? page = null,
                                                                          int? pageSize = null,
                                                                          CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();

        if (start is not null) parameters["startDateRange"] = start.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (end is not null) parameters["endDateRange"] = end.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (page is not null) parameters["page"] = page.Value.ToString();
        if (pageSize is not null) parameters["pageSize"] = Math.Min(pageSize.Value, 500).ToString();

        return await PostAsync<Credit>("/services/business-api/credit", parameters, cancellationToken);
    }

    /// <summary>
    /// Get custom sim plan lists
    /// </summary>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<SimPlanCollection>> GetSimPlansAsync(CancellationToken cancellationToken = default)
    {
        return await PostAsync<SimPlanCollection>("/services/business-api/customPlanList", cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Sets an expiry date for a sim card
    /// </summary>
    /// <param name="blockSimAfterExpiry">Whether to  block the sim card after expiry</param>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="expiryDateString">Date which the sim card expires. Format (yyyy-MM-dd)</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> SetSimExpiryDateAsync(string expiryDateString,
                                                                                     bool blockSimAfterExpiry,
                                                                                     string? msisdn = null,
                                                                                     string? iccid = null,
                                                                                     CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["expirationDate"] = expiryDateString,
            ["blockSim"] = blockSimAfterExpiry ? "1" : "0"
        };

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/setupSimExpirationDate", parameters, cancellationToken);
    }

    /// <summary>
    /// Sets Sim thresholds
    /// </summary>
    /// <param name="threshold">Details of the thresholds to be set</param>
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
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> UnblockSimCardAsync(string? msisdn = null,
                                                                                   string? iccid = null,
                                                                                   CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/unblockSim", parameters, cancellationToken);
    }

    /// <summary>
    /// Updates the name of the sim
    /// </summary>
    /// <param name="name">Name of the sim card</param>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimNameAsync(string name,
                                                                                  string? msisdn = null,
                                                                                  string? iccid = null,
                                                                                  CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?> { ["name"] = name, };

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/updateSimName", parameters, cancellationToken);
    }

    /// <summary>
    /// Send an SMS to an active Things Mobile SIM 
    /// </summary>
    /// <param name="message">sms message (160 characters maximum)</param>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> SendSmsToSimAsync(string message,
                                                                                 string? msisdn = null,
                                                                                 string? iccid = null,
                                                                                 CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?> { ["message"] = message, };

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/sendSms", parameters, cancellationToken);
    }

    /// <summary>
    /// Associates a custom sim plan to specified sim card
    /// </summary>
    /// <param name="customPlanId">Unique identifier for the sim plan</param>
    /// <param name="msisdn">MSISDN for the sim card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> ChangeSimPlanAsync(string customPlanId,
                                                                                  string? msisdn = null,
                                                                                  string? iccid = null,
                                                                                  CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?> { ["customPlanId"] = customPlanId, };

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/changeSimPlan", parameters, cancellationToken);
    }

    /// <summary>
    /// Updates the sim tag
    /// </summary>
    /// <param name="tag">Tag for the SIM card</param>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> UpdateSimTagAsync(string tag,
                                                                                 string? msisdn = null,
                                                                                 string? iccid = null,
                                                                                 CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?> { ["tag"] = tag, };

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/updateSimTag", parameters, cancellationToken);
    }

    /// <summary>
    /// Recharge a sim
    /// </summary>
    /// <param name="amount">The amount in MB (Mega Bytes)</param>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
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

    /// <summary>
    /// Asynchronous export of CDR for the selected SIM in the time range.
    /// The result is send via email.
    /// </summary>
    /// <param name="msisdnList">Sim numbers</param>
    /// <param name="start">Start date of the range</param>
    /// <param name="end">End date of the range</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BasicResponse>> DownloadCdrAsync(List<string> msisdnList,
                                                                            DateTimeOffset? start = null,
                                                                            DateTimeOffset? end = null,
                                                                            CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["msisdnList"] = string.Join(",", msisdnList),
        };

        if (start is not null) parameters["startDateRange"] = start.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (end is not null) parameters["endDateRange"] = end.Value.ToString("yyyy-MM-dd HH:mm:ss");

        return await PostAsync<BasicResponse>("/services/business-api/downloadCdr", parameters, cancellationToken);
    }

    /// <summary>
    /// Get paginated CDRs for the selected SIM in the time range.
    /// </summary>
    /// <param name="msisdnList">Sim numbers</param>
    /// <param name="start">Start date of the range</param>
    /// <param name="end">End date of the range</param>
    /// <param name="page">page number for SIM's CDR</param>
    /// <param name="pageSize">CDR number per page</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<CdrPaginated>> GetCdrAsync(List<string> msisdnList,
                                                                      DateTimeOffset? start = null,
                                                                      DateTimeOffset? end = null,
                                                                      int? page = null,
                                                                      int? pageSize = null,
                                                                      CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["msisdnList"] = string.Join(",", msisdnList),
        };

        if (start is not null) parameters["startDateRange"] = start.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (end is not null) parameters["endDateRange"] = end.Value.ToString("yyyy-MM-dd HH:mm:ss");
        if (page is not null) parameters["page"] = page.Value.ToString();
        if (pageSize is not null) parameters["pageSize"] = Math.Min(pageSize.Value, 500).ToString();

        return await PostAsync<CdrPaginated>("/services/business-api/getCdrPaginated", parameters, cancellationToken);
    }

    /// <summary>
    /// Deactivate sim
    /// </summary>
    /// <param name="msisdn">MSISDN for the SIM card</param>
    /// <param name="iccid">ICCID for the SIM card</param>
    /// <param name="cancellationToken">The token for cancelling the task</param>
    public async Task<ThingsMobileResponse<BaseResponseModel>> DeactivateSimAsync(string? msisdn = null,
                                                                                  string? iccid = null,
                                                                                  CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string?>();

        if (!string.IsNullOrWhiteSpace(msisdn)) parameters["msisdn"] = msisdn;
        else if (!string.IsNullOrWhiteSpace(iccid)) parameters["iccid"] = iccid;
        else throw new InvalidOperationException($"Either '{nameof(msisdn)}' or '{nameof(iccid)}' is required.");

        return await PostAsync<BaseResponseModel>("/services/business-api/deactivateSim", parameters, cancellationToken);
    }

    private async Task<ThingsMobileResponse<T>> PostAsync<T>(string path, Dictionary<string, string?>? parameters = null, CancellationToken cancellationToken = default)
        where T : BaseResponseModel
    {
        // ensure there are parameters
        parameters ??= [];

        // add authentication parameters
        parameters.Add("username", options.Username);
        parameters.Add("token", options.Token);

        // form the content and request
        var nvc = parameters.Select(kvp => new KeyValuePair<string?, string?>(kvp.Key, kvp.Value));
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new FormUrlEncodedContent(nvc)
        };

        // send the request
        var response = await httpClient.SendAsync(request, cancellationToken);

        // extract the response
#if NET5_0_OR_GREATER
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
#else
        using var stream = await response.Content.ReadAsStreamAsync();
#endif
        var error = default(ThingsMobileErrorResponse);
        var resource = default(T);

        if (response.IsSuccessStatusCode)
        {
            var serializer = new XmlSerializer(typeof(T));
            resource = (T?)serializer.Deserialize(stream);
        }
        else
        {
            error = (ThingsMobileErrorResponse?)errorSerializer.Deserialize(stream);
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
