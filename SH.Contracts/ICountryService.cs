using SH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.Contracts
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryModel>> GetAsync(CancellationToken cancellationToken);
    }
}
