using System;
using System.Collections.Generic;

namespace HL7.Dotnetcore
{
    public class Component : MessageElement
    {
        internal List<SubComponent> SubComponentList { get; set; }

        public bool IsSubComponentized { get; set; } = false;

        private bool isDelimiter = false;

        public Component(HL7Encoding encoding, bool isDelimiter = false)
        {
            this.isDelimiter = isDelimiter;
            this.SubComponentList = new List<SubComponent>();
            this.Encoding = encoding;
        }
        public Component(string pValue, HL7Encoding encoding)
        {
            this.SubComponentList = new List<SubComponent>();
            this.Encoding = encoding;
            this.Value = pValue;
        }

        protected override void ProcessValue()
        {
            List<string> allSubComponents;
            
            if (this.isDelimiter)
#if NET8_0_OR_GREATER
                allSubComponents = new List<string>([Value]);
#else
                allSubComponents = new List<string>(new [] {this.Value});
#endif

            else
                allSubComponents = MessageHelper.SplitString(_value, this.Encoding.SubComponentDelimiter);

            if (allSubComponents.Count > 1)
                this.IsSubComponentized = true;

#if NET8_0_OR_GREATER
            SubComponentList.Clear(); // in case there's existing data in there
#else
            SubComponentList = new List<SubComponent>(allSubComponents.Count);
#endif

            foreach (string strSubComponent in allSubComponents)
            {
                SubComponent subComponent = new SubComponent(strSubComponent, this.Encoding);
                SubComponentList.Add(subComponent);
            }
        }

        public SubComponent SubComponents(int position)
        {
            position = position - 1;

            try
            {
                return SubComponentList[position];
            }
            catch (Exception ex)
            {
                throw new HL7Exception("SubComponent not available Error-" + ex.Message, ex);
            }
        }

        public List<SubComponent> SubComponents()
        {
            return SubComponentList;
        }
    }
}
