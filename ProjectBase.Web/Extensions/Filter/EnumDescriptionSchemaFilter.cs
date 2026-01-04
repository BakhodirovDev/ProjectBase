using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectBase.WebApi.Extensions;

public class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var enumDescriptions = Enum.GetValues(context.Type)
                .Cast<Enum>()
                .Select(e => $"{e}")
                .ToList();

            schema.Description = string.Join("<br>", enumDescriptions);

            schema.Enum.Clear();
            foreach (var description in enumDescriptions)
            {
                schema.Enum.Add(new OpenApiString(description));
            }
        }
    }
}
