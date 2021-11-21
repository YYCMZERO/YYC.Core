using AntJoin.Core.Extensions;

namespace AntJoin.Core.Exceptions
{
    public class ApiException : OpenBaseException
    {
        public ApiException()
        {
        }

        public ApiException(ApiErrorCode code)
        {
            ErrorCode = (int)code;
            switch (code)
            {
                case ApiErrorCode.LACK_ID:
                    ErrorMsg = "参数ID不能为空！";
                    break;
                case ApiErrorCode.INVALID_PAGE:
                    ErrorMsg = "参数page必须为正整数！";
                    break;
                case ApiErrorCode.INVALID_PAGE_SIZE:
                    ErrorMsg = "参数pageSize必须在1~100范围内！";
                    //case ApiErrorCode
                    break;
                case ApiErrorCode.INVALID_PAGE_SIZE_BIG:
                    ErrorMsg = "参数pageSize必须在1~10000范围内！";
                    break;
                case ApiErrorCode.LACK_START_DATE:
                    ErrorMsg = "参数startDate不能为空！";
                    break;
                case ApiErrorCode.LACK_END_DATE:
                    ErrorMsg = "参数endDate不能为空！";
                    break;
                case ApiErrorCode.USER_NON_EXISTENT:
                    ErrorMsg = "用户不存在！";
                    break;
                case ApiErrorCode.REQUEST_API_ERROR:
                    ErrorMsg = "第三方接口请求失败！";
                    break;
                case ApiErrorCode.DATA_EXECUTE_ERROR:
                    ErrorMsg = "数据库数据操作失败！";
                    break;
                case ApiErrorCode.DATA_NON_EXISTENT:
                    ErrorMsg = "数据不存在！";
                    break;
                case ApiErrorCode.DEVELOPING:
                    ErrorMsg = "功能开发中！";
                    break;
                default:
                    ErrorCode = -1;
                    ErrorMsg = "未知错误！";
                    break;
            }
        }

        public ApiException(ApiErrorCode code, string msg)
        {
            ErrorCode = (int)code;
            ErrorMsg = msg;
        }


        public ApiException(UserErrorCode code, string msg = null)
        {
            ErrorCode = (int)code;
            ErrorMsg = msg ?? code.ToEnumDescription();
        }
    }
}
