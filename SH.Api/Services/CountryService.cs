using SH.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SH.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SH.Api.Services
{
    public class CountryService : ICountryService
    {
        public Task<IEnumerable<CountryModel>> GetAsync(CancellationToken cancellationToken)
        {
            IEnumerable<CountryModel> countries = new[] { new CountryModel { Id = 1, Name = "Ukraine" } };
            return Task.FromResult(countries);
        }
    }
}