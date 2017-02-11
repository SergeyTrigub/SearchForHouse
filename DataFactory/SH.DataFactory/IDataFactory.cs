using SH.DataFactory.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.DataFactory
{
    public interface IDataFactory
    {
        IEnumerable<IDataSource> Sources { get; }
        IEnumerable<IDataWriter> Writers { get; }

        Task ProcessAsync();
    }
}
