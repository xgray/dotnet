
//------------------------------------------------------------------------------
// This code was generated by a tool.
//
//   Tool : Bond Compiler 0.7.0.0
//   File : stress_types.cs
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// <auto-generated />
//------------------------------------------------------------------------------


// suppress "Missing XML comment for publicly visible type or member"
#pragma warning disable 1591


#region ReSharper warnings
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Local
// ReSharper disable RedundantUsingDirective
#endregion

namespace bond
{
    using System.Collections.Generic;

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Simple
    {
        [global::Bond.Id(1)]
        public string Value { get; set; }

        [global::Bond.Id(2)]
        public int ShortValue { get; set; }

        [global::Bond.Id(3)]
        public int IntValue { get; set; }

        [global::Bond.Id(4)]
        public long LongValue { get; set; }

        public Simple()
            : this("bond.Simple", "Simple")
        {}

        protected Simple(string fullName, string name)
        {
            Value = "";
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Complex
    {
        [global::Bond.Id(1)]
        public Simple SimpleValue { get; set; }

        [global::Bond.Id(2)]
        public List<Simple> ListValue { get; set; }

        [global::Bond.Id(4)]
        public Dictionary<string, Simple> MapValue { get; set; }

        [global::Bond.Id(6)]
        public Dictionary<string, LinkedList<Dictionary<string, Simple>>> MapList { get; set; }

        [global::Bond.Id(7)]
        public LinkedList<LinkedList<string>> ListOfList { get; set; }

        [global::Bond.Id(8)]
        public LinkedList<LinkedList<LinkedList<string>>> ListOfListOfList { get; set; }

        public Complex()
            : this("bond.Complex", "Complex")
        {}

        protected Complex(string fullName, string name)
        {
            SimpleValue = new Simple();
            ListValue = new List<Simple>();
            MapValue = new Dictionary<string, Simple>();
            MapList = new Dictionary<string, LinkedList<Dictionary<string, Simple>>>();
            ListOfList = new LinkedList<LinkedList<string>>();
            ListOfListOfList = new LinkedList<LinkedList<LinkedList<string>>>();
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Record
    {
        [global::Bond.Id(0)]
        public string Name { get; set; }

        [global::Bond.Id(1)]
        public List<double> Constants { get; set; }

        public Record()
            : this("bond.Record", "Record")
        {}

        protected Record(string fullName, string name)
        {
            Name = "";
            Constants = new List<double>();
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class GuidExample
    {
        [global::Bond.Id(0)]
        public string id_str { get; set; }

        [global::Bond.Id(1), global::Bond.Type(typeof(global::Bond.Tag.blob))]
        public System.ArraySegment<byte> id_bin { get; set; }

        public GuidExample()
            : this("bond.GuidExample", "GuidExample")
        {}

        protected GuidExample(string fullName, string name)
        {
            id_str = "";
            id_bin = new System.ArraySegment<byte>();
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Generic1<T>
    {
        [global::Bond.Id(0), global::Bond.Type(typeof(global::Bond.Tag.nullable<global::Bond.Tag.classT>))]
        public T Field { get; set; }

        public Generic1()
            : this("bond.Generic1", "Generic1")
        {}

        protected Generic1(string fullName, string name)
        {
            
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Generic2<T>
        where T : struct
    {
        [global::Bond.Id(0), global::Bond.Type(typeof(global::Bond.Tag.structT?))]
        public T? Field { get; set; }

        public Generic2()
            : this("bond.Generic2", "Generic2")
        {}

        protected Generic2(string fullName, string name)
        {
            
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class GenericExample
    {
        [global::Bond.Id(0)]
        public Generic1<Generic2<int>> Field { get; set; }

        public GenericExample()
            : this("bond.GenericExample", "GenericExample")
        {}

        protected GenericExample(string fullName, string name)
        {
            Field = new Generic1<Generic2<int>>();
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public enum ShapeType
    {
        Unknown,
        Circle,
        Rectange,
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Shape
    {
        [global::Bond.Id(0)]
        public ShapeType Type { get; set; }

        public Shape()
            : this("bond.Shape", "Shape")
        {}

        protected Shape(string fullName, string name)
        {
            Type = ShapeType.Unknown;
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Circle
        : Shape
    {
        [global::Bond.Id(0)]
        public double Radius { get; set; }

        public Circle()
            : this("bond.Circle", "Circle")
        {}

        protected Circle(string fullName, string name)
        {
            
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Rectangle
        : Shape
    {
        [global::Bond.Id(0)]
        public double Width { get; set; }

        [global::Bond.Id(1)]
        public double Height { get; set; }

        public Rectangle()
            : this("bond.Rectangle", "Rectangle")
        {}

        protected Rectangle(string fullName, string name)
        {
            
        }
    }

    [global::Bond.Schema]
    [System.CodeDom.Compiler.GeneratedCode("gbc", "0.7.0.0")]
    public partial class Polymorphic
    {
        [global::Bond.Id(0)]
        public List<global::Bond.IBonded<Shape>> Shapes { get; set; }

        public Polymorphic()
            : this("bond.Polymorphic", "Polymorphic")
        {}

        protected Polymorphic(string fullName, string name)
        {
            Shapes = new List<global::Bond.IBonded<Shape>>();
        }
    }
} // bond
