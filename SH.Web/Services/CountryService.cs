using SH.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SH.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SH.Web.Services
{
    public class CountryService : BaseApiService, ICountryService
    {
        public async Task<IEnumerable<CountryModel>> GetAsync(CancellationToken cancellationToken)
        {
            var url = "/api/country";
            return await GetAsync<IEnumerable<CountryModel>>(url, cancellationToken).ConfigureAwait(false);
        }
    }
}