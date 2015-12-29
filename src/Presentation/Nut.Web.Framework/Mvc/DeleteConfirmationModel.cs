namespace Nut.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseNutEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}