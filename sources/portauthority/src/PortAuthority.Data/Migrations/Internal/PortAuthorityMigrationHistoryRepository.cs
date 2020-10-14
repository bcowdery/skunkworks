using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace PortAuthority.Data.Migrations.Internal
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public class PortAuthorityMigrationHistoryRepository : SqlServerHistoryRepository
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        protected override string TableSchema { get; } = "pa";

        public PortAuthorityMigrationHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}
