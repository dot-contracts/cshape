using System;
using System.Collections;
using System.Text;

namespace nexus.common.helper
{
    #region IValueRows Interface Definition

    /// <summary>Represents a nongeneric collection of IValueCol collections of key/value pairs</summary>
    public interface IValueRows : ICollection
    {
        /// <summary>Removes all elements from the IValueRows object</summary>
        /// <exception cref="System.NotSupportedException">Whenever encountering any IValueCols object having a <paramref name="State"/> other than ReadWriteDelete</exception>
        void Reset();

        Boolean Read();
        Boolean Write();
        Boolean Delete();
        Boolean Insert();
        Boolean Append();
    }
    #endregion

    [Obsolete] // To be eliminated soon
    public class NameValueList : NameValueSet, IValueRows
    {
        private ValueRows[] Rows;
        private Int16       RowsInUse = 0;

        #region Constructors
        public NameValueList(NameValueSet vSet) : base( vSet.Keys )
        {
            // Copy the ValueItems into intial row
            Rows = null;

            Rows = new ValueRows[10];
            Rows[0].Value = new object[12];
            Rows[0].Value[0] = "1";

            // Intialise reader
            Reset();
        }

        public NameValueList(string[] keys) : base( keys )
        {
            // Intialise reader
            Reset();
        }

        public NameValueList(Int32 maxItems) : base( maxItems )
        {
            // Intialise reader
            Reset();
        }

        public NameValueList() : base()
        {
            // Intialise reader
            Reset();
        }
        #endregion

        #region IValueRows Members
        // 
        // Summary:
        // Function for object ??
        // IDictionary funtionality
        // Functions for value retrieval
        // ICollection functionality
        // Functions for Row management
        // Parameter: 
        //            fred
        // Returns: 
        //            freddy
        //

        public void Reset()
        {
        }

        public bool Read()
        {
            return (false);
        }

        public bool Write()
        {
            return (false);
        }

        public bool Insert()
        {
            return (false);
        }

        public bool Delete()
        {
            return (false);
        }

        public bool Append()
        {
            return (false);
        }
        #endregion

        #region Private Classes
        class ValueRows
        {
            public object[] Value;
        }
        #endregion
    }

}
