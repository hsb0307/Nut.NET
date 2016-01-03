using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    public static class DepartmentExtensions {
        public static string GetFormattedBreadCrumb(this Department department,
            IDepartmentService departmentService,
            string separator = ">>") {
            if (department == null)
                throw new ArgumentNullException("category");

            string result = string.Empty;

            //used to prevent circular references
            var alreadyProcessedCategoryIds = new List<int>() { };

            while (department != null &&  //not null
                !alreadyProcessedCategoryIds.Contains(department.Id)) //prevent circular references
            {
                if (String.IsNullOrEmpty(result)) {
                    result = department.Name;
                } else {
                    result = string.Format("{0} {1} {2}", department.Name, separator, result);
                }

                alreadyProcessedCategoryIds.Add(department.Id);

                department = departmentService.GetById(department.ParentId);

            }
            return result;
        }
    }
}
