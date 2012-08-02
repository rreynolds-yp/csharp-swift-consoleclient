using System;
using System.Collections.Generic;
using ATTi.Core.Mementos;

namespace ATTi.Core.Mementos.Helpers
{
	public class DictionaryMementoHelper<TKey, TValue> : IMementoHelper<Dictionary<TKey, TValue>>
	{		
		[Flags]
		enum TypeArgumentClassification
		{
			ObjectObject = 0,
			KNative = 1,
			VNative = 2
		}
		interface ISelfRestore
		{
			void RestoreAll(IMementoContext ctx, ref Dictionary<TKey, TValue> item);
		}

		struct DictionaryMemento_NativeKeyNativeValue : IMemento, ISelfRestore
		{
			public Dictionary<TKey, TValue> _target;
			public List<System.Collections.Generic.KeyValuePair<TKey, TValue>> Values;
			bool _isRestored;

			public DictionaryMemento_NativeKeyNativeValue(Dictionary<TKey, TValue> target)
			{
				_target = target;
				_isRestored = false;
				if (_target.Count > 0)
				{
					Values = new List<System.Collections.Generic.KeyValuePair<TKey, TValue>>(_target.Count);
					foreach (var kvp in target)
					{
						Values.Add(kvp);
					}
				}
				else Values = null;
			}

			object IMemento.Target { get { return _target; } }
			bool IMemento.IsRestored { get { return _isRestored; } set { _isRestored = value; } }

			void ISelfRestore.RestoreAll(IMementoContext ctx, ref Dictionary<TKey, TValue> item)
			{
				item.Clear();
				if (Values != null)
				{
					foreach (var kvp in Values)
					{
						item.Add(kvp.Key, kvp.Value);
					}
				}
			}
		}

		struct DictionaryMemento_NativeKeyObjectValue : IMemento, ISelfRestore
		{
			public Dictionary<TKey, TValue> _target;
			public List<KeyValuePair<TKey, IMemento>> Values;
			bool _isRestored;

			public DictionaryMemento_NativeKeyObjectValue(IMementoContext ctx, Dictionary<TKey, TValue> target)
			{
				_target = target;
				_isRestored = false;
				if (_target.Count > 0)
				{
					Values = new List<KeyValuePair<TKey, IMemento>>(_target.Count);
					foreach (var kvp in target)
					{
						Values.Add(new KeyValuePair<TKey, IMemento>(kvp.Key
							, Memento.CaptureMemento(ctx, kvp.Value)));
					}
				}
				else Values = null;
			}

			object IMemento.Target { get { return _target; } }
			bool IMemento.IsRestored { get { return _isRestored; } set { _isRestored = value; } }

			void ISelfRestore.RestoreAll(IMementoContext ctx, ref Dictionary<TKey, TValue> item)
			{
				item.Clear();
				if (Values != null)
				{
					foreach (var kvp in Values)
					{
						TValue v = (TValue)kvp.Value.Target;
						Memento.RestoreMemento(ctx, ref v, kvp.Value);
						item.Add(kvp.Key, v);
					}
				}
			}
		}

		struct DictionaryMemento_ObjectKeyNativeValue : IMemento, ISelfRestore
		{
			public Dictionary<TKey, TValue> _target;
			public List<KeyValuePair<IMemento, TValue>> Values;
			bool _isRestored;

			public DictionaryMemento_ObjectKeyNativeValue(IMementoContext ctx, Dictionary<TKey, TValue> target)
			{
				_target = target;
				_isRestored = false;
				if (_target.Count > 0)
				{
					Values = new List<KeyValuePair<IMemento, TValue>>(_target.Count);
					foreach (var kvp in target)
					{
						Values.Add(new KeyValuePair<IMemento, TValue>(
							Memento.CaptureMemento(ctx, kvp.Key)
							, kvp.Value));
					}
				}
				else Values = null;
			}

			object IMemento.Target { get { return _target; } }
			bool IMemento.IsRestored { get { return _isRestored; } set { _isRestored = value; } }

			void ISelfRestore.RestoreAll(IMementoContext ctx, ref Dictionary<TKey, TValue> item)
			{
				item.Clear();
				if (Values != null)
				{
					foreach (var kvp in Values)
					{
						TKey k = (TKey)kvp.Key.Target;
						Memento.RestoreMemento(ctx, ref k, kvp.Key);
						item.Add(k, kvp.Value);
					}
				}
			}
		}

		struct DictionaryMemento_ObjectKeyObjectValue : IMemento, ISelfRestore
		{
			public Dictionary<TKey, TValue> _target;
			public List<KeyValuePair<IMemento, IMemento>> Values;
			bool _isRestored;

			public DictionaryMemento_ObjectKeyObjectValue(IMementoContext ctx, Dictionary<TKey, TValue> target)
			{
				_target = target;
				_isRestored = false;
				if (_target.Count > 0)
				{
					Values = new List<KeyValuePair<IMemento, IMemento>>(_target.Count);
					foreach (var kvp in target)
					{
						Values.Add(new KeyValuePair<IMemento, IMemento>(
							Memento.CaptureMemento(ctx, kvp.Key)
							, Memento.CaptureMemento(ctx, kvp.Value)));
					}
				}
				else Values = null;
			}

			object IMemento.Target { get { return _target; } }
			bool IMemento.IsRestored { get { return _isRestored; } set { _isRestored = value; } }

			void ISelfRestore.RestoreAll(IMementoContext ctx, ref Dictionary<TKey, TValue> item)
			{
				item.Clear();
				if (Values != null)
				{
					foreach (var kvp in Values)
					{
						TKey k = (TKey)kvp.Key.Target;
						Memento.RestoreMemento(ctx, ref k, kvp.Key);
						TValue v = (TValue)kvp.Value.Target;
						Memento.RestoreMemento(ctx, ref v, kvp.Value);
						item.Add(k, v);
					}
				}
			}
		}

		static TypeArgumentClassification __classification;

		static DictionaryMementoHelper()
		{
			TypeCode kcode = Type.GetTypeCode(typeof(TKey));
			if (kcode != TypeCode.Object) __classification = TypeArgumentClassification.KNative;
			TypeCode vnative = Type.GetTypeCode(typeof(TValue));
			if (vnative != TypeCode.Object) __classification |= TypeArgumentClassification.VNative;
		}

		#region IMementoHelper<Dictionary<TKey,TValue>> Members

		public IMemento CaptureMemento(IMementoContext ctx, Dictionary<TKey, TValue> item)
		{
			if ((__classification & TypeArgumentClassification.KNative) == TypeArgumentClassification.KNative)
			{
				if ((__classification & TypeArgumentClassification.VNative) == TypeArgumentClassification.VNative)
				{
					return new DictionaryMemento_NativeKeyNativeValue(item);
				}
				else
				{
					return new DictionaryMemento_NativeKeyObjectValue(ctx, item);
				}
			}
			else
			{
				if ((__classification & TypeArgumentClassification.VNative) == TypeArgumentClassification.VNative)
				{
					return new DictionaryMemento_ObjectKeyNativeValue(ctx, item);
				}
				else
				{
					return new DictionaryMemento_ObjectKeyObjectValue(ctx, item);
				}
			}
		}

		public void RestoreMemento(IMementoContext ctx, ref Dictionary<TKey, TValue> item, IMemento m)
		{
			ISelfRestore r = (ISelfRestore)m;
			r.RestoreAll(ctx, ref item);
		}

		#endregion
	}
}
