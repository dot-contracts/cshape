using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using Windows.System;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

using nexus.common;
using nexus.common.control;


namespace nexus.common.control
{

    public class AutoCompleteArgs : EventArgs
        {
            private string      pattern;
            private IEnumerable<DataTable> dataSource;
            private bool        cancelBinding = false;
           
            public AutoCompleteArgs(string Pattern)
            { this.pattern = Pattern; }

            public string Pattern
            { get { return this.pattern; } }

            public IEnumerable<DataTable> DataSource
            { get { return this.dataSource; } set { this.dataSource = value; } }

            public bool CancelBinding
            { get { return this.cancelBinding; } set { this.cancelBinding = value; } }

        }

    public partial class NxComboBase : NxControlBase
    {
        //private bool            initialized = false;        // Flag indicating that all required initializations have been successfully performed


        private NxComboDialog     mDrop;

        public enum DropTypes { None, List, Option, Options, Date, Time, DateTime, Calculator, External, Internal }

        private string    mList             = "";    public string    List             { get { return mList; }             set { mList = value; SetDropList(mList); } }
        private DataTable mData;                     public DataTable Data             { get { return mData; }             set { mData = value; } }
        private string    mDisplayPath      = "";    public string    DisplayPath      { get { return mDisplayPath; }      set { mDisplayPath = value; } }
        private string    mValuePath        = "";    public string    ValuePath        { get { return mValuePath; }        set { ValuePath = value; } }
        private bool      mDataLoaded       = false; public bool      DataLoaded       { get { return mDataLoaded; }       set { mDataLoaded = value; } }
        private bool      mAllowMultiSelect = false; public bool      AllowMultiSelect { get { return mAllowMultiSelect; } set { mAllowMultiSelect = value;}}


        private bool      mIsMouseDown      = false;
        private bool      mIsValidated      = false;
        private bool      mIsKeyEvent       = false;
        private bool      mClearOnEmpty     = true;
        private bool      mDropCancel       = false;


        private bool      mDropShown        = false;           public     bool      DropShown        { get { return mDropShown; }        set { mDropShown = value; } }
        private DropTypes mDropType         = DropTypes.List;  public     DropTypes DropType         { get { return mDropType; }         set { mDropType = value; } }
        private string    mDropPrompt       = "";              public     string    DropPrompt       { get { return mDropPrompt; }       set { mDropPrompt = value; } }


        #region Public Property

        private string mPrompt;       public string Prompt       { get { return mPrompt; }       set { mPrompt = value; } }
        private string mName;         public string Name         { get { return mName; }         set { mName = value; } }
        private string mActionCode;   public string ActionCode   { get { return mActionCode; }   set { mActionCode = value; } }

        private bool   mAllowChange;  public bool   AllowChange  { get { return mAllowChange; }  set { mAllowChange = value; } }

        //private bool   mIsActive;     public bool IsActive       { get { return mIsActive; }     set { mIsActive = value;    if (OnActive != null)   OnActive(mIsActive); } }
        //private bool   mIsValid;      public bool IsValid        { get { return mIsValid; }      set { mIsValid = value;     if (OnValid != null)    OnValid(mIsValid); } }
        private bool   mIsHidden;     public bool IsHidden       { get { return mIsHidden; }     set { mIsHidden = value; } }

        //public  Boolean         IsInitialized   { get { return ( initialized ); } }

        #endregion

        #region Value Dependency Property

        private bool   mSetValue = false;

        private string mDisplay = "";
        public  string  Display
        {
            get { return mDisplay; }
            set { mDisplay = value; if (mDisplay != null) { mValue = FindValue(mDisplayPath, mDisplay, mValuePath); } }
        }

        private string mValue = "";
        public  string  Value
        {
            get { return string.IsNullOrEmpty(mValue) ? "" : mValue; }
            set { mValue = value; if (mValue != null) { mDisplay = FindValue(mValuePath, mValue, mDisplayPath); } }
        }

        public int ValueInt { get { int ret; int.TryParse(mValue, out ret); return ret; } }


        #endregion

        
        private List<object> mItems = new();
        public IList<object> Items => mItems;

        private int mSelectedIndex = -1;
        public int SelectedIndex
        {
            get => mSelectedIndex;
            set
            {
                if (value >= 0 && value < mItems.Count)
                {
                    mSelectedIndex = value;
                    var selected = mItems[value];
                    Value = selected?.ToString() ?? "";
                    Display = Value;
                }
            }
        }

