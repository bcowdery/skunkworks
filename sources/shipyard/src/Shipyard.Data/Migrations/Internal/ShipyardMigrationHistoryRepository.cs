using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Shipyard.Data.Migrations.Internal
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    public class ShipyardMigrationHistoryRepository : SqlServerHistoryRepository
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
        protected override string TableSchema { get; } = "Yrd";

        public ShipyardMigrationHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}
