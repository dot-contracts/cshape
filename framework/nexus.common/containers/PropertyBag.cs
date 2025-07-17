using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace nexus.common
{
    public interface IPropertyKey
    {
        string  Name { get; }
    }

    public class NameValue : IPropertyKey
    {
        private string      name;
        private string      valu;

        public  string      Name  { get { return  name ; } }
        public  string      Value { get { return  valu ; } set { valu = value; } }

        public  NameValue ( string _name, string _value )
        {
            name  = _name;
            valu  = _value;
        }
    }

    public class NameVtyp  : IPropertyKey
    {
        private string      name;
        private string      valu;

        public  string      Name  { get { return  name ; } }
        public  string      Value { get { return  valu ; } set { valu = value; } }

        public  NameVtyp ( string _name, string _value)
        {
            name = _name;
            valu = _value;
        }
    }

    public class NameCtyp  : IPropertyKey
    {
        private string   name;
        private string   valu;

        public  string   Name  { get { return  name ; } }
        public  string   Value { get { return  valu ; } set { valu = value; } }

        public  NameCtyp ( string _name, string _value )
        {
            name = _name;
            valu = _value;
        }
    }

    public class NameObject : IPropertyKey
    {
        private string      name;
        private object      valu;

        public  string      Name  { get { return  name ; } }
        public  object      Value { get { return  valu ; } set { valu = value; } }

        public  NameObject ( string _name, object _value )
        {
            name = _name;
            valu = _value;
        }
    }

    public class PropertyBag<T> : Dictionary<string, T> where T : IPropertyKey
    {
        public void Add( T obj )
        {
            if (!ContainsKey(obj.Name))
                 Add ( obj.Name, obj );
        }

        public bool Remove( T obj )
        {
            return  Remove ( obj.Name ) ;
        }

        public bool Contains( T obj )
        {
            return  ContainsKey( obj.Name ) ;
        }

        public T GetValue( string key )
        {
            T tmp;

            if ( TryGetValue( key, out tmp ) ) return  tmp ;

            return  default(T) ;
        }

        public bool IsGreaterThanZero(string key)
        {
            int Int;
            int.TryParse(GetValue(key).ToString(), out Int);
            return Int > 0;
        }
        public bool IsNumeric(string key)
        {
            return Information.IsNumeric(GetValue(key));
        }
        public bool IsDate(string key)
        {
            return Information.IsDate(GetValue(key));
        }



        public T this[string key]   { get { return  base[key] ; } set { base[key] = value; } }

    }
}
