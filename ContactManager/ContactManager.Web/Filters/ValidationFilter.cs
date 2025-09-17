using FluentValidation;

namespace ContactManager.Web.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator != null)
        {
            var model = context.Arguments.OfType<T>().FirstOrDefault();
            if (model != null)
            {
                var result = await validator.ValidateAsync(model);
                if (!result.IsValid)
                {
                    return Results.ValidationProblem(result.ToDictionary());
                }
            }
        }

        return await next(context);
    }
}