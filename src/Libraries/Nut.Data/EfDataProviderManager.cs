using System;
using Nut.Core;
using Nut.Core.Data;

namespace Nut.Data
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {

            var providerName = Settings.DataProvider;
            if (String.IsNullOrWhiteSpace(providerName))
                throw new NutException("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider();
                case "sqlce":
                    return new SqlCeDataProvider();
                case "mysql":
                    return new MySqlDataProvider();
                default:
                    throw new NutException(string.Format("Not supported dataprovider name: {0}", providerName));
            }
        }

    }
}
