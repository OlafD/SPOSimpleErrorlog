using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SPOSimpleErrorlog
{
    public class SimpleErrorlog
    {
        SimpleErrorlogStructure Structure = new SimpleErrorlogStructure();

        protected ClientContext _ctx;
        protected Guid _correlationId = Guid.Empty;
        protected List _errorlogList;

        /// <summary>
        /// Initialize the SimpleErrorlog class
        /// </summary>
        /// <param name="context">ClientContext object, initialized and ready to use</param>
        public SimpleErrorlog(ClientContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("The parameter cannot be null.", "context");
            }

            _ctx = context;

            GetErrorlogList();
        }

        /// <summary>
        /// Check, whether the necessary list is available in the web of the client context. When the list is
        /// not available, create the list and the default view.
        /// </summary>
        public void EnsureList()
        {
            bool listExists = _ctx.Web.ListExists(Structure.Listname);

            if (listExists == true)
            {
                // the list exist, check the schema and make necessary corrections
            }
            else
            {
                // create the list and the fields
                CreateList();
                CreateDefaultView();

                GetErrorlogList();
            }
        }

        /// <summary>
        /// Create a new correlation id.
        /// </summary>
        public void SetCorrelation()
        {
            Guid id = Guid.NewGuid();

            SetCorrelation(id);
        }

        /// <summary>
        /// Create a new correlation id.
        /// </summary>
        /// <param name="correlationId"></param>
        public void SetCorrelation(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        /// <summary>
        /// Write a new information item to the errorlog list.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="message">string with the message vale</param>
        public void WriteInformation(string process, string message)
        {
            WriteToErrorlog(process, message, "Information");
        }

        /// <summary>
        /// Write a new warning item to the errorlog list.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="message">string with the message vale</param>
        public void WriteWarning(string process, string message)
        {
            WriteToErrorlog(process, message, "Warning");
        }

        /// <summary>
        /// Write a new warning item to the errorlog list.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="exception">Exception object, from where the Message, the InnerException and the StackTrace are taken</param>
        public void WriteWarning(string process, Exception exception)
        {
            var message = String.Format("{0}\r\nInnerException:{1}\r\nStacktrace: {2}", exception.Message, exception.InnerException, exception.StackTrace);

            WriteToErrorlog(process, message, "Warning");
        }

        /// <summary>
        /// Write a new error item to the errorlog list.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="message">string with the message vale</param>
        public void WriteError(string process, string message)
        {
            WriteToErrorlog(process, message, "Error");
        }

        /// <summary>
        /// Write a new error item to the errorlog list.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="exception">Exception object, from where the Message, the InnerException and the StackTrace are taken</param>
        public void WriteError(string process, Exception exception)
        {
            var message = String.Format("{0}\r\nInnerException:{1}\r\nStacktrace: {2}", exception.Message, exception.InnerException, exception.StackTrace);

            WriteToErrorlog(process, message, "Error");
        }

        /// <summary>
        /// Set the member _errorlogList to the List object of the errorlog list.
        /// </summary>
        protected void GetErrorlogList()
        {
            if (_ctx.Web.ListExists(Structure.Listname) == true)
            {
                _errorlogList = _ctx.Web.Lists.GetByTitle(Structure.Listname);
                _ctx.Load(_errorlogList);
                _ctx.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Write an item to the errorlog list in the web of the current client context.
        /// </summary>
        /// <param name="process">string with the process value</param>
        /// <param name="message">string with the message vale</param>
        /// <param name="type">string with the type, should be Information, Waring or Error</param>
        protected void WriteToErrorlog(string process, string message, string type)
        {
            ListItemCreationInformation newItem = new ListItemCreationInformation();
            ListItem item = _errorlogList.AddItem(newItem);

            item["Title"] = process;
            item["Type"] = type;
            item["Message"] = message;
            item["Timestamp"] = DateTime.Now;
            item["CorrelationId"] = _correlationId;

            item.Update();

            _ctx.ExecuteQueryRetry();
        }

        /// <summary>
        /// Create the errorlog list, based on the structurure definded.
        /// </summary>
        protected void CreateList()
        {
            ListCreationInformation newList = new ListCreationInformation();
            newList.Title = Structure.Listname;
            newList.TemplateType = (int)ListTemplateType.GenericList;
            newList.QuickLaunchOption = QuickLaunchOptions.Off;
            newList.Url = String.Format("lists/{0}", Structure.Listname);

            _ctx.Web.Lists.Add(newList);
            _ctx.Web.Update();
            _ctx.ExecuteQueryRetry();

            List list = _ctx.Web.Lists.GetByTitle(Structure.Listname);
            _ctx.Load(list);
            _ctx.ExecuteQueryRetry();

            IEnumerator e = Structure.FieldNames.GetEnumerator();

            while (e.MoveNext() == true)
            {
                string fieldName = e.Current as string;
                string fieldXml = Structure.GetFieldXml(fieldName);

                if (fieldXml != String.Empty)
                {
                    list.Fields.AddFieldAsXml(fieldXml, false, AddFieldOptions.DefaultValue);
                }
            }

            list.Update();
            _ctx.ExecuteQueryRetry();

            GetErrorlogList();
        }

        /// <summary>
        /// Create the default view in the errorlog list.
        /// </summary>
        protected void CreateDefaultView()
        {
            View view = _errorlogList.DefaultView;
            _ctx.Load(view);
            _ctx.ExecuteQueryRetry();

            view.ViewFields.RemoveAll();
            view.ViewFields.Add("Timestamp");
            view.ViewFields.Add("LinkTitle");
            view.ViewFields.Add("Message");
            view.ViewFields.Add("Type");
            view.ViewFields.Add("CorrelationId");

            view.ViewQuery = "<OrderBy><FieldRef Name='ID' Ascending='True'/></OrderBy>";

            view.RowLimit = 100;
            view.Paged = true;

            view.Update();

            _ctx.ExecuteQueryRetry();
        }
    }
}
