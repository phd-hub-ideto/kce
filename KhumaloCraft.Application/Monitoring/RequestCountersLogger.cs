using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.UserEnvironment;
using KhumaloCraft.Http;
using System.Diagnostics;

namespace KhumaloCraft.Application.Monitoring;

[Singleton]
public class RequestCountersLogger
{
    private static bool _registered;
    private readonly IRequestStorage _requestStorage;
    private readonly IHttpResponseProvider _httpResponseProvider;
    private readonly IEnvironmentService _environmentService;
    private readonly IHttpContextProvider _httpContextProvider;
    private readonly IHttpRequestProvider _httpRequestProvider;
    private readonly ITemplateNameProvider _templateNameProvider;

    public RequestCountersLogger(
        IRequestStorage requestStorage,
        IHttpResponseProvider httpResponseProvider,
        IEnvironmentService environmentService,
        IHttpContextProvider httpContextProvider,
        IHttpRequestProvider httpRequestProvider,
        ITemplateNameProvider templateNameProvider)
    {
        _requestStorage = requestStorage;
        _httpResponseProvider = httpResponseProvider;
        _environmentService = environmentService;
        _httpContextProvider = httpContextProvider;
        _httpRequestProvider = httpRequestProvider;
        _templateNameProvider = templateNameProvider;

        if (!_registered)
        {
            _registered = true;
        }
    }

    public void Start()
    {
        var timers = _requestStorage.Get<RequestCountersContext>();

        timers.PageTimer = Stopwatch.StartNew();
    }

    public void Log()
    {
        var timers = _requestStorage.Get<RequestCountersContext>();

        if (_httpResponseProvider.ResponseIsAvailable)
        {
            if (_templateNameProvider.TryGetTemplateName(out var templateName))
            {
                _httpResponseProvider.SetHeader("X-Template", templateName);

                var metrics = new List<(string Name, string Value)>
                {
                    ("Sql",                     $"{timers.SqlCalls}"),
                    ("Elasticsearch",           $"{timers.SearchEngineCalls}"),
                    ("React",                   $"{timers.ReactCalls}"),
                    ("SqlDuration",             $"{(int)timers.SqlDuration.TotalMilliseconds}"),
                    ("ElasticsearchDuration",   $"{(int)timers.SearchEngineDuration.TotalMilliseconds}"),
                    ("ReactDuration",           $"{(int)timers.ReactDuration.TotalMilliseconds}")
                };

                _httpResponseProvider.SetHeader("X-TemplateMetricNames", string.Join(",", metrics.Select(item => item.Name)));
                _httpResponseProvider.SetHeader("X-TemplateMetrics", string.Join(",", metrics.Select(item => item.Value)));
            }

            _httpResponseProvider.SetHeader("X-UniqueUserId", _environmentService.GetEnvironment().UniqueUserId);

            if (_httpRequestProvider.TryGetHeader("X-RequestId", out var value))
            {
                _httpResponseProvider.SetHeader("X-RequestId", value);
            }
            else
            {
                _httpResponseProvider.SetHeader("X-RequestId", _httpContextProvider.RequestId);
            }
        }

    }
}