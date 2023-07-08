using ChatApp.Services;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Extension;

public static class ApiResponseExtension
{
    public static IActionResult ToOkResponse<TResult>(this Result<TResult> result)
    {
        return result.Match<IActionResult>(ok =>
        {
            return new OkResult();
        }, failure =>
        {
            if (failure is ApiException exception)
            {
                return new BadRequestObjectResult(new { error = new string[] { exception.Message } });
            }

            return new BadRequestResult();
        });
    }
}
