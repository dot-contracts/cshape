
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Dispatching;
using System;
using Windows.UI;
using Windows.Foundation;

using nexus.common.control.Themes;
using System.Data;

namespace nexus.common.control
{

    public enum NxDialogTypes {List, Date, Time, DateTime }

    public partial class NxComboDialog : NxDialogBase
    {
        //private NxDialogPanel  _dialog;
        private NxCalendar   _calendar;
        private NxTime       _time;
        private NxList       _list;

        public delegate void ListSelectionChangedEventHandler(string Display, string Value); public event ListSelectionChangedEventHandler OnListSelectionChanged;

        public NxComboDialog(NxDialogTypes DialogTypes)
        {
            var rootGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Stretch
            };

            switch (DialogTypes)
            {
                case NxDialogTypes.List:
                    _list = new NxList();
                    rootGrid.Children.Add(_list);
                    _list.OnChanged += _OnChanged;
                    break;
                case NxDialogTypes.Date:
                    _calendar = new NxCalendar
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Value = DateTime.Today
                    };
                    rootGrid.Children.Add(_calendar);
                    _calendar.OnChanged += _OnChanged;
                    break;

                case NxDialogTypes.Time:
                    _time = new NxTime
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment    = VerticalAlignment.Top,
                        Value = DateTime.Now.ToString("HH:mm")
                    };
                    rootGrid.Children.Add(_time);
                    _time.OnChanged += _OnChanged;
                    break;

                case NxDialogTypes.DateTime:
                    break;
                default:
                    break;
            }

            this.Content = rootGrid;
        }

        private void _OnChanged(object? sender, ChangedEventArgs e)
        {
            RaiseOnChanged(e.Value);
            Close();
        }

        public void setListData(DataTable dataTable, string value, NxList.GridTypes type = NxList.GridTypes.DropList, string DisplayPath = "Description", string ValuePath = "Id")
        {
            _list.SetDataTable(dataTable, DisplayPath, ValuePath, type, value);
        }
    }
}
