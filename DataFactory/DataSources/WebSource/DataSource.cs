using SH.DataFactory.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSource
{
    public class DataSource : IDataSource
    {
        public async Task<byte[]> GetDataAsync()
        {
            byte[] b = new byte[0];
            return await Task.FromResult(b).ConfigureAwait(false);
        }
    }
}
