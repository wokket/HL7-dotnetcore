namespace HL7.Dotnetcore
{
    public abstract class MessageElement
    {
        protected string _value = string.Empty;
       
        
        public  string Value 
        { 
            get 
            {
                return _value == Encoding.PresentButNull ? null : Encoding.Decode(_value); 
            }
            set 
            { 
                _value = value; 
                ProcessValue(); 
            }
        }

        public  string UndecodedValue 
        { 
            get 
            {
                return _value == Encoding.PresentButNull ? null : _value; 
            }
        }

        public HL7Encoding Encoding { get; protected set; }

        protected abstract void ProcessValue();
    }
}
