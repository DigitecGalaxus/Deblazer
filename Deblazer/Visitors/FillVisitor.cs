using System;
using System.Data.Linq;
using System.Data.SqlClient;
using Dg.Deblazer.Internal;
using Dg.Deblazer.Read;
using Dg.Deblazer.Extensions;

namespace Dg.Deblazer.Visitors
{
    public class FillVisitor
    {
        private readonly SqlDataReader reader;
        private readonly IObjectFillerFactory objectFillerFactory;

        private bool hasNext;

        public bool HasNext { get { return hasNext && reader.FieldCount > index; } }

        private int index;

        public readonly IDb Db;
        internal IDbInternal DbInternal => (IDbInternal)Db;

        public FillVisitor(SqlDataReader reader, IDb db, IObjectFillerFactory objectFillerFactory)
        {
            this.reader = reader;
            Db = db;
            this.objectFillerFactory = objectFillerFactory;
        }

        public Binary GetRowVersion()
        {
            return PrimitiveTypeFiller.GetBinary(reader, index++);
        }

        public Binary GetBinary()
        {
            if (!Db.Settings.AllowLoadingBinaryData)
            {
                throw new Exception("Loading binary data is forbidden by default and has to be allowed explicitly");
            }

            return PrimitiveTypeFiller.GetBinary(reader, index++);
        }

        public int GetInt32()
        {
            return PrimitiveTypeFiller.GetInt32(reader, index++);
        }

        public int? GetNullableInt32()
        {
            return PrimitiveTypeFiller.GetNullableInt32(reader, index++);
        }

        public double GetDouble()
        {
            return PrimitiveTypeFiller.GetDouble(reader, index++);
        }

        public decimal GetDecimal()
        {
            return PrimitiveTypeFiller.GetDecimal(reader, index++);
        }

        public decimal? GetNullableDecimal()
        {
            return PrimitiveTypeFiller.GetNullableDecimal(reader, index++);
        }

        public decimal GetMoney()
        {
            return PrimitiveTypeFiller.GetMoney(reader, index++);
        }

        public decimal? GetNullableMoney()
        {
            return PrimitiveTypeFiller.GetNullableMoney(reader, index++);
        }

        public DateTimeOffset GetDateTimeOffset()
        {
            return PrimitiveTypeFiller.GetDateTimeOffset(reader, index++);
        }

        public bool GetBoolean()
        {
            return PrimitiveTypeFiller.GetBoolean(reader, index++);
        }

        public byte GetByte()
        {
            return PrimitiveTypeFiller.GetByte(reader, index++);
        }

        public DateTime GetDateTime()
        {
            return PrimitiveTypeFiller.GetDateTime(reader, index++);
        }

        public Date GetDate()
        {
            return PrimitiveTypeFiller.GetDate(reader, index++);
        }

        public Date? GetNullableDate()
        {
            return PrimitiveTypeFiller.GetNullableDate(reader, index++);
        }

        public TimeSpan GetTimeSpan()
        {
            return PrimitiveTypeFiller.GetTimeSpan(reader, index++);
        }

        public char GetChar()
        {
            return PrimitiveTypeFiller.GetChar(reader, index++);
        }

        public char? GetNullableChar()
        {
            return PrimitiveTypeFiller.GetNullableChar(reader, index++);
        }

        public T GetValue<T>()
        {
            return PrimitiveTypeFiller.GetValue<T>(reader, index++);
        }

        public object Fill(Type type)
        {
            // Use the generic fill method
            var objectFiller = objectFillerFactory.GetObjectFiller();
            object entity = objectFiller.Build(type, reader);
            return entity;
        }

        public T Fill<T>()
        {
            var t = PrimitiveTypeFiller.GetFillMethod<T>();
            var type = typeof(T);

            if (type.IsPrimitive || PrimitiveTypeFiller.TypesToTreatAsPrimitives.Contains(type))
            {
                return PrimitiveTypeFiller.GetReaderValue<T>(reader, index: 0);
            }
            else if (type.IsNullable()
                && !IdTypeExtension.IsNullableIdType<T>()
                && !LongIdTypeExtension.IsNullableLongIdType<T>())
            {
                if (reader.IsDBNull(0))
                {
                    return default(T);
                }

                return PrimitiveTypeFiller.GetReaderValue<T>(reader, index: 0);
            }
            else if (type == typeof(object))
            {
                return (T)reader.GetValue(0);
            }
            else
            {
                var filler = objectFillerFactory.GetObjectFiller();
                T entity = (T)filler.Build(type, reader);
                return entity;
            }
        }

        public bool Read()
        {
            index = 0;
            hasNext = reader.Read();
            return HasNext;
        }

        public bool IsDBNull()
        {
            return reader.IsDBNull(index);
        }

        public void Skip(int columnCount)
        {
            index += columnCount;
        }
    }
}