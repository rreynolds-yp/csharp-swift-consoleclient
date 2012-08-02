using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;

namespace ATTi.Core.Data
{
	public sealed class ExplicitDbAuthorization : IDisposable
	{
		[ThreadStatic]
		private static Stack<ExplicitDbAuthorization> __current;

		public static ExplicitDbAuthorization MakeExplicitAutorization(string authKeys)
		{
			if (authKeys == null) throw new ArgumentNullException("authKeys");
			ExplicitDbAuthorization result = new ExplicitDbAuthorization();

			string[] keys = authKeys.Split(',');
			foreach (string auth in keys)
			{
				if (!string.IsNullOrEmpty(auth))
				{
					new ExplicitDbPermission(auth).Demand();
					result.AddAuthorization(auth);
				}
			}
			return result;
		}
		public static void AssertAuthorization(string authKey)
		{
			if (__current != null)
			{
				foreach (ExplicitDbAuthorization auth in __current)
				{
					if (auth.HasExplicitAuthorization(authKey)) return;
				}
			}

			new ExplicitDbPermission(authKey).Demand();
		}

		List<string> _authorizations = new List<string>();

		private ExplicitDbAuthorization()
		{
			if (__current == null) __current = new Stack<ExplicitDbAuthorization>();
			__current.Push(this);
		}

		private void AddAuthorization(string auth)
		{
			if (auth == null) throw new ArgumentNullException("auth");
			new ExplicitDbPermission(auth).Demand();
			_authorizations.Add(auth);
		}

		private bool HasExplicitAuthorization(string auth)
		{
			return _authorizations.Contains(auth);
		}

		#region IDisposable Members

