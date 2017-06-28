using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using Dg.Deblazer.Extensions;
using JetBrains.Annotations;

namespace Dg.Deblazer.Read
{
    internal class PrimitiveTypeFiller
    {
        public static readonly Dictionary<Type, Func<IDataRecord, int, object>> FillersByType = new Dictionary<Type, Func<IDataRecord, int, object>>();
        public static readonly IImmutableSet<Type> TypesToTreatAsPrimitives = ImmutableHashSet.Create
        (
            typeof(Date),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(string),
            typeof(decimal),
            typeof(Binary)
        );

        private static void AddFiller<TPrimitiveType>(Func<IDataRecord, int, TPrimitiveType> f)
        {
            FillersByType.Add(typeof(TPrimitiveType), (reader, index) => f(reader, index));
        }

        static PrimitiveTypeFiller()
        {
            AddFiller(GetInt32);
            AddFiller(GetNullableInt32);
            AddFiller(GetLong);
            AddFiller(GetNullableLong);
            AddFiller(GetString);
            AddFiller(GetChar);
            AddFiller(GetNullableChar);
            AddFiller(GetBinary);
            AddFiller(GetDouble);
            AddFiller(GetDecimal);
            AddFiller(GetNullableDecimal);
            AddFiller(GetDateTime);
            AddFiller(GetNullableDateTime);
            AddFiller(GetDateTimeOffset);
            AddFiller(GetBoolean);
            AddFiller(GetDate);
            AddFiller(GetNullableDate);
            AddFiller(GetTimeSpan);
            AddFiller(GetByte);
            AddFiller(GetUnsignedLong);
            AddFiller(GetNullableUnsignedLong);
        }

        public static Func<IDataReader, int, object> GetFillMethod<TType>()
        {
            Func<IDataRecord, int, object> fillMethod;
            if (FillersByType.TryGetValue(typeof(TType), out fillMethod))
            {
                return  fillMethod;
            }

            return (reader, index) => GetValue<TType>(reader, index);
        }

        public static TType GetReaderValue<TType>(IDataRecord reader, int index)
        {
            Func<IDataRecord, int, object> fillMethod;
            if (!FillersByType.TryGetValue(typeof(TType), out fillMethod))
            {
                fillMethod = (r, i) => GetValue<TType>(r, i);
            }

            return (TType)fillMethod(reader, index);
        }

        public static Binary GetRowVersion(IDataRecord reader, int index)
        {
            var obj = reader.GetValue(index);
            return new Binary((byte[])obj);
        }
        
        public static Binary GetBinary(IDataRecord reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                return default(Binary);
            }
            
            var obj = reader.GetValue(index);
            
            return new Binary((byte[])obj);
        }

        public static int GetInt32(IDataRecord reader, int index)
        {
            return reader.GetInt32(index);
        }

        public static int? GetNullableInt32(IDataRecord reader, int index)
        {
            int? value = reader.IsDBNull(index) ? (int?)null : reader.GetInt32(index);
            return value;
        }

        public static decimal GetMoney(IDataRecord reader, int index)
        {
            var sqlDataReader = reader as SqlDataReader;

            if (sqlDataReader != null)
            {
                return reader.GetDecimal(index);
            }

            var sqlDataReader = reader as SqlDataReader;
            return (decimal)sqlDataReader.GetSqlMoney(index);
        }

        public static decimal? GetNullableMoney(IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (decimal?)null : GetMoney(reader, index);
        }

        public static long GetLong(IDataRecord reader, int index)
        {
            return reader.GetInt64(index);
        }

        public static long? GetNullableLong(IDataRecord reader, int index)
        {
            long? value = reader.IsDBNull(index) ? (long?)null : reader.GetInt64(index);
            return value;
        }

        public static ulong GetUnsignedLong(IDataRecord reader, int index)
        {
            var value = reader.GetValue(index);
            return BinaryExtensions.RowVersionBytesToUInt64((byte[])value);
        }

        public static ulong? GetNullableUnsignedLong(IDataRecord reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                return default(ulong?);
            }
            var value = reader.GetValue(index);
            var bytes = (byte[])value;
            if (bytes.Length == 0)
            {
                return default(ulong?);
            }

