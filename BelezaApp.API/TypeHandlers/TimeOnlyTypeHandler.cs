namespace BelezaApp.API.TypeHandlers;

using System.Data;
using Dapper;

public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value;
    }

    public override TimeOnly Parse(object value)
    {
        return value switch
        {
            TimeSpan ts => TimeOnly.FromTimeSpan(ts),
            DateTime dt => TimeOnly.FromDateTime(dt),
            TimeOnly to => to,
            _ => TimeOnly.Parse(value.ToString()!)
        };
    }
}
