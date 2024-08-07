using FluentResults;

namespace Demo.ExtensionMethod
{
    public static class ResultExtention
    {
        public static IResult Match<T>(
           this Result<T> result,
           Func<T, IResult> onSuccess,
           Func<List<IReason>, IResult> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Reasons);
        }
    }
}
