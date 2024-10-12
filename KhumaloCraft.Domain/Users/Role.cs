using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhumaloCraft.Domain.Users;

using System.ComponentModel;

public enum Role
{
    [Description("Super User")]
    SuperUser = 1,

    [Description("Receptionist")]
    Receptionist = 2,

    [Description("Operational Support")]
    OperationalSupport = 3,

    [Description("KC User")]
    KCUser = 4
}