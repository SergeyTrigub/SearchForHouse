using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.DataFactory.Contracts
{
    public interface IDataWriter
    {
        Task WriteAsync(byte[] data);
    }
}
