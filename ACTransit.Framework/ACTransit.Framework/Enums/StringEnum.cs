namespace ACTransit.Framework.Enums
{
    public abstract class StringEnum
    {
        private readonly string _name;
        private readonly object _value;

        public string Value
        {
            get
            {
                return _value as string;
            }            
        }

        protected StringEnum(string name):this(name,name){}
        protected StringEnum(string name, object value)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }

        // For More information about Conversion Operators, take alook at the links below.
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/conversion-operators
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/using-conversion-operators
        public static implicit operator string(StringEnum roleName)
        {
            return roleName.ToString();
        }
    }
}
