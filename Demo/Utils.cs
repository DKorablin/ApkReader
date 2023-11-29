using System;
using System.Reflection;
using System.Text;

namespace Demo
{
	public struct Utils
	{
		public static String GetReflectedMembers(Object obj)
		{
			if(obj == null)
				return "<NULL>";


			StringBuilder result = new StringBuilder();
			Type objType = obj.GetType();
			if(objType.Assembly.GetName().Name == "mscorlib")
				result.Append(obj.ToString() + "\t");
			else
			{
				foreach(PropertyInfo prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
					result.AppendFormat("{0}: {1}\t", prop.Name, prop.GetValue(obj, null));
				foreach(FieldInfo field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
					result.AppendFormat("{0}: {1}\t", field.Name, field.GetValue(obj));
			}

			return result.ToString();
		}

		public static Boolean IsGenericType(Type type, Type genericType)
		{
			if(type.IsGenericType)
			{
				Type genericDef = type.GetGenericTypeDefinition();
				return genericDef == genericType;
			} else return false;
		}
	}
}
