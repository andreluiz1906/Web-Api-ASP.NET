using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ProdutoCatalogo.Application.Conventions;

public class LowercaseControllerModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        controller.ControllerName = controller.ControllerName.ToLower();
        foreach (var selector in controller.Selectors)
        {
            selector.AttributeRouteModel.Template = selector.AttributeRouteModel.Template.ToLower();
        }
    }
}