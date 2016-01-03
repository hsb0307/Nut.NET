using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    public class DepartmentService : IDepartmentService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string DEPARTMENT_ALL_KEY = "nut.department.all-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string DEPARTMENT_PATTERN_KEY = "nut.department.";

        #endregion

        #region Fields

        private readonly IRepository<Department> _departmentRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="departmentRepository">Department repository</param>
        /// <param name="eventPublisher">Event published</param>
        public DepartmentService(ICacheManager cacheManager,
            IRepository<Department> departmentRepository,
            ISignals signals) {
            _cacheManager = cacheManager;
            _departmentRepository = departmentRepository;
            _signals = signals;
        }

        #endregion

        #region Methods



        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Department collection</returns>
        public virtual IList<Department> GetAll(bool showHidden = false) {
            string key = string.Format(DEPARTMENT_ALL_KEY, showHidden);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(DEPARTMENT_PATTERN_KEY));
                var query = from c in _departmentRepository.Table
                                //orderby c.DisplayOrder, c.Name
                                //where showHidden || c.Published
                            select c;
                if (!showHidden)
                    query = query.Where(c => !c.Deleted).OrderBy(c => c.DisplayOrder);
                var departments = query.ToList();
                return departments;
            });
        }


        public virtual IPagedList<Department> GetPaged(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false) {
            var query = _departmentRepository.Table;
            if (!showHidden)
                query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplayOrder).ThenBy(v => v.Name);

            var departments = new PagedList<Department>(query, pageIndex, pageSize);
            return departments;
        }

        /// <summary>
        /// Gets a department 
        /// </summary>
        /// <param name="departmentId">Department identifier</param>
        /// <returns>Department</returns>
        public virtual Department GetById(int departmentId) {
            if (departmentId == 0)
                return null;

            return _departmentRepository.GetById(departmentId);
        }


        public virtual Department GetByName(string name) {
            //var query = _vendorRepository.Table;
            if (String.IsNullOrWhiteSpace(name))
                return null;
            var query = from c in _departmentRepository.Table
                        where c.Name == name
                        select c;
            var department = query.FirstOrDefault();

            return department;
        }

        /// <summary>
        /// Inserts a department
        /// </summary>
        /// <param name="department">Department</param>
        public virtual void Insert(Department department) {
            if (department == null)
                throw new ArgumentNullException("department");

            _departmentRepository.Insert(department);

            _signals.Trigger(DEPARTMENT_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the department
        /// </summary>
        /// <param name="department">Department</param>
        public virtual void Update(Department department) {
            if (department == null)
                throw new ArgumentNullException("department");

            _departmentRepository.Update(department);

            _signals.Trigger(DEPARTMENT_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes a department
        /// </summary>
        /// <param name="department">Department</param>
        public virtual void Delete(Department department) {
            if (department == null)
                throw new ArgumentNullException("department");

            _departmentRepository.Delete(department);

            _signals.Trigger(DEPARTMENT_PATTERN_KEY);

        }

        #endregion
    }
}
