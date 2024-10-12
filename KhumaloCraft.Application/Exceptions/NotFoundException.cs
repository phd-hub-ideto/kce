using System.Net;

namespace KhumaloCraft.Application.Exceptions;

public class NotFoundException : HttpException
{
    public NotFoundException()
        : this("Not found") { }

    public NotFoundException(string msg)
        : base((int)HttpStatusCode.NotFound, msg)
    {

    }
}

public class CraftNotFoundHttpException : NotFoundException
{
    public int CraftId { get; }

    public CraftNotFoundHttpException(int craftId)
        : base("Craft not found")
    {
        CraftId = craftId;
    }
}