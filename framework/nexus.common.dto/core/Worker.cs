using System;
using System.Data; 
using System.Linq;
using System.Text;
using System.Collections.Generic;

using nexus.common.core;
using nexus.common.dal;

namespace nexus.common.core
{
    public class Worker : nexus.common.dal.dlWorker 
    {
        public enum WorkerChangeType { offline, invalid, valid, active, error };

        public event         OnWorkerStatusChangeHandler OnWorkerStatusChange;
        public delegate void OnWorkerStatusChangeHandler(WorkerChangeType reason, string Worker, int WorkerID, int WorkerSecurityClearance);

    }
}
