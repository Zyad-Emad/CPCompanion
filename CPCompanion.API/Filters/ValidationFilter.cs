using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CPCompanion.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments)
            {
                var value = argument.Value;
                if (value is null)
                    continue;
                var argType = value.GetType();

                var validatorType = typeof(IValidator<>).MakeGenericType(argType);

                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if(validator is null) 
                    continue;

                var validationContext = new ValidationContext<object>(value);

                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {

                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }

            }

            await next();
        }
    }
}
