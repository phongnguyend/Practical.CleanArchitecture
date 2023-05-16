using ClassifiedAds.CrossCuttingConcerns.Locks;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassifiedAds.Persistence.Locks;

public class SqlDistributedLockScope : IDistributedLockScope
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction _transaction;
    private readonly string _lockName;

    public bool HasTransaction
    {
        get
        {
            return _transaction != null;
        }
    }

    public SqlDistributedLockScope(SqlConnection connection, SqlTransaction transaction, string lockName)
    {
        _connection = connection;
        _transaction = transaction;
        _lockName = lockName;
    }

    public void Dispose()
    {
        if (HasTransaction)
        {
            return;
        }

        SqlParameter returnValue;
        var releaseCommand = CreateReleaseCommand(_lockName, false, out returnValue);
        releaseCommand.ExecuteNonQuery();

        if (ParseReturnCode((int)returnValue.Value))
        {
        }
    }

    public bool StillHoldingLock()
    {
        var command = _connection.CreateCommand();
        command.Transaction = _transaction;

        command.CommandText = @"SELECT APPLOCK_MODE('public', @Resource, @LockOwner)";
        command.Parameters.Add(new SqlParameter("Resource", _lockName));
        command.Parameters.Add(new SqlParameter("LockOwner", HasTransaction ? "Transaction" : "Session"));
        var lockMode = (string)command.ExecuteScalar();

        return lockMode != "NoLock";
    }

    private SqlCommand CreateReleaseCommand(string lockName, bool isTry, out SqlParameter returnValue)
    {
        var command = _connection.CreateCommand();
        command.Transaction = _transaction;

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
        command.Parameters.Add(new SqlParameter("LockOwner", HasTransaction ? "Transaction" : "Session"));

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
