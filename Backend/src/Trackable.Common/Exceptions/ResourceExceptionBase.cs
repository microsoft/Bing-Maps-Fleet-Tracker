using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Common.Exceptions
{
    public class ResourceExceptionBase : ExceptionBase
    {
        public ResourceExceptionBase(string userFriendlyMessage)
            : base(userFriendlyMessage)
        {
        }
    }
}
