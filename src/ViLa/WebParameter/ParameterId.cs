using WebExpress.WebMessage;

namespace ViLa.WebParameter
{
    internal class ParameterId : Parameter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterId()
         : this(null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The value.</param>
        public ParameterId(string value)
            : base("Id", value, ParameterScope.Url)
        {

        }
    }
}
