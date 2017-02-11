using SH.Contracts;
using SH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SH.Api.Controllers
{
    [RoutePrefix("api/country")]
    public class CountryController : ApiController
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [Route]
        public async Task<IEnumerable<CountryModel>> Get(CancellationToken cancellationToken)
        {
            return await _countryService.GetAsync(cancellationToken);
        }
    }
}