		~ExplicitDbAuthorization()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);			
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (__current.Peek() != this)
					throw new InvalidOperationException("Explicit authorizations may be nested but cannot overlap. Dispose in the reverse order of authorization.");
				__current.Pop();
				if (__current.Count == 0) __current = null;
			}
		}

		#endregion
	}

	/// <summary>
	/// CodeAccessPermission for operations requiring explicit permission.
	/// </summary>
	[Serializable]
	public sealed class ExplicitDbPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		#region Declarations

		private bool _isUnrestricted;
		private List<string> _keys;

		#endregion

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="state">PermissionState for the new instance.</param>
		public ExplicitDbPermission(PermissionState state)
		{
			_isUnrestricted = (state == PermissionState.Unrestricted);
			_keys = new List<string>();
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="state">PermissionState for the new instance.</param>
		public ExplicitDbPermission()
		{
			_isUnrestricted = false;
			_keys = new List<string>();
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ExplicitDbPermission(string key)
		{
			_isUnrestricted = false;
			_keys = new List<string>();
			if (key != null)
				_keys.Add(key);
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		private ExplicitDbPermission(IEnumerable<string> keys)
		{
			_isUnrestricted = false;
			_keys = new List<string>(keys);
		}

		#region IUnrestrictedPermission interface

		/// <summary>
		/// Indicates whether the permission is unrestricted.
		/// </summary>
		/// <returns><b>true</b> if the permission is unrestricted; otherwise <b>false</b>.</returns>
		public bool IsUnrestricted()
		{
			return _isUnrestricted;
		}

		#endregion

		#region IPermission interface (overrides)

		/// <summary>
		/// Makes a copy of the permission.
		/// </summary>
		/// <returns>A new permission instance that is a copy of the instance on 
		/// which Copy is called.</returns>
		public override IPermission Copy()
		{
			if (_isUnrestricted)
				return new ExplicitDbPermission(PermissionState.Unrestricted);
			else
				return new ExplicitDbPermission(_keys);
		}

		/// <summary>
		/// Produces a permission that is a intersection (combined permission) of 
		/// the permission on which Intersect is called and the target permission.
		/// </summary>
		/// <param name="target">Permission to be combined.</param>
		/// <returns>A permission that is an intersection of the target
		/// and the instance on which Intersect is called.</returns>
		public override IPermission Intersect(IPermission target)
		{
			ExplicitDbPermission result = null;
			if (target != null)
			{
				if (target is ExplicitDbPermission)
				{
					ExplicitDbPermission other = (ExplicitDbPermission)target;
					if (this._isUnrestricted && other._isUnrestricted)
						result = new ExplicitDbPermission(PermissionState.Unrestricted);
					else
					{
						result = new ExplicitDbPermission();
						result.PerformIntersection(other._keys);
					}
				}
				else
					throw new ArgumentException("target");
			}
			return result;
		}

		private void PerformIntersection(List<string> list)
		{
			this._keys = new List<string>(this._keys.Intersect(list));
		}

		/// <summary>
		/// Produces a permission that is a union (common permission set) of the 
		/// permission on which Union is called and the target permission.
		/// </summary>
		/// <param name="other">Permission to union.</param>
		/// <returns>A permission that is a union of the target and the
		/// instance on which Union is called.</returns>
		public override IPermission Union(IPermission target)
		{
			ExplicitDbPermission result = null;
			if (target != null)
			{
				if (target is ExplicitDbPermission)
				{
					ExplicitDbPermission other = (ExplicitDbPermission)target;
					if (this._isUnrestricted || other._isUnrestricted)
						result = new ExplicitDbPermission(PermissionState.Unrestricted);
					else
					{
						result = new ExplicitDbPermission();
						result.PerformUnion(other._keys);
					}
				}
				else
					throw new ArgumentException("target");
			}
			return result;
		}

		private void PerformUnion(List<string> list)
		{
			this._keys = new List<string>(this._keys.Union(list));
		}
		
		/// <summary>
		/// Determines if the permission of the instance on which IsSubsetOf
		/// is called is a subset of the permissions of the target.
		/// </summary>
		/// <param name="target">The target instance (potential superset).</param>
		/// <returns><b>true</b> if the instance on which IsSubsetOf is called is
		/// a subset of the target permission; otherwise <b>false</b>.</returns>
		public override bool IsSubsetOf(IPermission target)
		{
			bool result = false;
			if (target != null)
			{
				if (target is ExplicitDbPermission)
				{
					bool subset = true;
					ExplicitDbPermission other = (ExplicitDbPermission)target;
					foreach (string s in other._keys)
					{
						if (!other._keys.Contains(s))
						{
							subset = false;
							break;
						}
					}
					result = subset;
				}
				else
					throw new ArgumentException("target");
			}
			return result;
		}

		#endregion

		#region ISecurityEncodable interface (overrides)

		/// <summary>
		/// Captures a SecurityElement containing the XML representation
		/// of the permission.
		/// </summary>
		/// <returns>A SecurityElement.</returns>
		public override SecurityElement ToXml()
		{
			SecurityElement result = new SecurityElement("IPermission");
			result.AddAttribute("class", base.GetType().FullName + ", " + base.GetType().Module.Assembly.FullName.Replace('"', '\''));
			result.AddAttribute("version", "1");
			if (!this.IsUnrestricted())
			{
				SecurityElement accessKeys = new SecurityElement("AccessKeys");
				foreach (string key in this._keys)
				{
					SecurityElement keyelm = new SecurityElement("Key");
					keyelm.AddAttribute("name", SecurityElement.Escape(key));
					accessKeys.AddChild(keyelm);
				}
				result.AddChild(accessKeys);
			}
			else
				result.AddAttribute("Unrestricted", "true");
			return result;
		}

		/// <summary>
		/// Restores the internal state of the permission to reflect
		/// a previously captured permission described in the SecurityElement.
		/// </summary>
		/// <param name="elem">Security element containing an XML representation
		/// of the permission.</param>
		public override void FromXml(SecurityElement elem)
		{
			if (elem == null)
				throw new ArgumentNullException("elem");

			if (!elem.Tag.Equals("IPermission"))
			{
				throw new ArgumentException("Missing IPermission element", "elem");
			}
			string cls = elem.Attribute("class");
			if (cls == null)
			{
				throw new ArgumentException("Missing class element", "elem");
			}
			if (cls.IndexOf(base.GetType().FullName) < 0)
			{
				throw new ArgumentException("Missing type name element", "elem");
			}
			string unre = elem.Attribute("Unrestricted");
			this._keys = new List<string>();
			if ((unre != null) && (string.Compare(unre, "true", StringComparison.OrdinalIgnoreCase) == 0))
			{
				this._isUnrestricted = true;
			}
			else
			{
				string name;
				this._isUnrestricted = false;
				SecurityElement keys = elem.SearchForChildByTag("AccessKeys");
				if (keys != null)
				{
					foreach (SecurityElement key in keys.Children)
					{
						if (!key.Tag.Equals("Key"))
						{
							continue;
						}
						else
						{
							try
							{
								name = key.Attribute("name");
							}
							catch
							{
								name = null;
							}
							if (name == null)
								throw new ArgumentException("Invalid key name", "elem");
							else
								_keys.Add(name);
						}
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Attribute for declaratively applying the ExplicitDbPermission.
	/// </summary>
	[Serializable, AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
	public class ExplicitDbPermissionAttribute : CodeAccessSecurityAttribute
	{
		#region Declarations
		string _securityKey = null;
		#endregion

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="action"></param>
		public ExplicitDbPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		/// <summary>
		/// Gets or sets the explicit authorization key used by the object owner
		/// to assert explicit authorization as the owner.
		/// </summary>
		public string ExplicitAuthorizationKey
		{
			get { return _securityKey; }
			set { _securityKey = value; }
		}


		/// <summary>
		/// Factory method; constructs an instance of the permission initialized
		/// with the properties of the attribute.
		/// </summary>
		/// <returns>A TriggeredActionPermission reflecting the declaration.</returns>
		public override IPermission CreatePermission()
		{
			if (this.Unrestricted)
			{
				return new ExplicitDbPermission(PermissionState.Unrestricted);
			}
			else
			{
				return new ExplicitDbPermission(_securityKey);
			}
		}
	}
}
