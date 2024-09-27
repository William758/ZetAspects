using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System;
using System.Reflection;

namespace TPDespair.ZetAspects
{
	public static class ReflectionUtility
	{
		public class Reflector
		{
			public string GUID;
			public string identifier;

			public Assembly PluginAssembly;

			internal static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			private Reflector()
			{

			}

			public Reflector(string GUID, string identifier)
			{
				this.GUID = GUID;
				this.identifier = identifier;
			}

			public bool FindPluginAssembly()
			{
				Logger.Warn(identifier + " - Find Assembly for : " + GUID);

				if (!Chainloader.PluginInfos.ContainsKey(GUID))
				{
					Logger.Warn(identifier + " - No plugin with that GUID found!");
					return false;
				}

				BaseUnityPlugin plugin = Chainloader.PluginInfos[GUID].Instance;
				PluginAssembly = Assembly.GetAssembly(plugin.GetType());

				if (PluginAssembly == null)
				{
					Logger.Warn(identifier + " - Could Not Get Assembly");

					return false;
				}

				return true;
			}
		}



		public static Type GetType(this Reflector reflector, string typeName)
		{
			Type type = Type.GetType(typeName + ", " + reflector.PluginAssembly.FullName, false);
			if (type != null)
			{
				return type;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find Type : " + typeName);
				return null;
			}
		}

		public static Type GetType(this Reflector reflector, string typeName, string nestedTypeName)
		{
			Type type = reflector.GetType(typeName);
			if (type != null)
			{
				return reflector.GetType(type, nestedTypeName);
			}

			return null;
		}

		public static Type GetType(this Reflector reflector, Type type, string nestedTypeName)
		{
			type = type.GetNestedType(nestedTypeName, Reflector.Flags);
			if (type != null)
			{
				return type;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find NestedType : " + type.Name + "." + nestedTypeName);
				return null;
			}
		}



		public static FieldInfo GetField(this Reflector reflector, string typeName, string fieldName)
		{
			Type type = reflector.GetType(typeName);
			if (type != null)
			{
				return reflector.GetField(type, fieldName);
			}

			return null;
		}

		public static FieldInfo GetField(this Reflector reflector, Type type, string fieldName)
		{
			FieldInfo fieldInfo = type.GetField(fieldName, Reflector.Flags);
			if (fieldInfo != null)
			{
				return fieldInfo;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find Field : " + type.Name + "." + fieldName);
				return null;
			}
		}

		public static FieldInfo GetField(this Reflector reflector, string typeName, string nestedTypeName, string fieldName)
		{
			Type type = reflector.GetType(typeName, nestedTypeName);
			if (type != null)
			{
				return reflector.GetField(type, fieldName);
			}

			return null;
		}



		public static MethodInfo GetMethod(this Reflector reflector, string typeName, string methodName)
		{
			Type type = reflector.GetType(typeName);
			if (type != null)
			{
				return reflector.GetMethod(type, methodName);
			}

			return null;
		}

		public static MethodInfo GetMethod(this Reflector reflector, Type type, string methodName)
		{
			MethodInfo methodInfo = type.GetMethod(methodName, Reflector.Flags);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find Method : " + type.Name + "." + methodName);
				return null;
			}
		}

		public static MethodInfo GetMethod(this Reflector reflector, string typeName, string nestedTypeName, string methodName)
		{
			Type type = reflector.GetType(typeName, nestedTypeName);
			if (type != null)
			{
				return reflector.GetMethod(type, methodName);
			}

			return null;
		}



		public static MethodInfo GetMatchingMethod(this Reflector reflector, Type type, Type[] pars)
		{
			MethodInfo[] methodInfos = type.GetMethods(Reflector.Flags);
			for (int i = 0; i < methodInfos.Length; i++)
			{
				MethodInfo methodInfo = methodInfos[i];
				ParameterInfo[] parInfos = methodInfo.GetParameters();
				if (parInfos.Length == pars.Length)
				{
					for (int j = 0; j < pars.Length; j++)
					{
						if (pars[j] != null && parInfos[j].ParameterType != pars[j]) break;

						if (j == pars.Length - 1)
						{
							return methodInfo;
						}
					}
				}
			}

			Logger.Warn(reflector.identifier + " - Could Not Find Any Method In : " + type.FullName + " With Parameters : " + ParameterNames(pars));

			return null;
		}

		private static string ParameterNames(Type[] pars)
		{
			string result = "";

			for (int i = 0; i < pars.Length; i++)
			{
				if (pars[i] == null)
				{
					result += "*any*";
				}
				else
				{
					result += pars[i].Name;
				}

				if (i < pars.Length - 1)
				{
					result += ", ";
				}
			}

			return result;
		}




		public class FieldRefer<T>
		{
			public FieldInfo fieldInfo;
			public object instance;
			public T defaultValue;
			public bool valid = false;



			public FieldRefer(T defaultValue)
			{
				this.defaultValue = defaultValue;

				valid = false;
			}

			public FieldRefer(FieldInfo fieldInfo, object instance, T defaultValue)
			{
				this.fieldInfo = fieldInfo;
				this.instance = instance;
				this.defaultValue = defaultValue;

				valid = true;

				if (fieldInfo == null)
				{
					valid = false;

					Logger.Warn("FieldRefer - Assigned null FieldInfo!");
				}
				else
				{
					try
					{
						T test = (T)this.fieldInfo.GetValue(this.instance);
					}
					catch (Exception e)
					{
						Logger.Warn(e);

						valid = false;
					}
				}
			}



			public static implicit operator T(FieldRefer<T> fieldRefer)
			{
				if (!fieldRefer.valid) return fieldRefer.defaultValue;

				return (T)fieldRefer.fieldInfo.GetValue(fieldRefer.instance);
			}



			public void SetInfo(FieldInfo fieldInfo, object instance)
			{
				this.fieldInfo = fieldInfo;
				this.instance = instance;

				valid = true;

				if (fieldInfo == null)
				{
					valid = false;

					Logger.Warn("FieldRefer - Assigned null FieldInfo!");
				}
				else
				{
					try
					{
						T test = (T)this.fieldInfo.GetValue(this.instance);
					}
					catch (Exception e)
					{
						Logger.Warn(e);

						valid = false;
					}
				}
			}

			public void SetValue(T value)
			{
				if (!valid)
				{
					Logger.Warn("FieldRefer - Tried to assign value to Invalid!");
					return;
				}

				fieldInfo.SetValue(instance, value);
			}
		}
	}
}
