using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pragmasoft.QuickpayV10.Extensions.Services.Interfaces
{
    public interface IQuickPayV10TestcardNumbersProvider
    {
        IEnumerable<String> GetCardNumbers();
    }
}
