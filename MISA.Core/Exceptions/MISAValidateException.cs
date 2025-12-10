using System.Collections;

namespace MISA.Core.Exceptions
{
    /// <summary>
    /// Xử lý custom exception cho validate dữ liệu
    /// </summary>
    /// Created by: LQThinh (03/12/2025)
    public class MISAValidateException : Exception
    {
        IDictionary _errors = new Dictionary<string, string>();
        private string _message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin!";
        public MISAValidateException(IDictionary errors)
        {
            _errors = errors;
        }
        public override string Message => _message;
        public IDictionary Errors => _errors;
    }
}
