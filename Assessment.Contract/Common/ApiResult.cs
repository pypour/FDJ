using Assessment.Contract.Enums;

namespace Assessment.Contract.Common
{
    //We need to have a common structure for all APIs
    public class ApiResult<T>
    {
        public bool Success { get; init; }
        public ExceptionCodeEnum Error { get; init; }
        public T Data { get; init; }

        public ApiResult()
        {
            
        }

        public ApiResult(ExceptionCodeEnum exceptionCode)
        {
            Success = false;
            Error = exceptionCode;
        }
    }
}