        public object? SelectedItem
        {
            get => mSelectedIndex >= 0 && mSelectedIndex < mItems.Count ? mItems[mSelectedIndex] : null;
            set
            {
                var index = mItems.IndexOf(value);
                if (index >= 0) SelectedIndex = index;
            }
        }


#region Constructors
        public NxComboBase()
        {
            IsTabStop  = false;
            //Focusable = false;
            //IsEditable = false; // by default
            //mSearchTimer = new Timer(this.delay);
            //mSearchTimer.AutoReset = true;

            //mSearchTimer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);

        }
        #endregion


        public void LoadEnum(Type enumType, bool showIndex = true)
        {
            mData = new DataTable();
            mData.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));
            mData.Columns.Add(new DataColumn("Id",          Type.GetType("System.String")));
            mData.AcceptChanges();

            foreach (var value in Enum.GetValues(enumType))
            {
                var name = value.ToString();
                var index = (int)value;

                DataRow nr = mData.NewRow();
                nr["Description"] = value.ToString();
                nr["Id"]          = (int)value;
                mData.Rows.Add(nr);
            }
            SetDataTable(mData);
        }

        public void SetDropList(string data)
        {

            mData = new DataTable();
            mData.Columns.Add(new DataColumn("Description", Type.GetType("System.String")));
            mData.Columns.Add(new DataColumn("Id",          Type.GetType("System.String")));
            mData.AcceptChanges();

            Array lArr = data.Split(';');
            for (int i = 0; i < lArr.GetLength(0); i++)
            {
                Array cArr = (lArr.GetValue(i).ToString() + ",").Split(',');
                DataRow nr = mData.NewRow();

                nr["Description"] = cArr.GetValue(0).ToString();
                nr["Id"]          = (String.IsNullOrEmpty(cArr.GetValue(1).ToString())) ? cArr.GetValue(0).ToString() : cArr.GetValue(1).ToString();
                mData.Rows.Add(nr);
            }

            SetDataTable(mData);

        }


        public void SetDataTable(DataTable Data) { SetDataTable(Data, "Description", "Id"); }
        public void SetDataTable(DataTable Data, string DisplayPath, string ValuePath)
        {
            mData        = Data;
            mValuePath   = ValuePath;
            mDisplayPath = DisplayPath;
        }

        public bool HasRows()
        {
            bool ret = false;
            if (mData != null) ret = mData.Rows.Count>0;
            return ret;
        }

        public string FindValue(string TestPath, string TestValue, string ReturnPath)
        {
            if (!string.IsNullOrEmpty(TestPath))
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    TestValue = TestValue.ToUpper();
                    for (int i = 0; i <= Data.Rows.Count - 1; i++)
                    {
                        if (TestValue.Equals (Data.Rows[i][TestPath].ToString().ToUpper())) { return Data.Rows[i][ReturnPath].ToString(); }
                    }
                }

            }
            return TestValue;
        }



        public string FindValueFromDisplay(string Display)
        {
            if (Data == null) return Display;
            if (!string.IsNullOrEmpty(Display))
            {
                int       i = 0;
                string Test = null;

                Display = Display.ToUpper();
                for (i = 0; i <= Data.Rows.Count - 1; i++)
                {
                    Test = Data.Rows[i][mDisplayPath].ToString().ToUpper();
                    if (Test.Length > Display.Length) Test = Test.Substring(0, Display.Length);
                    if (Test.Contains(Display))
                        return Data.Rows[i][mValuePath].ToString();
                }
            }
            return Display;
        }

        public string FindDisplayFromValue(string Value)
        {
            if (Data == null) return Value;
            if (!string.IsNullOrEmpty(Value))
            {
                int i = 0;
                string Test = null;

                Value = Value.ToUpper();
                for (i = 0; i <= Data.Rows.Count - 1; i++)
                {
                    Test = Convert.ToString(Data.Rows[i][mValuePath]).ToUpper();
                    if (Test.Equals(Value)) return Data.Rows[i][mDisplayPath].ToString();
                }
            }
            return Display;
        }

        public string GetSelectedItems()
        {
            mValue   = String.Empty;
            mDisplay = String.Empty;

            //for (int i = 0; i <= Data.Rows.Count - 1; i++)
            //{
            //    if (!Data.Rows[i].IsNull(mValuePath))
            //    {
            //        Test = Convert.ToString(Data.Rows[i][Column]).ToUpper();
            //        if (Value == Test) { return i; }
            //    }
            //}

            //if (List.SelectedItems != null)
            //{
            //    foreach (object item in List.SelectedItems)
            //    {
            //        Type mType = item.GetType();
            //        if (mType == typeof(CodeTypes))
            //        {
            //            CodeTypes sel = (CodeTypes)item;

            //            if (!String.IsNullOrEmpty(mValue)) mValue += ",";
            //            mValue += sel.Id.ToString();

            //            if (!String.IsNullOrEmpty(mDisplay)) mDisplay += ",";
            //            mDisplay += sel.Name.ToString();
            //        }
            //    }

            //    Value = mValue;
            //    Editor.Value = mDisplay;
            //}
            return mValue;
        }




        //public void SetList(string Data)
        //{ if (List != null) List.SetList(Data); }

        protected DataTable PatternChanged(object sender, AutoCompleteArgs args)
        {
            //if (string.IsNullOrEmpty(args.Pattern))
            //    args.CancelBinding = true;
            //else
            //{
            //    DataTable newTable = GetDataSource(args.Pattern);
            //    return newTable;

            //}
            return null;
        }
        //protected DataTable GetDataSource(string pattern)
        //{
        //    if (List != null) return List.FilterList(pattern, mDisplayPath);
        //    return null;
        //}


        public bool ShowDrop(Windows.Foundation.Rect ScreenRect)
        {
            if (DropShown) HideDrop();
            mDropShown = true;

            switch (mDropType)
            {
                case DropTypes.List:      mDrop = new NxComboDialog(NxDialogTypes.List);       mDrop.setListData(mData, Value); break;
                case DropTypes.Date:      mDrop = new NxComboDialog(NxDialogTypes.Date);                                        break; 
                case DropTypes.DateTime:  mDrop = new NxComboDialog(NxDialogTypes.DateTime);                                    break;
                case DropTypes.Time:      mDrop = new NxComboDialog(NxDialogTypes.Time);                                        break;
                case DropTypes.Option:
                case DropTypes.Options:   mDrop = new NxComboDialog(NxDialogTypes.Date);                                        break;
            }

            mDrop.OnChanged += _OnChanged;
            if (mDrop == null) return false;

            mDrop.ShowBelow(ScreenRect);

            return !mDropCancel; 

        }


        public void DropClose(ref bool Cancel, string Display, string Value)
        {
            mDropCancel = Cancel;
            if (!Cancel)
            {

                if (!(Value.Equals(mValue) && Display.Equals(mDisplay))) IsChanged = true;

                if (IsChanged)
                {
                    mDisplay = Display;
                    mValue   = Value;
                }

            }
            HideDrop();
        }

        public void HideDrop()
        {
            if (mDrop != null)
            {
                mDrop.Close();
                mDrop = null;
            }
            mDropShown = false;
        }

        protected virtual void _OnChanged(object? sender, ChangedEventArgs e)
        {
            Value = e.Value;
            switch (mDropType)
            {
                case DropTypes.List:
                    Display = FindDisplayFromValue(Value);
                    break;

                case DropTypes.Date:
                    //Display = Value;
                    break;

                case DropTypes.DateTime:
                    break;

                case DropTypes.Time:
                    break;

                case DropTypes.Option:
                case DropTypes.Options:
                    break;
            }
            RaiseOnChanged(e.Value);
        }
    }


    //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    //{
    //    if (!IsReadonly)
    //    {
    //        mIsKeyEvent = false;
    //        mSearchTimer.Stop(); // stop the timer

    //        this.Dispatcher.BeginInvoke((Action)delegate
    //        {
    //            mIsDropDownOpen = true;
    //            AutoCompleteArgs args = new AutoCompleteArgs(Value);
    //            DataTable dt = this.PatternChanged(this, args);
    //            // bind the new datasource
    //            //List.ShowFilterList(dt);

    //        }, DispatcherPriority.ApplicationIdle);
    //    }
    //}
    //public void ResetTimer()
    //{
    //    SearchTimer.Stop();
    //    SearchTimer.Start();
    //}

}