            return BinaryExtensions.RowVersionBytesToUInt64(bytes);
        }

        public static double GetDouble(IDataRecord reader, int index)
        {
            return reader.GetDouble(index);
        }

        public static decimal? GetNullableDecimal(IDataRecord reader, int index)
        {
            decimal? result;
            var sqlDataReader = reader as SqlDataReader;
            if (sqlDataReader == null)
            {
                // if for some reason we are not using a SqlDataReader anymore, we fallback to default method (useful for tests)
                return reader.IsDBNull(index) ? default(decimal?) : reader.GetDecimal(index);
            }

            var sqlDecimal = sqlDataReader.GetSqlDecimal(index);
            result = ToDecimal(sqlDecimal);

            return result;
        }

        [CanBeNull]
        public static decimal? ToDecimal(System.Data.SqlTypes.SqlDecimal sqlDecimal)
        {
            if (sqlDecimal.IsNull)
            {
                return null;
            }

            const int decimalMaxPrecision = 28;
            const int decimalMaxScale = 28;

            if (sqlDecimal.Precision <= decimalMaxPrecision)
            {
                // the maximum precision for decimal is 28. If the precision is higher accessing the Value property will throw an exception
                return sqlDecimal.Value;
            }
            else
            {
                // There is no easy way to actually convert all SqlDecimals which can be converted to System.Decimal.
                // Therefore we have to find a scale that works without truncating the value by brute force.
                for (int scale = 0; scale <= decimalMaxScale; scale++)
                {
                    var normalized = System.Data.SqlTypes.SqlDecimal.ConvertToPrecScale(sqlDecimal, precision: decimalMaxPrecision, scale: scale);
                    if (normalized == sqlDecimal)
                    {
                        return normalized.Value;
                    }
                }

                throw new OverflowException("SqlDecimal has a higher accuracy than can be represented by System.Decimal. Conversion failed");
            }
        }

        public static decimal GetDecimal(IDataRecord reader, int index)
        {
            return GetNullableDecimal(reader, index).Value;
        }

        public static DateTimeOffset GetDateTimeOffset(IDataRecord reader, int index)
        {
            var sqlDataReader = reader as SqlDataReader;
            if (sqlDataReader == null)
            {
                throw new NotSupportedException("DateTimeOffset is only supported with SqlDataReader at this point.");
            }

            return sqlDataReader.GetDateTimeOffset(index);
        }

        public static bool GetBoolean(IDataRecord reader, int index)
        {
            return reader.GetBoolean(index);
        }

        public static byte GetByte(IDataRecord reader, int index)
        {
            return reader.GetByte(index);
        }

        public static DateTime GetDateTime(IDataRecord reader, int index)
        {
            return reader.GetDateTime(index);
        }

        public static DateTime? GetNullableDateTime(IDataRecord reader, int index)
        {
            DateTime? tmp = reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
            return tmp;
        }

        public static Date GetDate(IDataRecord reader, int index)
        {
            return (Date)GetDateTime(reader, index);
        }

        public static Date? GetNullableDate(IDataRecord reader, int index)
        {
            Date? tmp = reader.IsDBNull(index) ? (Date?)null : (Date)reader.GetDateTime(index);
            return tmp;
        }

        public static TimeSpan GetTimeSpan(IDataRecord reader, int index)
        {
            var sqlDataReader = reader as SqlDataReader;
            if (sqlDataReader == null)
            {
                throw new NotSupportedException("TimeSpan is only supported with SqlDataReader at this point.");
            }

            return sqlDataReader.GetTimeSpan(index++);
        }

        public static char GetChar(IDataRecord reader, int index)
        {
            return reader.GetString(index)[0];
        }

        public static string GetString(IDataRecord reader, int index)
        {
            return reader.GetString(index);
        }

        public static char? GetNullableChar(IDataRecord reader, int index)
        {
            var value = reader.GetValue(index);
            char? tmp = Convert.IsDBNull(value) ? (char?)null : ((string)value)[0];
            return tmp;
        }

        public static T GetValue<T>(IDataRecord reader, int index)
        {
            if (reader.IsDBNull(index))
            {
                // Check for DBNull first to avoid allocating an Object on the heap if the value is DBNull.
                return default(T);
            }

            var value = reader.GetValue(index);

            T tmp = value == null ? default(T) : (T)value;
            index++;
            return tmp;
        }
    }
}