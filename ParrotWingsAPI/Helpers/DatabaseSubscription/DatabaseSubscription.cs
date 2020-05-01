namespace ParrotWingsAPI
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using TableDependency.SqlClient;
    using TableDependency.SqlClient.Base.Enums;
    using TableDependency.SqlClient.Base.EventArgs;

    public abstract class DatabaseSubscription<THub,  TTable> : IDatabaseSubscription
        where THub : Hub
        where TTable : class , new() 
    {
        private bool disposedValue = false;
        private SqlTableDependency<TTable> _tableDependency;
        protected readonly IHubContext<THub> HubContext;
        protected readonly DmlTriggerType NotifyOn;

        public DatabaseSubscription(IHubContext<THub> hubContext, DmlTriggerType notifyOn)
        {
            HubContext = hubContext;
            NotifyOn = notifyOn;
        }

        public void Configure(string connectionString)
        {
            _tableDependency = new SqlTableDependency<TTable>(connectionString, notifyOn: NotifyOn);
            _tableDependency.OnChanged += OnChanged;
            _tableDependency.OnError += OnError;
            _tableDependency.Start();
        }

        protected abstract void OnError(object sender, ErrorEventArgs e);

        protected abstract void OnChanged(object sender, RecordChangedEventArgs<TTable> e);

        #region IDisposable

        ~DatabaseSubscription()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tableDependency.Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

}
