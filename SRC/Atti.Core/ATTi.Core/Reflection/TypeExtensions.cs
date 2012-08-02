namespace ATTi.Core.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class TypeExtensions
	{
		

		public static bool ContainsAnyFieldMatching(this Type t, Func<FieldInfo, bool> match)
		{
			foreach (var f in t.GetFields())
			{
				if (match(f))
					return true;
			}
			return false;
		}

		public static bool ContainsAnyFieldMatching(this Type t, BindingFlags flags, Func<FieldInfo, bool> match)
		{
			foreach (var f in t.GetFields(flags))
			{
				if (match(f))
					return true;
			}
			return false;
		}

		public static bool DeclaresMethodWithAttribute<TAttribute>(this Type t, bool inherit)
		{
			return (from m in t.GetMethods()
							where m.IsDefined(typeof(TAttribute), inherit)
							select m).SingleOrDefault() != null;
		}

		public static bool DeclaresMethodWithAttribute<TAttribute>(this Type t, BindingFlags binding, bool inherit)
		{
			return (from m in t.GetMethods(binding)
							where m.IsDefined(typeof(TAttribute), inherit)
							select m).SingleOrDefault() != null;
		}

		/// <summary>
		/// Discovers the element type of the present type when used as an enumerable.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <returns>The element type if the present type is enumerable; otherwise the present type itself.</returns>
		public static Type DiscoverElementType(this Type t)
		{
			Type ienum = FindIEnumerable(t);
			if (ienum == null) return t;
			return ienum.GetGenericArguments()[0];
		}

		public static Type DiscoverFirstCompatibleGenericInterfaceImplementation(this Type t
			, Type genericTypeDef, params Type[] typeArgs)
		{
			Contracts.Require.IsNotNull("typeArgs", typeArgs);
			Contracts.Require.IsNotNull("genericTypeDef", genericTypeDef);
			Contracts.Require.IsGenericTypeDefinition("genericTypeDef", genericTypeDef);
			Contracts.Require.IsEqual(genericTypeDef.GetGenericArguments().Length, typeArgs.Length);

			switch (typeArgs.Length)
			{
				case 1: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
												select ii).First();
				case 2: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
												select ii).First();
				case 3: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
												select ii).First();
				case 4: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
												select ii).First();
				case 5: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
													&& typeArgs[4].IsAssignableFrom(gargs[4])
												select ii).First();
				case 6: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
													&& typeArgs[4].IsAssignableFrom(gargs[4])
													&& typeArgs[5].IsAssignableFrom(gargs[5])
												select ii).First();
				case 7: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
													&& typeArgs[4].IsAssignableFrom(gargs[4])
													&& typeArgs[5].IsAssignableFrom(gargs[5])
													&& typeArgs[6].IsAssignableFrom(gargs[6])
												select ii).First();
				case 8: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
													&& typeArgs[4].IsAssignableFrom(gargs[4])
													&& typeArgs[5].IsAssignableFrom(gargs[5])
													&& typeArgs[6].IsAssignableFrom(gargs[6])
													&& typeArgs[7].IsAssignableFrom(gargs[7])
												select ii).First();
				case 9: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												let gargs = ii.GetGenericArguments()
												where typeArgs[0].IsAssignableFrom(gargs[0])
													&& typeArgs[1].IsAssignableFrom(gargs[2])
													&& typeArgs[2].IsAssignableFrom(gargs[2])
													&& typeArgs[3].IsAssignableFrom(gargs[3])
													&& typeArgs[4].IsAssignableFrom(gargs[4])
													&& typeArgs[5].IsAssignableFrom(gargs[5])
													&& typeArgs[6].IsAssignableFrom(gargs[6])
													&& typeArgs[7].IsAssignableFrom(gargs[7])
													&& typeArgs[8].IsAssignableFrom(gargs[8])
												select ii).First();
				case 10: return (from ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef)
												 let gargs = ii.GetGenericArguments()
												 where typeArgs[0].IsAssignableFrom(gargs[0])
													 && typeArgs[1].IsAssignableFrom(gargs[2])
													 && typeArgs[2].IsAssignableFrom(gargs[2])
													 && typeArgs[3].IsAssignableFrom(gargs[3])
													 && typeArgs[4].IsAssignableFrom(gargs[4])
													 && typeArgs[5].IsAssignableFrom(gargs[5])
													 && typeArgs[6].IsAssignableFrom(gargs[6])
													 && typeArgs[7].IsAssignableFrom(gargs[7])
													 && typeArgs[8].IsAssignableFrom(gargs[8])
													 && typeArgs[9].IsAssignableFrom(gargs[9])
												 select ii).First();
				default:
					foreach (Type ii in t.DiscoverGenericInterfaceImplementations(genericTypeDef))
					{
						Type[] g = ii.GetGenericArguments();
						if (typeArgs[0].IsAssignableFrom(g[0])
									&& typeArgs[1].IsAssignableFrom(g[2])
									&& typeArgs[2].IsAssignableFrom(g[2])
									&& typeArgs[3].IsAssignableFrom(g[3])
									&& typeArgs[4].IsAssignableFrom(g[4])
									&& typeArgs[5].IsAssignableFrom(g[5])
									&& typeArgs[6].IsAssignableFrom(g[6])
									&& typeArgs[7].IsAssignableFrom(g[7])
									&& typeArgs[8].IsAssignableFrom(g[8])
									&& typeArgs[9].IsAssignableFrom(g[9]))
						{
							bool compatible = true;
							for (int i = 10; i < g.Length; i++)
							{
								if (!typeArgs[i].IsAssignableFrom(g[i]))
								{
									compatible = false;
									break;
								}
							}
							if (compatible) return ii;
						}
					}
					break;
			}
			return null;
		}

		/// <summary>
		/// Discovers the first instance of TAttribute that is defined for the present type.
		/// </summary>
		/// <typeparam name="TAttribute">Type of Attribute</typeparam>
		/// <param name="t">this Type.</param>
		/// <param name="inherit">Specifies whether to search the type's inheritance chain to find the attribute.</param>
		/// <returns>The first instance of TAttribute defined for the type or null if no such attribute is defined.</returns>
		public static TAttribute DiscoverFirstCustomAttribute<TAttribute>(this Type t, bool inherit)
			where TAttribute : Attribute
		{
			TAttribute result = null;
			if (t.IsDefined(typeof(TAttribute), inherit))
			{
				object[] attribs = t.GetCustomAttributes(typeof(TAttribute), inherit);
				result = attribs[0] as TAttribute;
			}
			return result;
		}

		/// <summary>
		/// For a given generic type definition, discovers implementations declared on the present type.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <param name="genericTypeDef">A generic type definition.</param>
		/// <returns>An enumerable of Type where each element is an implementation of the generic
		/// type definition defined for the present type.</returns>
		public static IEnumerable<Type> DiscoverGenericInterfaceImplementations(this Type t
			, Type genericTypeDef)
		{
			Contracts.Require.IsNotNull("genericTypeDef", genericTypeDef);
			Contracts.Require.IsGenericTypeDefinition(genericTypeDef);

			return from ii in t.GetInterfaces()
						 where ii.IsGenericType && Object.Equals(ii.GetGenericTypeDefinition(), genericTypeDef)
						 select ii;
		}

		/// <summary>
		/// Discovers a generic method definition defined for the present type by method name and binding flags.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <param name="methodName">The name of the generic method definition</param>
		/// <param name="flags">The set of binding flags that specifies how the search is conducted.</param>
		/// <returns>MethodInfo for a matching generic method definition if it is found; otherwise null.</returns>
		public static MethodInfo DiscoverGenericMethodDefinition(this Type t, string methodName, BindingFlags flags)
		{
			return (from m in t.GetMethods(flags)
							where String.Equals(methodName, m.Name, StringComparison.Ordinal)
							 && m.IsGenericMethodDefinition
							select m).SingleOrDefault();
		}

		/// <summary>
		/// Discovers a generic method definition defined for the present type by method name and binding flags
		/// where the method definition has the given number of generic arguments.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <param name="methodName">The name of the generic method definition</param>
		/// <param name="flags">The set of binding flags that specifies how the search is conducted.</param>
		/// <param name="genericArgumentCount">Number of generic arguments.</param>
		/// <returns>MethodInfo for a matching generic method definition if it is found; otherwise null.</returns>
		public static MethodInfo DiscoverGenericMethodDefinitionWithArgumentCount(this Type t, string methodName, BindingFlags flags
			, int genericArgumentCount)
		{
			return (from m in t.GetMethods(flags)
							where String.Equals(methodName, m.Name, StringComparison.Ordinal)
							 && m.IsGenericMethodDefinition
							 && m.GetGenericArguments().Count() == genericArgumentCount
							select m).SingleOrDefault();
		}

		/// <summary>
		/// Discovers all interfaces implemented by the present type that are assignable from a given interface.
		/// </summary>
		/// <typeparam name="TIntf">The interface used to determine assignability.</typeparam>
		/// <param name="targetType">The target type.</param>
		/// <returns>Interface types assignable from TIntf</returns>
		public static IEnumerable<Type> DiscoverInterfaceAssignableFromInterface<TIntf>(this Type t)
		{
			return from ii in t.GetInterfaces()
						 where typeof(TIntf).IsAssignableFrom(t)
						 select ii;
		}

		public static IEnumerable<MethodInfo> DiscoverMethodsWithAttribute<TAttribute>(this Type t, bool inherit)
		{
			return (from m in t.GetMethods()
							where m.IsDefined(typeof(TAttribute), inherit)
							select m);
		}

		public static IEnumerable<MethodInfo> DiscoverMethodsWithAttribute<TAttribute>(this Type t, BindingFlags binding, bool inherit)
		{
			return (from m in t.GetMethods(binding)
							where m.IsDefined(typeof(TAttribute), inherit)
							select m);
		}

		/// <summary>
		/// Collapses a set of interfaces into the most derived interfaces of the set.
		/// </summary>
		/// <param name="intfs">Interfaces to collapse.</param>
		/// <returns>The most derived interfaces in the set.</returns>
		public static IEnumerable<Type> DiscoverMostDerivedInterfacesByInheritance(this Type tthis)
		{
			var intfs = tthis.GetInterfaces();
			List<Type> copy = new List<Type>(intfs);

			foreach (Type i in intfs)
			{
				List<Type> copy2 = new List<Type>(copy);
				foreach (Type ii in copy)
				{
					if (i != ii)
					{
						if (ii.IsAssignableFrom(i))
							copy2.Remove(ii);
						else if (i.IsAssignableFrom(ii))
							copy2.Remove(i);
					}
				}
				copy = copy2;
			}

			return copy;
		}

		public static FieldInfo FindFirstFieldMatching(this Type t, BindingFlags flags, Func<FieldInfo, bool> match)
		{
			foreach (var f in t.GetFields(flags))
			{
				if (match(f))
					return f;
			}
			return null;
		}

		/// <summary>
		/// Gets a readable full name. Since this method uses reflection it should be used
		/// rarely. It was created to supply simpler type names when constructing error messages.
		/// </summary>
		/// <param name="t">The type.</param>
		/// <returns>A readable name such as My.Namespace.MyType&lt;string, int></returns>
		public static string GetReadableFullName(this Type t)
		{
			string result;
			Type tt = (t.IsArray) ? t.GetElementType() : t;
			string simpleName = tt.Name;
			if (simpleName.Contains('`'))
			{
				simpleName = simpleName.Substring(0, simpleName.IndexOf("`"));
				var args = tt.GetGenericArguments();
				for(int i = 0; i < args.Length; i++)
				{
					if (i == 0) simpleName = String.Concat(simpleName, '<', args[i].GetReadableSimpleName());
					else simpleName = String.Concat(simpleName, ',', args[i].GetReadableSimpleName());
				}
				simpleName = String.Concat(simpleName, '>');
			}
			if (tt.IsNested)
			{
				result = String.Concat(tt.DeclaringType.GetReadableFullName(), "+", simpleName);
			}
			else
			{
				result = String.Concat(tt.Namespace, ".", simpleName);
			}
			return result;
		}

		public static IEnumerable<PropertyInfo> GetReadableProperties(this Type t)
		{
			return from p in t.GetProperties()
						 where p.CanRead
						 select p;
		}

		public static IEnumerable<PropertyInfo> GetReadableProperties(this Type t, BindingFlags binding)
		{
			return from p in t.GetProperties(binding)
						 where p.CanRead
						 select p;
		}

		public static PropertyInfo GetReadableProperty(this Type t, string propertyName)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);

			PropertyInfo p = t.GetProperty(propertyName);
			return (p != null && p.CanRead) ? p : null;
		}

		public static PropertyInfo GetReadableProperty(this Type t, string propertyName, BindingFlags binding)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);

			return (from p in GetReadableProperties(t, binding)
							where p.Name == propertyName
							select p).FirstOrDefault();
		}

		public static PropertyInfo GetReadablePropertyWithAssignmentCompatablity(this Type t, string propertyName, BindingFlags binding, Type sourceType)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);

			return (from p in GetReadableProperties(t, binding)
							where p.Name == propertyName
								&& p.PropertyType.IsAssignableFrom(sourceType)
							select p).FirstOrDefault();
		}

		public static string GetReadableSimpleName(this Type t)
		{
			Type tt = (t.IsArray) ? t.GetElementType() : t;
			string simpleName = tt.Name;
			if (simpleName.Contains('`'))
			{
				simpleName = simpleName.Substring(0, simpleName.IndexOf("`"));
				var args = tt.GetGenericArguments();
				for (int i = 0; i < args.Length; i++)
				{
					if (i == 0) simpleName = String.Concat(simpleName, '<', args[i].GetReadableSimpleName());
					else simpleName = String.Concat(simpleName, ',', args[i].GetReadableSimpleName());
				}
				simpleName = String.Concat(simpleName, '>');
			}
			return simpleName;
		}

		/// <summary>
		/// Gets a "safe" object for use synchronizing access to a type's assembly.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <returns>An object that may be "safely" used to synchronize access to an assembly.</returns>
		/// <remarks>
		/// This object is the same object returned by t.Assembly.GetSafeLock();
		/// </remarks>
		public static Object GetSafeAssemblyLock(this Type t)
		{
			return t.Assembly.GetSafeLock();
		}

		/// <summary>
		/// Gets the simple assembly qualified name for a type. This is the full name without culture, version or strong-name.
		/// </summary>
		/// <param name="t">this Type</param>
		/// <returns>The simple assembly qualified name formatted as "{full-type-name},{simple-assembly-name}".</returns>
		public static string GetSimpleAssemblyQualifiedName(this Type t)
		{
			AssemblyName asm = t.Assembly.GetName();
			return String.Format("{0},{1}", t.FullName, asm.Name);
		}

		public static IEnumerable<PropertyInfo> GetWritableProperties(this Type t)
		{
			return from p in t.GetProperties()
						 where p.CanWrite && p.GetSetMethod() != null
						 select p;
		}

		public static IEnumerable<PropertyInfo> GetWritableProperties(this Type t, BindingFlags binding)
		{
			return from p in t.GetProperties(binding)
						 where p.CanWrite && p.GetSetMethod() != null
						 select p;
		}

		public static PropertyInfo GetWriteableProperty(this Type t, string propertyName)
		{
			Contracts.Require.IsNotNull("propertyName", propertyName);

			PropertyInfo p = t.GetProperty(propertyName);
			return (p != null && p.CanWrite && p.GetSetMethod() != null) ? p : null;
		}

		private static Type FindIEnumerable(Type seqType)
		{
			if (seqType == null || seqType == typeof(string))
				return null;
			if (seqType.IsArray)
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
			if (seqType.IsGenericType)
			{
				foreach (Type a in seqType.GetGenericArguments())
				{
					Type ienum = typeof(IEnumerable<>).MakeGenericType(a);
					if (ienum.IsAssignableFrom(seqType))
					{
						return ienum;
					}
				}
			}
			Type[] ifaces = seqType.GetInterfaces();
			if (ifaces != null && ifaces.Length > 0)
			{
				foreach (Type iface in ifaces)
				{
					Type ienum = FindIEnumerable(iface);
					if (ienum != null) return ienum;
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return FindIEnumerable(seqType.BaseType);
			}
			return null;
		}

		
	}
}