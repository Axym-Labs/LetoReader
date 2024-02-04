using Microsoft.AspNetCore.Mvc.ApplicationModels;

using Reader.Data.Storage;

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly string _routePrefix;

    public RoutePrefixConvention(string routePrefix)
    {
        _routePrefix = routePrefix;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            bool? condition = controller.ControllerType.Namespace?.StartsWith(Constants.ProjectDir + ".api.controllers");
            if (condition == true)
            {
                foreach (var selector in controller.Selectors)
                {
                    if (selector.AttributeRouteModel != null)
                    {
                        selector.AttributeRouteModel.Template = _routePrefix + "/" + selector.AttributeRouteModel.Template?.TrimStart('/');
                    }
                }
            }
        }
    }
}