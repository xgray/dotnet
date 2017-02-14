
namespace ProtoThrift
{
  using System;
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;
  using Thrift.Protocol;

  public class ArrayValue<V> : IProtoValue<V[]>
  {
    public ArrayValue(IProtoValue<V> valueMetadata)
    {
      this.ValueMetadata = valueMetadata;
    }

    public IProtoValue<V> ValueMetadata { get; private set; }

    public TType Type
    {
      get { return TType.List; }
    }

    public Expression Read(Expression iprot)
    {
      ParameterExpression tList = Expression.Variable(typeof(TList));
      Expression count = Expression.Property(tList, typeof(TList).GetProperty("Count"));
      ParameterExpression value = Expression.Variable(typeof(V[]));
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { tList, value, i },
        Expression.Assign(
          tList,
          Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadListBegin"))
        ),
        Expression.Assign(
          value,
          Expression.New(typeof(V[]).GetConstructor(typeof(int)), count)
        ),
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            Expression.Assign(
              Expression.ArrayAccess(value, i),
              this.ValueMetadata.Read(iprot)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadListEnd")),
        value
      );
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(V[]));

      Expression count = Expression.Property(value, "Length");
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { i },
        Expression.Call(
          oprot,
          typeof(TProtocol).GetMethod("WriteListBegin"),
          Expression.New(
            typeof(TList).GetConstructor(typeof(TType), typeof(int)),
            Expression.Constant(this.ValueMetadata.Type),
            count
          )
        ),
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            this.ValueMetadata.Write(
              oprot,
              Expression.ArrayIndex(value, i)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteListEnd"))
      );
    }

    public V[] Read(TProtocol iprot)
    {
      TList tList = iprot.ReadListBegin();
      V[] value = new V[tList.Count];
      for (int i = 0; i < tList.Count; ++i)
      {
        V element = this.ValueMetadata.Read(iprot);
        value[i] = element;
      }
      iprot.ReadListEnd();
      return value;
    }

    public void Write(TProtocol oprot, V[] value)
    {
      TList tList = new TList(this.ValueMetadata.Type, value.Length);
      oprot.WriteListBegin(tList);
      for (int i = 0; i < value.Length; i++)
      {
        this.ValueMetadata.Write(oprot, value[i]);
      }
      oprot.WriteListEnd();
    }
  }
}
