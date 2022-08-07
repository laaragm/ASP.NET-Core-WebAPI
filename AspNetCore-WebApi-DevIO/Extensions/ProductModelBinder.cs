using AspNetCore_WebApi_DevIO.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Extensions
{
    // Custom Binder to send IFormFile and ViewModel inside a FormData
    public class ProductModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };

            var productViewModel = JsonSerializer.Deserialize<ProductViewModel>(bindingContext.ValueProvider.GetValue("product").FirstOrDefault(), serializeOptions);
            productViewModel.ImageUpload = bindingContext.ActionContext.HttpContext.Request.Form.Files.FirstOrDefault();

            bindingContext.Result = ModelBindingResult.Success(productViewModel);
            return Task.CompletedTask;
        }
    }
}
