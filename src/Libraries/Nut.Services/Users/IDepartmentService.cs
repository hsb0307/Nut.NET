using Nut.Core;
using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    public interface IDepartmentService {
        /// <summary>
        /// Gets all Departments
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Department collection</returns>
        IList<Department> GetAll(bool showHidden = false);


        IPagedList<Department> GetPaged(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a department 
        /// </summary>
        /// <param name="countryId">Department identifier</param>
        /// <returns>Department</returns>
        Department GetById(int id);

        /// <summary>
        /// Gets a department by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Department two letter ISO code</param>
        /// <returns>Department</returns>
        Department GetByName(string name);

        /// <summary>
        /// Inserts a department
        /// </summary>
        /// <param name="department">Department</param>
        void Insert(Department department);

        /// <summary>
        /// Updates the department
        /// </summary>
        /// <param name="department">Department</param>
        void Update(Department department);
        /// <summary>
        /// Deletes a department
        /// </summary>
        /// <param name="department">Department</param>
        void Delete(Department department);
    }
}
