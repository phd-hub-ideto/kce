using System.ComponentModel;

namespace KhumaloCraft.Domain.Orders;

public enum OrderStatus
{
    [Description("Awaiting payment")]
    AwaitingPayment = 1,

    [Description("Cancelled")]
    Cancelled,

    [Description("Completed")]
    Completed,

    [Description("Declined")]
    Declined,

    [Description("Pending")]
    Pending,

    [Description("Shipped")]
    Shipped
}