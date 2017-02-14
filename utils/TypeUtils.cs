
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public static partial class TypeUtils
    {
        /// <summary>
        /// look up table for types by their names
        /// </summary>
        private static ConcurrentDictionary<string, Type> typeDict =
            new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets type from the name
        /// </summary>
        /// <param name="name">type full name</param>
        /// <returns>type object</returns>
        public static Type GetType(string name)
        {
            try
            {
                Type type;
                if (typeDict.TryGetValue(name, out type))
                {
                    return type;
                }

                type = Type.GetType(name);
                if (type != null)
                {
                    typeDict[name] = type;
                    return type;
                }

#if !CORE
                foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        type = assembly.GetType(name);
                        if (type != null)
                        {
                            typeDict[name] = type;
                            return type;
                        }
                    }
                    catch
                    {
                    }
                }
#endif
            }
            catch
            {
            }

            return null;
        }

        public static Type[] GetTypes<T>(this Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes().Where(type => type.HasCustomAttribute<T>()).ToArray();
        }

        /// <summary>
        /// convert object to type T
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="x">input object</param>
        /// <returns>converted type</returns>
        public static T cast<T>(this object x) where T : class
        {
            return x as T;
        }

        /// <summary>
        /// checks if two objects are equal
        /// </summary>
        /// <param name="x">first object</param>
        /// <param name="y">second object</param>
        /// <returns>true if two objects are equal</returns>
        public static bool ObjectEquals(object x, object y)
        {
            return (x == null && y == null) 
                || (x != null && y != null && x.Equals(y));
        }

        public static T GetProperty<T>(object x, string property)
        {
            PropertyInfo pi = x.GetType().GetProperty(property);
            return (T)pi.GetMethod.Invoke(x, Array<object>.Empty);
        }

        public static void SetProperty<T>(object x, string property, T value)
        {
            PropertyInfo pi = x.GetType().GetProperty(property);
            pi.SetMethod.Invoke(x, new object[] { value });
        }

#if CORE
        // public static PropertyInfo[] GetProperties(this Type type)
        // {
        //     return type.GetTypeInfo().GetProperties();
        // }

        // public static FieldInfo[] GetFields(this Type type)
        // {
        //     return type.GetTypeInfo().GetFields();
        // }

        // public static MethodInfo[] GetMethods(this Type type)
        // {
        //     return type.GetTypeInfo().GetMethods();
        // }

        // public static MethodInfo GetMethod(this Type type, string method)
        // {
        //     return type.GetMethods().Single(m => m.Name == method);
        // }

		// public static IEnumerable<Attribute> GetCustomAttributes(this Type type)
		// {
		// 	return type.GetTypeInfo().GetCustomAttributes(false);
		// }

		// public static IEnumerable<Attribute> GetCustomAttributes(this Type type, Type attributeType, bool inherit = false)
		// {
		// 	return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
		// }

        public static InterfaceMapping GetInterfaceMap(this Type type, Type iType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(iType);
        }

        public static ConstructorInfo GetConstructor(this Type type, params Type[] types)
        {
            return TypeExtensions.GetConstructor(type, types);
        }
        
#else
        public static ConstructorInfo GetConstructor(this Type type, params Type[] types)
        {
            return type.GetConstructor(types);
        }

#endif

        public static bool HasCustomAttribute(this Type type, Type attributeType, bool inherit = false)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Any();
        }

        public static bool HasCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>(inherit).Any();
        }

        public static bool HasCustomAttribute(this MemberInfo memberInfo, Type attributeType, bool inherit = false)
        {
            return memberInfo.GetCustomAttributes(attributeType, inherit).Any();
        }

        public static bool HasCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>(inherit).Any();
        }

        public static T[] GetCustomAttributes<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return (T[])type.GetTypeInfo().GetCustomAttributes(typeof(T), inherit);
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            return (T)type.GetTypeInfo().GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return (T[])memberInfo.GetCustomAttributes(typeof(T), inherit);
        }

        public static MethodInfo GetInterfaceMethod(this Type type, MethodInfo iMethod)
        {
            InterfaceMapping im = type.GetInterfaceMap(iMethod.DeclaringType);
            for( int i = 0; i < im.InterfaceMethods.Length; i++)
            {
                if( im.InterfaceMethods[i] == iMethod)
                {
                    return im.TargetMethods[i];
                }
            }

            return null;
        }
    }

}
