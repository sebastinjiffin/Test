using FieldMax.MobileSyncService.Core.Business;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FieldMax.AsyncService.Core
{
    public class CoreService
    {
        #region properties

        private int _capacity { get; set; } = 0;
        private int _uploadFrequency { get; set; }
        private int _processFrequency { get; set; }
        private Queue<SyncDbDetail> _jobCollection { get; set; }
        private CancellationTokenSource _source { get; set; }

        private SyncSqLiteService _service
        {
            get { return new SyncSqLiteService(); }
        }

        #endregion

        #region Constructor

        public CoreService()
        {
            _source = new CancellationTokenSource();
            _jobCollection = ApplicationContext.GetQueue();
        }

        #endregion

        #region Public methods
        public void Start()
        {
            //// Start
            LoadConfiguration();
            InitializeConnection();

            // Upload timer
            System.Timers.Timer uploadTimer = new System.Timers.Timer();
            uploadTimer.Elapsed += new ElapsedEventHandler(OnUploadEvent);
            uploadTimer.Interval = _uploadFrequency;
            uploadTimer.Enabled = true;

            // Process timer
            System.Timers.Timer processTimer = new System.Timers.Timer();
            processTimer.Elapsed += new ElapsedEventHandler(OnProcessEvent);
            processTimer.Interval = _processFrequency;
            processTimer.Enabled = true;
        }

        public void Stop()
        {
            _source.Cancel();
            Console.WriteLine("Stop()");
            // write code here that runs when the Windows Service stops.  
        }
        #endregion

        #region private region
        private void InitializeConnection()
        {
            Console.WriteLine("InitializeConnection");
        }

        private void LoadConfiguration()
        {
            Console.WriteLine("LoadConfiguration");
            _capacity = Convert.ToInt32(ApplicationContext.GetBySection("Capasity", true).Value);
            _uploadFrequency = Convert.ToInt32(ApplicationContext.GetBySection("UploadFreq").Value);
            _processFrequency = Convert.ToInt32(ApplicationContext.GetBySection("ProcessFreq").Value);
        }

        private void OnProcessEvent(object sender, ElapsedEventArgs e)
        {
            ////Parallel.For(0, ApplicationContext.GetCapacity(), (i) =>
            ////{
            ////    _service.Process();
            ////});

            // Execute the task currentCapacity times.
            for (int ctr = 0; ctr < ApplicationContext.GetCapacity(); ctr++)
            {
                if(ApplicationContext.Peek() != null)
                {
                    Task.Run(() =>
                    {
                        SyncDbDetail detail = ApplicationContext.DeQueue();
                        _service.Process(detail);
                    }, _source.Token).ContinueWith((i) =>
                    {
                        ApplicationContext.Consume();
                    }, _source.Token);
                }
                
            }

            Console.WriteLine("OnProcessEvent");
            ////throw new NotImplementedException();

        }

        private void OnUploadEvent(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                LoadJobCollectionQueue();

            }, _source.Token).ContinueWith((i) =>
            {
            }, _source.Token);

            Console.WriteLine("OnUploadEvent");
            ////throw new NotImplementedException();

        }

        private void LoadJobCollectionQueue()
        {
            // Get Data from Db

            var data = new SyncDbDetail() { CustomerCode = "1", FileName = "1.zip", Status = 1, User = "sa" };
            _jobCollection.Enqueue(data);
        }

        #endregion
    }
}
