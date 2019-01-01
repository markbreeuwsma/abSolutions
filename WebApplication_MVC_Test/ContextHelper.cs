using DatabaseModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApplication_MVC_Test
{
    public static class ContextHelper
    {
        // Create a context which uses an InMemory representation, so needs to be filled before doing any actions on it.
        // No working on a relational database does allow to save data with relational violation contraints, a set 
        // DefaultValueSql on a property will have no effect and concurrency exceptions will never occur.
        public static DatabaseContext GetInMemoryContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<DatabaseContext>()
                                 .UseInMemoryDatabase(databaseName: name).Options;
            var context = new DatabaseContext(dbOptions);

            // DatabaseInitializer.Initialize(context);  // could be used to fill the context, but to loose for my taste

            return context;
        }

        // Create a context which uses an InMemory representation, but also be a relational database using SQLite to
        // also check for the relational constraints, set defaults and concurrency exceptions that the plain InMemory
        // context does not support or could detect. Makes for more realistic tests, but a bit slower.
        // NB: In regular programming it can also be usefull to work on a temp database and this could work there.
        // NB: Could fit the open/get/close in a new class with an IDispose interface and a using statement when called
        // but the current try/finaly calls also do the job just fine.

        private static SqliteConnection _connection;

        public static void OpenSQLiteInMemory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        public static DatabaseContext GetSQLiteInMemoryContext()
        {
            if (_connection == null)
            {
                throw new ArgumentNullException("Use OpenSQLiteConnection before calling GetSQLiteInMemoryContext");
            }

            var dbOptions = new DbContextOptionsBuilder<DatabaseContext>()
                                 .UseSqlite(_connection).Options;
            var context = new DatabaseContext(dbOptions);

            context.Database.EnsureCreated();

            // DatabaseInitializer.Initialize(context);  // could be used to fill the context, but to loose for my taste

            return context;
        }

        public static void CloseSQLiteInMemory()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}
