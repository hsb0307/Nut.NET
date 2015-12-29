using System.Web.Mvc;

namespace Nut.Web.Framework.Mvc
{
    public class NutModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseNutModel)
            {
                ((BaseNutModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }
    }
}
