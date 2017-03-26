using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SPOSimpleErrorlog
{
    class SimpleErrorlogStructure
    {
        protected const string SP_ERRORLISTNAME = "Errorlog";

        protected Hashtable ErrorlogFields;

        public struct ErrorlogField
        {
            public string InternalName;
            public string DisplayName;
            public string Type;
            public bool ShowInNewForm;
            public bool ShowInEditForm;
            public bool ShowInDisplayForm;
            public string FieldAsXml;
        }

        /// <summary>
        /// The predefined listname of the errorlog list.
        /// </summary>
        public string Listname
        {
            get
            {
                return SP_ERRORLISTNAME;
            }
        }

        /// <summary>
        /// The predefined names of the fields in the errorlog list.
        /// </summary>
        public ICollection FieldNames
        {
            get
            {
                return ErrorlogFields.Keys;
            }
        }

        /// <summary>
        /// Initialize the SimpleErrorlogStructure class
        /// </summary>
        public SimpleErrorlogStructure()
        {
            ErrorlogFields = new Hashtable()
            {
                { "Title", new ErrorlogField() { InternalName = "Title",
                                                DisplayName = "Process",
                                                Type = "Text",
                                                ShowInNewForm = false,
                                                ShowInEditForm = false,
                                                ShowInDisplayForm = true,
                                                FieldAsXml = "" } },
                { "Timestamp", new ErrorlogField() { InternalName = "Timestamp",
                                                DisplayName = "Timestamp",
                                                Type = "DatetimeFull",
                                                ShowInNewForm = false,
                                                ShowInEditForm = false,
                                                ShowInDisplayForm = true,
                                                FieldAsXml = "<Field ID='{070c54ef-9d0a-4309-af3b-aa13923ce240}' Name='Timestamp' DisplayName='Timestamp' StaticName='Timestamp' Type='DateTime' Format='DateTime' Required='FALSE' ShowInNewForm='FALSE' ShowInEditForm='FALSE' ShowInDisplayForm='TRUE'/>" } },
                { "Type", new ErrorlogField() { InternalName = "Type",
                                                DisplayName = "Type",
                                                Type = "Text",
                                                ShowInNewForm = false,
                                                ShowInEditForm = false,
                                                ShowInDisplayForm = true,
                                                FieldAsXml = "<Field ID='{07dfddc3-0fd5-49e7-bb8c-82db03a47069}' Name='Type' DisplayName='Type' StaticName='Type' Type='Text' Required='FALSE' ShowInNewForm='FALSE' ShowInEditForm='FALSE' ShowInDisplayForm='TRUE' />" } },
                { "Message", new ErrorlogField() { InternalName = "Message",
                                                DisplayName = "Message",
                                                Type = "MultiText",
                                                ShowInNewForm = false,
                                                ShowInEditForm = false,
                                                ShowInDisplayForm = true,
                                                FieldAsXml = "<Field ID='{f2f6ae2d-d28d-44d4-83c1-530ede63700a}' Name='Message' DisplayName='Message' StaticName='Message' Type='Note' Required='FALSE' ShowInNewForm='FALSE' ShowInEditForm='FALSE' ShowInDisplayForm='TRUE' />" } },
                { "CorrelationId", new ErrorlogField() { InternalName = "CorrelationId",
                                                DisplayName = "CorrelationId",
                                                Type = "Text",
                                                ShowInNewForm = false,
                                                ShowInEditForm = false,
                                                ShowInDisplayForm = true,
                                                FieldAsXml = "<Field ID='{3f5f6c46-e59d-4f81-a7bb-68eb95879e7b}' Name='CorrelationId' DisplayName='CorrelationId' StaticName='CorrelationId' Type='Text' Required='FALSE' ShowInNewForm='FALSE' ShowInEditForm='FALSE' ShowInDisplayForm='TRUE' />" } }
            };
        }

        /// <summary>
        /// Get the predefined field by its internal namem.
        /// </summary>
        /// <param name="InternalName">string with the internal name of the field</param>
        /// <returns>the ErrorlogField struct for the field</returns>
        public ErrorlogField GetField(string InternalName)
        {
            if (ErrorlogFields.ContainsKey(InternalName) == true)
            {
                return (ErrorlogField)ErrorlogFields[InternalName];
            }
            else
            {
                return new ErrorlogField();
            }
        }

        /// <summary>
        /// Get the predefined FieldXml for a field by its internal name.
        /// </summary>
        /// <param name="InternalName">string with the internal name of the field</param>
        /// <returns>the string with the FieldXml</returns>
        public string GetFieldXml(string InternalName)
        {
            string result = String.Empty;

            if (ErrorlogFields.ContainsKey(InternalName) == true)
            {
                result = ((ErrorlogField)ErrorlogFields[InternalName]).FieldAsXml;
            }

            return result;
        }
    }
}
