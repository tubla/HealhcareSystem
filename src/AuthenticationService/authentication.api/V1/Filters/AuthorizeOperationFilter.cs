using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace authentication.api.V1.Filters
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if the endpoint has the [Authorize] attribute
            var hasAuthorizeAttribute = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any() ||
                context.MethodInfo.DeclaringType!
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any();

            if (!hasAuthorizeAttribute)
                return;

            // Apply security requirement only to the CheckPermission endpoint
            bool? isCheckPermissionEndpoint = context?.ApiDescription?.RelativePath?
                .Contains("check-permission", StringComparison.OrdinalIgnoreCase);

            if (isCheckPermissionEndpoint != null)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [bearerScheme] = new List<string>()
                });
            }
        }
    }
}
