using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Windows.Foundation;

using nexus.common;
using nexus.common.control.Themes;

namespace nexus.common.control
{
    public partial class NxTabBase : NxControlBase, IPlugin
    {
        public  PropertyBag<NameValue> PropBag;
        private bool   initialized = false;        // Flag indicating that all required initializations have been successfully performed
        private WinSize   mMinSize    = new WinSize(0, 0);
        private WinSize   mMaxSize    = new WinSize(0, 0);

        private System.Timers.Timer mTimer;
        private delegate void TimerCB();
        public  int    LoadState          { get; set; }
        public  string LastPanel          { get; set; }
        public  int    MaxLoadLoops       { get; set; }
        public  string LastActions;

        #region Public Properties
        public  bool                IsBusy          { get; set; }
        public  bool                IsActive        { get; set; }
        public  bool                IsChanged       { get; set; }
        public  bool                IsLoadComplete  { get; set; }
        #endregion

        #region Events
        public delegate void  OnLoadLoopEventHandler       ();                                           public event OnLoadLoopEventHandler       LoadLoop;
        public delegate void  OnStartLoadEventHandler      (int MaxLoops);                               public event OnStartLoadEventHandler      OnStartLoad;
        public delegate void  OnLoadingEventHandler        (int Progress, string Message);               public event OnLoadingEventHandler        OnProgress;
        public delegate void  OnLoadedEventHandler         ();                                           public event OnLoadedEventHandler         OnLoaded;

        //public delegate void  OnPluginEventHandler(NexusEnums.EventTypes reason, string sender, string command, string Property);  public event OnPluginEventHandler OnPluginEvent; 

        #endregion


        #region Constructors
        public NxTabBase()
        {
            PropBag = new PropertyBag<NameValue>();
            this.SizeChanged += NxPluginBase_SizeChanged;
        }

        #endregion

        //public void SetActionMenu(string Sender, string Actions)
        //{
        //    LastActions = Actions;
        //    RaisePluginEvent(NexusEnums.EventTypes.snapin, Sender, "Actions", Actions);
        //}


        //protected virtual void RaisePluginEvent(NexusEnums.EventTypes reason, string sender, string command, string Property)
        //{
        //    OnPluginEvent?.Invoke(reason, sender, command, Property);
        //}

        #region IPlugin Implementation

        public bool Create(PropertyBag<NameValue> propBag)
        {
            PropBag = propBag;

            mTimer = new System.Timers.Timer();
            return true;
        }

        public bool Load(int LoadState, string Property)
        {
            throw new NotImplementedException();
        }

        //public bool Show()
        //{
        //    return true;
        //}

        public void Reset ()
        {
            throw new NotImplementedException();
        }

        public bool Save ()
        {
            throw new NotImplementedException();
        }

        public bool Process()
        {
            throw new NotImplementedException();
        }

        public bool Execute(WinRect ScreenRect, enums.EventTypes Reason, string SourcePlugin, string Command, string Parameters = "", string DestinationPlugin = "")
        {
            throw new NotImplementedException();
        }

        public void Resize(WinSize PanelSize) { }

        public WinSize MinSize()
        {
            return mMinSize;
        }

        public WinSize MaxSize()
        {
            return mMaxSize;
        }

        public void TimerStart(int maxLoadLoops)
        {
            LoadState = 0;
            MaxLoadLoops = maxLoadLoops;
            if (OnStartLoad != null) OnStartLoad(MaxLoadLoops);

            if (mTimer == null)
                mTimer = new System.Timers.Timer();

            mTimer.AutoReset = false;
            mTimer.Interval = 100;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTimer_Elapsed);
            mTimer.Start();

        }


        private void mTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(ProcessTimer);
        }
        private async void ProcessTimer()
        {
            mTimer.Stop();
            if (!IsBusy)
            {
                IsBusy = true;

                try { LoadLoop(); }
                catch (Exception ex)
                {

                    LoadState = MaxLoadLoops;
                }
                IsBusy = false;
            }

            await Task.Yield();

            if (LoadState < MaxLoadLoops) mTimer.Start();
        }
        public void ProcessLoop(int loadState, string Message)
        {
            LoadState = loadState;
            if (OnProgress != null) OnProgress(LoadState, Message);
        }


        public void SetSize(WinSize MinSize, WinSize MaxSize)
        {
            mMinSize = MinSize;
            mMaxSize = MaxSize;
        }


        private void NxPluginBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mMinSize.Height + mMinSize.Width == 0)
            {
                mMinSize = new WinSize(this.ActualWidth, this.ActualHeight);
                mMaxSize = new WinSize(this.ActualWidth, this.ActualHeight);
            }
        }

        public void ReSize( WinSize PanelSize)
        {
        }


        //public bool ScaleToParent(NxMainUIBase Panel)
        //{

        //    bool Scaled = false;

        //    UpdateLayout();

        //    try
        //    {
        //        double ScaleX = 1;
        //        double ScaleY = 1;

        //        if (this.ActualWidth > 0 & this.ActualHeight > 0)
        //        {
        //            if (mMinSize.Width > 0 & Panel.ActualWidth < mMinSize.Width)
        //            {
        //                ScaleX = Panel.ActualWidth / this.ActualWidth;
        //            }
        //            else if (mMaxSize.Width > 0 & Panel.ActualWidth > mMaxSize.Width)
        //            {
        //                ScaleX = Panel.ActualWidth / this.ActualWidth;
        //            }

        //            if (mMinSize.Height > 0 & Panel.ActualHeight < mMinSize.Height)
        //            {
        //                ScaleY = Panel.ActualHeight / this.ActualHeight;
        //            }
        //            else if (mMaxSize.Height > 0 & Panel.ActualHeight > mMaxSize.Height)
        //            {
        //                ScaleY = Panel.ActualHeight / this.ActualHeight;
        //            }

        //            if (ScaleX != 1 | ScaleY != 1)
        //            {
        //                // Child.RenderTransformOrigin = New WinPoint(0.5, 0.5)

        //                var scaleTransform = new ScaleTransform
        //                {
        //                    ScaleX = ScaleX,
        //                    ScaleY = ScaleY
        //                };

        //                Scaled = true;
        //            }
        //        }

        //        UpdateLayout();


        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return Scaled;

        //}


        #endregion

        #region IDisposable Implementation
        #endregion
    }
}
