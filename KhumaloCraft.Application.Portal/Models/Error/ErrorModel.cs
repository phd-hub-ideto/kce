using KhumaloCraft.Application.Models;
using System.Net;

namespace KhumaloCraft.Application.Portal.Models.Error;

public class ErrorModel
{
    private static readonly Dictionary<HttpStatusCode, ErrorModel> _errorsMap = new Dictionary<HttpStatusCode, ErrorModel>
    {
        {
            HttpStatusCode.NotFound,
            new ErrorModel
            {
                H1 = "Page not found",
                Description = "The page you are trying to reach does not exist",
            }
        },
        {
            HttpStatusCode.Forbidden,
            new ErrorModel
            {
                H1 = "Forbidden",
                Description = "You do not have permission to access this page",
            }
        },
        {
            HttpStatusCode.InternalServerError, new ErrorModel
            {
                H1 = "Something went wrong",
                Description = "The server is unable to process your request.",
            }
        }
    };

    private const string ReturnToHomePage = "Return to home page";

    public string H1 { get; private set; }
    public string Description { get; private set; }
    public HtmlAnchorData HomeAnchor => new HtmlAnchorData(ReturnToHomePage, "/");

    public string ErrorReference { get; private set; }

    public static ErrorModel Create(HttpStatusCode httpStatusCode, string errorReference)
    {
        if (_errorsMap.TryGetValue(httpStatusCode, out var errorModel))
        {
            errorModel.ErrorReference = errorReference;

            return errorModel;
        }

        throw new NotImplementedException($"Unhandled ${nameof(HttpStatusCode)}: ${httpStatusCode}");
    }

    public static ErrorModel Create(string errorReference)
    {
        return new ErrorModel { ErrorReference = errorReference, };
    }
}