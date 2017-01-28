using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CTSignatureGenerator.Controllers
{
    public class SvgResult : ActionResult
    {
        private XDocument objectToSerialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public SvgResult(XDocument objectToSerialize) {
            this.objectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize {
            get { return this.objectToSerialize; }
        }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ActionContext context) {
            if (this.objectToSerialize != null) {
                context.HttpContext.Response.ContentType = "image/svg+xml";
                objectToSerialize.Save(context.HttpContext.Response.Body);
            }
        }
    }
}