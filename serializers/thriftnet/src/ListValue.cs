
namespace ProtoThrift
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;

  using Thrift.Protocol;

  public class ListValue<V> : IProtoValue<List<V>>
  {
    public ListValue(IProtoValue<V> valueMetadata)
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
      ParameterExpression value = Expression.Variable(typeof(List<V>));
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
          Expression.New(typeof(List<V>).GetConstructor(typeof(int)), count)
        ),
        i,
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            Expression.Call(
              value,
              typeof(List<V>).GetMethod("Add"),
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
      CommonUtils.ThrowIfFalse(value.Type == typeof(List<V>));

      Expression count = Expression.Property(value, "Count");
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
              Expression.MakeIndex(value, typeof(List<V>).GetProperty("Item"), new [] {i} )
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteListEnd"))
      );
    }

    public List<V> Read(TProtocol iprot)
    {
      TList tList = iprot.ReadListBegin();
      List<V> value = new List<V>(tList.Count);
      for (int i = 0; i < tList.Count; ++i)
      {
        V element = this.ValueMetadata.Read(iprot);
        value.Add(element);
      }
      iprot.ReadListEnd();
      return value;
    }

    public void Write(TProtocol oprot, List<V> value)
    {
      TList tList = new TList(this.ValueMetadata.Type, value.Count);
      oprot.WriteListBegin(tList);
      for (int i = 0; i < value.Count; i++)
      {
        this.ValueMetadata.Write(oprot, value[i]);
      }
      oprot.WriteListEnd();
    }
  }
}
