namespace ATTi.Core.Reflection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Security.Permissions;
	using System.Text;

	public abstract class MemberInfoEx : IComparable
	{
		

		private readonly MemberInfo _memberInfo;
		private readonly int _offset;

		

		#region Constructors

		protected MemberInfoEx(MemberInfo mbr, int offset)
		{
			this._memberInfo = mbr;
			this._offset = offset;
		}

		#endregion Constructors

		

		public MemberInfo MemberInfo
		{
			get { return _memberInfo; }
		}

		public int Offset
		{
			get { return _offset; }
		}

		

		

		public int CompareTo(object other)
		{
			MemberInfoEx ex = other as MemberInfoEx;
			if (ex == null)
			{
				return -1;
			}
			return this._offset.CompareTo(ex._offset);
		}

		[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
		internal object GetValue(object obj)
		{
			return DoGetValue(_memberInfo, obj, null);
		}

		[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
		internal object GetValue(object obj, object[] index)
		{
			return DoGetValue(_memberInfo, obj, index);
		}

		[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
		internal void SetValue(object recvr, object value)
		{
			DoSetValue(_memberInfo, recvr, value, null);
		}

		protected abstract object DoGetValue(MemberInfo mbr, object obj, object[] index);

		protected abstract void DoSetValue(MemberInfo mbr, object recvr, object value, object[] index);

		[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
		protected void SetValue(object recvr, object value, object[] index)
		{
			DoSetValue(_memberInfo, recvr, value, index);
		}

		
	}

	internal sealed class FieldMemberEx : MemberInfoEx
	{
		#region Constructors

		internal FieldMemberEx(MemberInfo mbr, int offset)
			: base(mbr, offset)
		{
		}

		#endregion Constructors

		

		protected override object DoGetValue(MemberInfo mbr, object obj, object[] index)
		{
			return ((FieldInfo)mbr).GetValue(obj);
		}

		protected override void DoSetValue(MemberInfo mbr, object recvr, object value, object[] index)
		{
			((FieldInfo)mbr).SetValue(recvr, value);
		}

		
	}

	internal sealed class PropertyMemberEx : MemberInfoEx
	{
		#region Constructors

		internal PropertyMemberEx(MemberInfo mbr, int offset)
			: base(mbr, offset)
		{
		}

		#endregion Constructors

		

		protected override object DoGetValue(MemberInfo mbr, object obj, object[] index)
		{
			return ((PropertyInfo)mbr).GetValue(obj, index);
		}

		protected override void DoSetValue(MemberInfo mbr, object recvr, object value, object[] index)
		{
			((PropertyInfo)mbr).SetValue(recvr, value, index);
		}

		
	}
}