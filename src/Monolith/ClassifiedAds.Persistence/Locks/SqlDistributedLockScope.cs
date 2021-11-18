using ClassifiedAds.CrossCuttingConcerns.Locks;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassifiedAds.Persistence.Locks
{
    public class SqlDistributedLockScope : IDistributedLockScope
    {
        private readonly SqlConnection _connection;
        private readonly string _lockName;

        public SqlDistributedLockScope(SqlConnection connection, string lockName)
        {
            _connection = connection;
            _lockName = lockName;
        }

        public void Dispose()
        {
            SqlParameter returnValue;
            var releaseCommand = CreateReleaseCommand(_connection, _lockName, false, out returnValue);
            releaseCommand.ExecuteNonQuery();

            if (ParseReturnCode((int)returnValue.Value))
            {
            }
        }

        public bool StillHoldingLock()
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"SELECT APPLOCK_MODE('public', @Resource, @LockOwner)";
            command.Parameters.Add(new SqlParameter("Resource", _lockName));
            command.Parameters.Add(new SqlParameter("LockOwner", "Session"));
            var lockMode = (string)command.ExecuteScalar();

            return lockMode != "NoLock";
        }

        private static SqlCommand CreateReleaseCommand(SqlConnection connection, string lockName, bool isTry, out SqlParameter returnValue)
        {
            var command = connection.CreateCommand();
            if (isTry)
            {
                command.CommandText =
                    @"IF APPLOCK_MODE('public', @Resource, @LockOwner) != 'NoLock'
                        EXEC @Result = dbo.sp_releaseapplock @Resource, @LockOwner
                      ELSE
                        SET @Result = 0"
                ;
            }
            else
            {
                command.CommandText = "dbo.sp_releaseapplock";
                command.CommandType = CommandType.StoredProcedure;
            }

            command.Parameters.Add(new SqlParameter("Resource", lockName));
            command.Parameters.Add(new SqlParameter("LockOwner", "Session"));

            if (isTry)
            {
                returnValue = command.Parameters.Add(new SqlParameter { ParameterName = "Result", DbType = DbType.Int32, Direction = ParameterDirection.Output });
            }
            else
            {
                returnValue = command.Parameters.Add(new SqlParameter { DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue });
            }

            return command;
        }

        /// <summary>
        /// sp_releaseapplock exit codes documented at
        /// https://docs.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-releaseapplock-transact-sql#return-code-values
        /// </summary>
        /// <param name="returnCode">code returned after calling sp_releaseapplock</param>
        /// <returns>true/false</returns>
        public static bool ParseReturnCode(int returnCode)
        {
            if (returnCode >= 0)
            {
                return true;
            }

            throw new InvalidOperationException($"Could not release lock with return code: {returnCode}");
        }
    }
}
