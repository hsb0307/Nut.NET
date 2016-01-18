using MySql.Data.MySqlClient;
using MySql.Data;
using Nut.Core.Data;
using Nut.Data.Initializers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Data.Common;

namespace Nut.Data {
    public class MySqlDataProvider : IDataProvider {

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        public virtual void InitConnectionFactory() {
            var connectionFactory = new MySqlConnectionFactory();
#pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitDatabase() {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        /// <summary>
        /// Set database initializer
        /// </summary>
        public virtual void SetDatabaseInitializer() {
            //pass some table names to ensure that we have nopCommerce 2.X installed
            var tablesToValidate = new[] { "UserRole", "PermissionRecord", "Language", "LocaleStringResource" };

            //custom commands (stored proedures, indexes)

            var customCommands = new List<string>();
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/Install/MySql.Indexes.sql"), false));
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/Install/MySql.StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<NutObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists) {
            if (!File.Exists(filePath)) {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream)) {
                var statement = "";
                while ((statement = readNextStatementFromStream(reader)) != null) {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader) {
            var sb = new StringBuilder();

            string lineOfText;

            while (true) {
                lineOfText = reader.ReadLine();
                if (lineOfText == null) {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }

                //MySql doesn't support GO, so just use a commented out GO as the separator
                if (lineOfText.TrimEnd().ToUpper() == "-- GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public virtual bool StoredProceduredSupported {
            get { return true; }
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter() {
            return new MySqlParameter();
        }
    }
}
