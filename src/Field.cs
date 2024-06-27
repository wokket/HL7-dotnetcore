﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7.Dotnetcore
{
    public class Field : MessageElement
    {
        private List<Field> _RepetitionList;

        internal ComponentCollection ComponentList { get; set; }

        public bool IsComponentized { get; set; } = false;
        public bool HasRepetitions { get; set; } = false;
        public bool IsDelimitersField { get; set; } = false;

        internal List<Field> RepetitionList
        {
            get
            {
                if (_RepetitionList == null)
                    _RepetitionList = new List<Field>();

                return _RepetitionList;
            }
            set
            {
                _RepetitionList = value;
            }
        }

        protected override void ProcessValue()
        {
            if (this.IsDelimitersField)  // Special case for the delimiters fields (MSH)
            {
                var subcomponent = new SubComponent(_value, this.Encoding);
#if NET8_0_OR_GREATER
                this.ComponentList.Clear();
#else
                this.ComponentList = new ComponentCollection(1);
#endif

                Component component = new Component(this.Encoding, true);

                component.SubComponentList.Add(subcomponent);

                this.ComponentList.Add(component);
                return;
            }

            this.HasRepetitions = _value.Contains(this.Encoding.RepeatDelimiter);

            if (this.HasRepetitions)
            {
                var individualFields = MessageHelper.SplitString(_value, this.Encoding.RepeatDelimiter);
                _RepetitionList = new List<Field>(individualFields.Length);

                foreach (var individualField in individualFields)
                {
                    Field field = new Field(individualField, Encoding);
                    _RepetitionList.Add(field);
                }
            }
            else
            {
                var allComponents = MessageHelper.SplitString(_value, this.Encoding.ComponentDelimiter);

                ComponentList = new ComponentCollection(allComponents.Length);

                foreach (string strComponent in allComponents)
                {
                    Component component = new Component(this.Encoding)
                    {
                        Value = strComponent
                    };
                    this.ComponentList.Add(component);
                }

                this.IsComponentized = this.ComponentList.Count > 1;
            }
        }

        public Field(HL7Encoding encoding)
        {
            this.ComponentList = new ComponentCollection();
            this.Encoding = encoding;
        }

        public Field(string value, HL7Encoding encoding)
        {
            this.ComponentList = new ComponentCollection();
            this.Encoding = encoding;
            this.Value = value;
        }

        public bool AddNewComponent(Component com)
        {
            try
            {
                this.ComponentList.Add(com);
                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add new component Error - " + ex.Message, ex);
            }
        }

        public bool AddNewComponent(Component component, int position)
        {
            try
            {
                this.ComponentList.Add(component, position);
                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add new component Error - " + ex.Message, ex);
            }
        }

        public Component Components(int position)
        {
            position = position - 1;

            try
            {
                return ComponentList[position];
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Component not available Error - " + ex.Message, ex);
            }
        }

        public List<Component> Components()
        {
            return ComponentList;
        }

        public List<Field> Repetitions()
        {
            if (this.HasRepetitions)
            {
                return RepetitionList;
            }
            return null;
        }

        public Field Repetitions(int repetitionNumber)
        {
            if (this.HasRepetitions)
            {
                return RepetitionList[repetitionNumber - 1];
            }
            return null;
        }

        public bool RemoveEmptyTrailingComponents()
        {
            try
            {
                for (var eachComponent = ComponentList.Count - 1; eachComponent >= 0; eachComponent--)
                {
                    if (ComponentList[eachComponent].Value == "")
                        ComponentList.Remove(ComponentList[eachComponent]);
                    else
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Error removing trailing components - " + ex.Message, ex);
            }
        }
        public void AddRepeatingField(Field field)
        {
            if (!this.HasRepetitions)
            {
                throw new HL7Exception("Repeating field must have repetitions (HasRepetitions = true)");
            }
            if (_RepetitionList == null)
            {
                _RepetitionList = new List<Field>();
            }
            _RepetitionList.Add(field);
        }
    }
}
