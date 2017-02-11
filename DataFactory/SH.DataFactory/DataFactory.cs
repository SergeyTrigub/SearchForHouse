using NLog;
using SH.DataFactory.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.DataFactory
{
    public class DataFactory : IDataFactory
    {
        private readonly IEnumerable<IDataSource> _sources;
        private readonly IEnumerable<IDataWriter> _writers;
        private readonly ILogger _logger;

        public DataFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<IDataSource> Sources
        {
            get
            {
                return _sources;
            }
        }

        public IEnumerable<IDataWriter> Writers
        {
            get
            {
                return _writers;
            }
        }

        /// <summary>
        ///  Processing Tasks as They Complete
        /// </summary>
        /// <returns></returns>
        public async Task ProcessAsync()
        {
            Task sourcesAllTasks = null;
            try
            {
                var tasksSources = _sources.Select(async t => {
                    var data = await t.GetDataAsync().ConfigureAwait(false);

                    var tasksWriters = _writers.Select(async w =>
                    {
                        await w.WriteAsync(data).ConfigureAwait(false);
                    });

                    await Task.WhenAll(tasksWriters).ConfigureAwait(false);
                });

                sourcesAllTasks = Task.WhenAll(tasksSources);
                await sourcesAllTasks.ConfigureAwait(false);
            }
            catch
            {
                AggregateException allExceptions = sourcesAllTasks.Exception;
                _logger.Error(allExceptions, "Data ProcessAsync error");
                throw allExceptions;
            }
        }
    }
}
