using System;
using System.Linq;
using System.Web.Mvc;
using Nut.Core;
using Nut.Admin.Models.Users;
using Nut.Core.Domain.Users;
using Nut.Services.Users;
using Nut.Services.Localization;
using Nut.Services.Stores;
using Nut.Services.Security;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Kendoui;
using Nut.Admin.Extensions;

namespace Nut.Admin.Controllers
{
    public class DepartmentController : BaseAdminController
    {
        #region Fields

        private readonly IDepartmentService _departmentService;
        private readonly IStoreService _storeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public DepartmentController(IDepartmentService departmentService,
            IStoreService storeService, 
            ILocalizationService localizationService, 
            IPermissionService permissionService) {
            this._departmentService = departmentService;
            this._storeService = storeService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareDepartmentModel(DepartmentModel model, Department department) {
            if (model == null)
                throw new ArgumentNullException("model");
            var departments = _departmentService.GetAll();

            model.AvailableDepartments.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Department.NotParentDepartment"), Value = "0" });

            foreach (var d in departments)
                model.AvailableDepartments.Add(new SelectListItem {
                    Text = d.Name,
                    Value = d.Id.ToString(),
                    Selected = department == null ? false : d.Id == department.ParentId
                });
            

            var stores = _storeService.GetAllStores();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.Select"), Value = "0" });
            foreach (var d in stores) {
                model.AvailableStores.Add(new SelectListItem {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                    Selected = department != null ? model.StoreId == d.Id : false
                });
            }
        }
        #endregion


        #region Methods

        //list
        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var departments = _departmentService
                .GetPaged(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult {
                Data = departments.Select(x => {
                    var m = x.ToModel();
                    m.Breadcrumb = x.GetFormattedBreadCrumb(_departmentService);
                    return m;
                }),
                Total = departments.TotalCount
            };

            return Json(gridModel);
        }

        //create
        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new DepartmentModel();
            model.DisplayOrder = 0;
            model.Deleted = false;
            PrepareDepartmentModel(model, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(DepartmentModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (ModelState.IsValid) {
                var department = model.ToEntity();

                _departmentService.Insert(department);

                //activity log
                //_customerActivityService.InsertActivity("AddNewDepartment", _localizationService.GetResource("ActivityLog.AddNewDepartment"), department.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.Departments.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = department.Id }) : RedirectToAction("List");
            }

            PrepareDepartmentModel(model, null);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var department = _departmentService.GetById(id);
            if (department == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = department.ToModel();
            PrepareDepartmentModel(model, department);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(DepartmentModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var department = _departmentService.GetById(model.Id);
            if (department == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid) {
                department = model.ToEntity(department);

                _departmentService.Update(department);

                //activity log
                //_customerActivityService.InsertActivity("EditDepartment", _localizationService.GetResource("ActivityLog.EditDepartment"), department.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Departments.Updated"));
                return continueEditing ? RedirectToAction("Edit", department.Id) : RedirectToAction("List");
            }
            PrepareDepartmentModel(model, department);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var department = _departmentService.GetById(id);
            if (department == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            if (department.Id.Equals(1))
                return Json(new { success = false, message = _localizationService.GetResource("Admin.Departments.DefaultNotDeleted") });

            _departmentService.Delete(department);

            //activity log
            //_customerActivityService.InsertActivity("DeleteDepartment", _localizationService.GetResource("ActivityLog.DeleteDepartment"), department.Name);

            return Json(new { success = true, message = _localizationService.GetResource("Admin.Departments.Deleted") });
        }

        #endregion
    }
}