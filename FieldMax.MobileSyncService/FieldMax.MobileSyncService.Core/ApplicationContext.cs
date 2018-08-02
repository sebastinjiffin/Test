using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldMax.AsyncService.Core
{
    public static class ApplicationContext
    {
        private static Queue<SyncDbDetail> _jobCollection { get; set; } = new Queue<SyncDbDetail>();
        private static int _currentCapacity { get; set; } = 0;
        private static readonly int _capacity;
        private static IConfigurationRoot _provider { get; set; }

        private static void LoadConfiguration()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(currentPath);
            builder.AddJsonFile("mssconfig.json");
            _provider = builder.Build();
        }

        public static IConfigurationSection GetBySection(string section, bool isReload = false)
        {
            if (_provider == null || isReload)
            {
                LoadConfiguration();
            }

            return _provider.GetSection(section);
        }

        public static void Consume()
        {
            _currentCapacity--;
        }

        public static void Spend()
        {
            _currentCapacity++;
        }

        public static int GetCapacity()
        {
            return _currentCapacity;
        }

        public static void EnQueue(SyncDbDetail detail)
        {
            _jobCollection.Enqueue(detail);
        }

        public static SyncDbDetail DeQueue()
        {
            return _jobCollection.Dequeue();
        }

        public static SyncDbDetail Peek()
        {
            return _jobCollection.Peek();
        }

        public static Queue<SyncDbDetail> GetQueue()
        {
            return _jobCollection;
        }
    }
}
