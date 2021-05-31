using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAM2.Shared.Shared.Interface
{
    public interface ITokenFactory
    {
	    CancellationToken Get(TimeSpan timeSpan);

	    CancellationToken GetDefault();
    }
}
