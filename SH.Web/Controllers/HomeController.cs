using SH.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SH.Web.Controllers
{
    public class HomeController : Controller
    {
        private ICountryService _countryService;

        public HomeController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _countryService.GetAsync(cancellationToken);
            return View();
        }

        async Task WaitAsync()
        {
            await Task.Delay(1);
        }

        static Task<T> CustomException<T>()
        {
            var tsc = new TaskCompletionSource<T>();
            tsc.SetException(new NotImplementedException());
            return tsc.Task;
        }

        public async Task<ActionResult> About()
        {
            await WaitAsync();
            await ProcessTasksAsync();

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        static async Task<int> DelayAndReturnAsync(int val)
        {
            await Task.Delay(TimeSpan.FromSeconds(val));
            return val;
        }

        // Currently, this method prints "2", "3", and "1".
        // We want this method to print "1", "2", and "3".
        static async Task ProcessTasksAsync()
        {
            // Create a sequence of tasks.
            Task<int> taskA = DelayAndReturnAsync(2);
            Task<int> taskB = DelayAndReturnAsync(3);
            Task<int> taskC = DelayAndReturnAsync(1);
            var tasks = new[] { taskA, taskB, taskC };

            var processingTasks = tasks.Select(async t =>
            {
                var res = await t;
                Debug.WriteLine(res);
            }).ToArray();

            await Task.WhenAll(processingTasks);
            // Await each task in order.
            //foreach (var task in tasks)
            //{
            //    var result = await task;
            //    Debug.WriteLine(result);
            //}
        }
    }
}