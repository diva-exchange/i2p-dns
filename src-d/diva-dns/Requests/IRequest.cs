using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diva_dns.Requests
{
    public interface IRequest
    {
        HttpResponseMessage? ResponseMessage { get; }

        bool SendAndWaitForAnswer(int timeout_in_ms = -1);
    }
}
