using System;
using System.Reflection;
using System.Reflection.Emit;


namespace ATTi.Core.Reflection.Emit
{
	/// <summary>
	/// Helper class for working with IL.
	/// </summary>
	public class ILHelper
	{
		ILGenerator _gen;
		public ILHelper(ILGenerator gen)
		{
			Contracts.Require.IsNotNull("gen", gen);

			_gen = gen;
		}
		public ILGenerator Generator { get { return _gen; } }

		public void BeginScope()
		{
			_gen.BeginScope();
		}
		public void EndScope()
		{
			_gen.EndScope();
		}

		/// <summary>
		/// Adds two values on the stack and pushes the result onto the stack.
		/// </summary>
		public void Add()
		{
			_gen.Emit(OpCodes.Add);
		}
		/// <summary>
		/// Adds two integers on the stack, performs an overflow check and pushes the result onto the stack.
		/// </summary>
		public void AddWithOverflowCheck()
		{
			_gen.Emit(OpCodes.Add_Ovf);
		}
		/// <summary>
		/// Adds two unsigned integers on the stack, performs an overflow check and pushes the result onto the stack.
		/// </summary>		
		public void AddUnsignedWithOverflowCheck()
		{
			_gen.Emit(OpCodes.Add_Ovf_Un);
		}
		/// <summary>
		/// Computes the bitwise AND of two values on the stack and pushes the result onto the stack.
		/// </summary>
		public void BitwiseAnd()
		{
			_gen.Emit(OpCodes.And);
		}
		/// <summary>
		/// Pushes an unmanaged pointer to the argument list of the current method onto the stack.
		/// </summary>
		public void ArgListPointer()
		{
			_gen.Emit(OpCodes.Arglist);
		}
		/// <summary>
		/// Transfers control to a target label if two values are equal.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfEqual(Label label)
		{
			
			_gen.Emit(OpCodes.Beq, label);
		}
		/// <summary>
		/// Transfers control to a target label if two values are equal.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfEqual_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Beq_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than or equal to the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThanOrEqual(Label label)
		{
			
			_gen.Emit(OpCodes.Bge, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than or equal to the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThanOrEqual_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Bge_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than or equal to the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThanOrEqual_Unsigned(Label label)
		{
			
			_gen.Emit(OpCodes.Bge_Un, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than or equal to the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThanOrEqual_Unsigned_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Bge_Un_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThan(Label label)
		{
			
			_gen.Emit(OpCodes.Bgt, label);
		}
		/// <summary>
		/// Transfers control to a target label the first value on the stack 
		/// is greater than the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThan_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Bgt_S, label);
		}
		/// <summary>
		/// Transfers control to a target label the first value on the stack 
		/// is greater than the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThan_Unsigned(Label label)
		{
			
			_gen.Emit(OpCodes.Bgt_Un, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is greater than the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfGreaterThan_Unsigned_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Bgt_Un_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than or equal to the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThanOrEqual(Label label)
		{
			
			_gen.Emit(OpCodes.Ble, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than or equal to the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThanOrEqual_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Ble_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than or equal to the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThanOrEqual_Unsigned(Label label)
		{
			
			_gen.Emit(OpCodes.Ble_Un, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than or equal to the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThanOrEqual_Unsigned_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Ble_Un_S, label);
		}
		/// <summary>
		/// Transfers control to a target label the first value on the stack 
		/// is less than the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThan(Label label)
		{
			
			_gen.Emit(OpCodes.Blt, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than the second value on the stack.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThan_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Blt_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThan_Unsigned(Label label)
		{
			
			_gen.Emit(OpCodes.Blt_Un, label);
		}
		/// <summary>
		/// Transfers control to a target label if the first value on the stack 
		/// is less than the second value on the stack when
		/// comparing unsigned integer values or unordered float values.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfLessThan_Unsigned_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Blt_Un_S, label);
		}
		/// <summary>
		/// Converts a ValueType to an object reference.
		/// </summary>
		public void Box(Type valueTyp)
		{
			Contracts.Require.IsNotNull("valueTyp", valueTyp);
			_gen.Emit(OpCodes.Box, valueTyp);
		}
		/// <summary>
		/// Transfers control to a target label.
		/// </summary>
		/// <param name="label"></param>
		public void Branch(Label label)
		{
			
			_gen.Emit(OpCodes.Br, label);
		}
		/// <summary>
		/// Transfers control to a target label.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void Branch_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Br_S, label);
		}
		/// <summary>
		/// Signals the CLI to inform the debugger that a breakpoint has been tripped.
		/// </summary>
		public void Break()
		{
			_gen.Emit(OpCodes.Break);
		}
		/// <summary>
		/// Transfers control to a target label if the value on the stack is false, null, or zero.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfFalse(Label label)
		{
			
			_gen.Emit(OpCodes.Brfalse, label);
		}
		/// <summary>
		/// Transfers control to a target label if the value on the stack is false, null, or zero.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfFalse_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Brfalse_S, label);
		}
		/// <summary>
		/// Transfers control to a target label if the value on the stack is true, not null, or non zero.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfTrue(Label label)
		{
			
			_gen.Emit(OpCodes.Brtrue, label);
		}
		/// <summary>
		/// Transfers control to a target label if the value on the stack is true, not null, or non zero.
		/// </summary>
		/// <param name="label">A target label.</param>
		public void BranchIfTrue_ShortForm(Label label)
		{
			
			_gen.Emit(OpCodes.Brtrue_S, label);
		}
		/// <summary>
		/// Calls the method indicated by the method descriptor.
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		public void Call(MethodInfo method)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCall(OpCodes.Call, method, null);
		}
		/// <summary>
		/// Calls the constructor indicated by the constructor descriptor.
		/// </summary>
		/// <param name="ctor">ConstructorInfo for the constructor to call.</param>
		public void Call(ConstructorInfo ctor)
		{
			Contracts.Require.IsNotNull("ctor", ctor);

			_gen.Emit(OpCodes.Call, ctor);
		}
		/// <summary>
		/// Calls the varargs method indicated by the method descriptor.
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		/// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method; otherwise null.</param>
		public void CallVarArgs(MethodInfo method, Type[] optionalParameterTypes)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCall(OpCodes.Call, method, optionalParameterTypes);
		}
		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point).
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		/// <param name="callingConventions">The managed calling conventions to be used.</param>
		/// <param name="returnType">The return type of the method if it returns a result; otherwise null.</param>
		/// <param name="parameterTypes">The types of parameters for the call.</param>
		/// <param name="optionalParameterTypes">The types of optional parameters for the call if the method accepts optional parameters; otherwise null.</param>
		public void CallIndirectManaged(MethodInfo method, CallingConventions callingConventions, Type returnType
			, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCalli(OpCodes.Calli, callingConventions, returnType, parameterTypes, optionalParameterTypes);
		}
		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point).
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		/// <param name="callingConventions">The unmanaged calling conventions to be used.</param>
		/// <param name="returnType">The return type of the method if it returns a result; otherwise null.</param>
		/// <param name="parameterTypes">The types of parameters for the call.</param>
		public void CallIndirectUnanaged(MethodInfo method, System.Runtime.InteropServices.CallingConvention callingConventions, Type returnType
			, Type[] parameterTypes)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCalli(OpCodes.Calli, callingConventions, returnType, parameterTypes);
		}
		/// <summary>
		/// Calls a late-bound method on an object, pushing the result object onto the stack.
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		public void CallVirtual(MethodInfo method)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCall(OpCodes.Callvirt, method, null);
		}
		/// <summary>
		/// Calls a late-bound method on an object, pushing the result object onto the stack.
		/// </summary>
		/// <param name="method">MethodInfo for the method to call.</param>
		/// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method; otherwise null.</param>
		public void CallVarArgsVirtual(MethodInfo method, Type[] optionalParameterTypes)
		{
			Contracts.Require.IsNotNull("method", method);

			_gen.EmitCall(OpCodes.Callvirt, method, optionalParameterTypes);
		}
		public void CastClass(Type targetType)
		{
			Contracts.Require.IsNotNull("targetType", targetType);

			_gen.Emit(OpCodes.Castclass, targetType);
		}
		/// <summary>
		/// Compares two values on the stack and if they are equal, the integer value 1 is placed on the stack; otherwise the value 0 is placed on the stack.
		/// </summary>
		public void CompareEqual()
		{
			_gen.Emit(OpCodes.Ceq);
		}
		/// <summary>
		/// Compares two values on the stack and if the first value is greater than the second, the integer value 1 is placed on the stack; otherwise the value 0 is placed on the stack.
		/// </summary>
		public void CompareGreaterThan()
		{
			_gen.Emit(OpCodes.Cgt);
		}
		/// <summary>
		/// Compares two values on the stack and if the first value is greater than the second, the integer value 1 is placed on the stack; otherwise the value 0 is placed on the stack.
		/// </summary>
		public void CompareGreaterThan_Unsigned()
		{
			_gen.Emit(OpCodes.Cgt_Un);
		}
		/// <summary>
		/// Compares two values on the stack and if the first value is less than the second, the integer value 1 is placed on the stack; otherwise the value 0 is placed on the stack.
		/// </summary>
		public void CompareLessThan()
		{
			_gen.Emit(OpCodes.Clt);
		}
		/// <summary>
		/// Compares two values on the stack and if the first value is less than the second, the integer value 1 is placed on the stack; otherwise the value 0 is placed on the stack.
		/// </summary>
		public void CompareLessThan_Unsigned()
		{
			_gen.Emit(OpCodes.Clt_Un);
		}
		/// <summary>
		/// Throws a System.ArithmeticException if the value on the stack is not a finite number.
		/// </summary>
		public void CheckFinite()
		{
			_gen.Emit(OpCodes.Ckfinite);
		}
		public void Constrained(Type t)
		{
			_gen.Emit(OpCodes.Constrained);
		}
		/// <summary>
		/// Converts the value on the top of the stack to a natural int.
		/// </summary>
		public void ConvertToNaturalInt()
		{
			_gen.Emit(OpCodes.Conv_I);
		}
		/// <summary>
		/// Converts the value on the top of the stack to a unsigned natural int.
		/// </summary>
		public void ConvertToUnsignedNaturalInt()
		{
			_gen.Emit(OpCodes.Conv_U);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int8 and pads it to an int.
		/// </summary>
		public void ConvertToInt8()
		{
			_gen.Emit(OpCodes.Conv_I1);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an unsigned int8 and pads it to an int.
		/// </summary>
		public void ConvertToUInt8()
		{
			_gen.Emit(OpCodes.Conv_U1);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int16 and pads it to an int.
		/// </summary>
		public void ConvertToInt16()
		{
			_gen.Emit(OpCodes.Conv_I2);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an unsigned int16 and pads it to an int.
		/// </summary>
		public void ConvertToUInt16()
		{
			_gen.Emit(OpCodes.Conv_U2);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int32.
		/// </summary>
		public void ConvertToInt32()
		{
			_gen.Emit(OpCodes.Conv_I4);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an unsigned int32.
		/// </summary>
		public void ConvertToUInt32()
		{
			_gen.Emit(OpCodes.Conv_U4);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int64.
		/// </summary>
		public void ConvertToInt64()
		{
			_gen.Emit(OpCodes.Conv_I8);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an unsigned int64.
		/// </summary>
		public void ConvertToUInt64()
		{
			_gen.Emit(OpCodes.Conv_U8);
		}
		/// <summary>
		/// Converts the value on the top of the stack to a natural int.
		/// </summary>
		public void ConvertToNaturalIntWithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I);
		}
		/// <summary>
		/// Converts the signed value on the top of the stack to a natural unsigned int.
		/// </summary>
		public void ConvertToUnsignedNaturalIntWithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to a natural int.
		/// </summary>
		public void ConvertToNaturalIntWithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an unsigned natural int.
		/// </summary>
		public void ConvertToUnsignedNaturalIntWithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U_Un);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int8 and pads it to an int.
		/// </summary>
		public void ConvertToInt8WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I1);
		}
		/// <summary>
		/// Converts the signed value on the top of the stack to a unsigned int8 and pads it to an int32.
		/// </summary>
		public void ConvertToUnsignedInt8WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U1);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an int8 and pads it to an int.
		/// </summary>
		public void ConvertToInt8WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I1_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an unsigned int8 and pads it to int32.
		/// </summary>
		public void ConvertToUnsignedInt8WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U1_Un);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int16 and pads it to an int.
		/// </summary>
		public void ConvertToInt16WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I2);
		}
		/// <summary>
		/// Converts the signed value on the top of the stack to a unsigned int16 and pads it to an int32.
		/// </summary>
		public void ConvertToUnsignedInt16WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U2);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an int16 and pads it to an int.
		/// </summary>
		public void ConvertToInt16WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I2_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an unsigned int16 and pads it to int32.
		/// </summary>
		public void ConvertToUnsignedInt16WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U2_Un);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int32.
		/// </summary>
		public void ConvertToInt32WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I4);
		}
		/// <summary>
		/// Converts the signed value on the top of the stack to a unsigned int32.
		/// </summary>
		public void ConvertToUnsignedInt32WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U4);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an int32.
		/// </summary>
		public void ConvertToInt32WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I4_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an unsigned int32.
		/// </summary>
		public void ConvertToUnsignedInt32WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U4_Un);
		}
		/// <summary>
		/// Converts the value on the top of the stack to an int64.
		/// </summary>
		public void ConvertToInt64WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I8);
		}
		/// <summary>
		/// Converts the signed value on the top of the stack to a unsigned int64.
		/// </summary>
		public void ConvertToUnsignedInt64WithOverflow()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U8);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an int64.
		/// </summary>
		public void ConvertToInt64WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_I8_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to an unsigned int64.
		/// </summary>
		public void ConvertToUnsignedInt64WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_Ovf_U8_Un);
		}
		/// <summary>
		/// Converts the unsigned value on the top of the stack to a float32.
		/// </summary>
		public void ConvertToFloat32WithOverflow_Unsigned()
		{
			_gen.Emit(OpCodes.Conv_R_Un);
		}
		/// <summary>
		/// Converts the value on the top of the stack to a float32.
		/// </summary>
		public void ConvertToFloat32()
		{
			_gen.Emit(OpCodes.Conv_R4);
		}
		/// <summary>
		/// Converts the value on the top of the stack to a float64.
		/// </summary>
		public void ConvertToFloat64()
		{
			_gen.Emit(OpCodes.Conv_R8);
		}
		/// <summary>
		/// Copies a specified number of bytes from a source address to a destination address.
		/// </summary>
		public void CopyBlock()
		{
			_gen.Emit(OpCodes.Cpblk);
		}
		/// <summary>
		/// Copies the value type located at an address to another address (type &, *, or natural int).
		/// </summary>
		public void CopyObject()
		{
			_gen.Emit(OpCodes.Cpobj);
		}
		/// <summary>
		/// Divides two values and pushes the result as a floating-point or quotient onto the stack.
		/// </summary>
		public void Divide()
		{
			_gen.Emit(OpCodes.Div);
		}
		/// <summary>
		/// Divides two unsigned integer values and pushes the result (int32) onto the stack.
		/// </summary>
		public void Divide_Unsigned()
		{
			_gen.Emit(OpCodes.Div_Un);
		}
		/// <summary>
		/// Copies the topmost value on the evaluation stack and pushes the copy onto the stack.
		/// </summary>
		public void Duplicate()
		{
			_gen.Emit(OpCodes.Dup);
		}
		/// <summary>
		/// Transfers control back from the filter clause of an exception block back to the CLI exception handler.
		/// </summary>
		public void EndFilter()
		{
			_gen.Emit(OpCodes.Endfilter);
		}
		/// <summary>
		/// Transfers control back from the fault or finally clause of an exception block back to the CLI exception handler.
		/// </summary>
		public void EndFinally()
		{
			_gen.Emit(OpCodes.Endfinally);
		}

		public void EndInitBlock()
		{
			_gen.Emit(OpCodes.Initblk);
		}

		public void LoadArg(int a)
		{
			switch (a)
			{
				case 0: _gen.Emit(OpCodes.Ldarg_0); break;
				case 1: _gen.Emit(OpCodes.Ldarg_1); break;
				case 2: _gen.Emit(OpCodes.Ldarg_2); break;
				case 3: _gen.Emit(OpCodes.Ldarg_3); break;
				default: _gen.Emit(OpCodes.Ldarg, a); break;
			}
		}
		public void LoadArg_0() { _gen.Emit(OpCodes.Ldarg_0); }
		public void LoadArg_1() { _gen.Emit(OpCodes.Ldarg_1); }
		public void LoadArg_2() { _gen.Emit(OpCodes.Ldarg_2); }
		public void LoadArg_3() { _gen.Emit(OpCodes.Ldarg_3); }
		public void LoadArg_ShortForm(short a) { _gen.Emit(OpCodes.Ldarg_S, a); }

		public void LoadField(FieldInfo field)
		{
			if (field.IsStatic) _gen.Emit(OpCodes.Ldsfld, field);
			else _gen.Emit(OpCodes.Ldfld, field);
		}
		public void LoadFieldAddress(FieldInfo field)
		{
			if (field.IsStatic) _gen.Emit(OpCodes.Ldsflda, field);
			else _gen.Emit(OpCodes.Ldflda, field);
		}

		public void Load_I4(int a)
		{
			switch (a)
			{
				case -1: _gen.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: _gen.Emit(OpCodes.Ldc_I4_0); break;
				case 1: _gen.Emit(OpCodes.Ldc_I4_1); break;
				case 2: _gen.Emit(OpCodes.Ldc_I4_2); break;
				case 3: _gen.Emit(OpCodes.Ldc_I4_3); break;
				case 4: _gen.Emit(OpCodes.Ldc_I4_4); break;
				case 5: _gen.Emit(OpCodes.Ldc_I4_5); break;
				case 6: _gen.Emit(OpCodes.Ldc_I4_6); break;
				case 7: _gen.Emit(OpCodes.Ldc_I4_7); break;
				case 8: _gen.Emit(OpCodes.Ldc_I4_8); break;
				default: _gen.Emit(OpCodes.Ldc_I4, a); break;
			}
		}
		public void Load_I4_M1() { _gen.Emit(OpCodes.Ldc_I4_M1); }
		public void Load_I4_0() { _gen.Emit(OpCodes.Ldc_I4_0); }
		public void Load_I4_1() { _gen.Emit(OpCodes.Ldc_I4_1); }
		public void Load_I4_2() { _gen.Emit(OpCodes.Ldc_I4_2); }
		public void Load_I4_3() { _gen.Emit(OpCodes.Ldc_I4_3); }
		public void Load_I4_4() { _gen.Emit(OpCodes.Ldc_I4_4); }
		public void Load_I4_5() { _gen.Emit(OpCodes.Ldc_I4_5); }
		public void Load_I4_6() { _gen.Emit(OpCodes.Ldc_I4_6); }
		public void Load_I4_7() { _gen.Emit(OpCodes.Ldc_I4_7); }
		public void Load_I4_8() { _gen.Emit(OpCodes.Ldc_I4_8); }
		public void Load_I4_ShortForm(short a) { _gen.Emit(OpCodes.Ldc_I4_S, a); }

		public void LoadLocal(LocalBuilder lcl)
		{
			Contracts.Require.IsNotNull("lcl", lcl);

			LoadLocal(lcl.LocalIndex);
		}
		public void LoadLocal(int lcl)
		{
			switch (lcl)
			{
				case 0: _gen.Emit(OpCodes.Ldloc_0); break;
				case 1: _gen.Emit(OpCodes.Ldloc_1); break;
				case 2: _gen.Emit(OpCodes.Ldloc_2); break;
				case 3: _gen.Emit(OpCodes.Ldloc_3); break;
				default: _gen.Emit(OpCodes.Ldloc, lcl); break;
			}

		}
		public void LoadLocal_0() { _gen.Emit(OpCodes.Ldloc_0); }
		public void LoadLocal_1() { _gen.Emit(OpCodes.Ldloc_1); }
		public void LoadLocal_2() { _gen.Emit(OpCodes.Ldloc_2); }
		public void LoadLocal_3() { _gen.Emit(OpCodes.Ldloc_3); }
		public void LoadLocalAddress(LocalBuilder lcl)
		{
			Contracts.Require.IsNotNull("lcl", lcl);

			LoadLocalAddress(lcl.LocalIndex);
		}
		public void LoadLocalAddress(int lcl)
		{
			_gen.Emit(OpCodes.Ldloca, lcl);
		}
		public void LoadLocalAddressShort(LocalBuilder lcl)
		{
			Contracts.Require.IsNotNull("lcl", lcl);

			LoadLocalAddressShort(lcl.LocalIndex);
		}
		public void LoadLocalAddressShort(int lcl)
		{
			_gen.Emit(OpCodes.Ldloca_S, lcl);
		}

		public void LoadNull() { _gen.Emit(OpCodes.Ldnull); }
		public void LoadValue(bool value) { _gen.Emit(((value) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)); }
		public void LoadValue(int value)
		{
			switch (value)
			{
				case -1: _gen.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: _gen.Emit(OpCodes.Ldc_I4_0); break;
				case 1: _gen.Emit(OpCodes.Ldc_I4_1); break;
				case 2: _gen.Emit(OpCodes.Ldc_I4_2); break;
				case 3: _gen.Emit(OpCodes.Ldc_I4_3); break;
				case 4: _gen.Emit(OpCodes.Ldc_I4_4); break;
				case 5: _gen.Emit(OpCodes.Ldc_I4_5); break;
				case 6: _gen.Emit(OpCodes.Ldc_I4_6); break;
				case 7: _gen.Emit(OpCodes.Ldc_I4_7); break;
				case 8: _gen.Emit(OpCodes.Ldc_I4_8); break;
				default:
					_gen.Emit(OpCodes.Ldc_I4, value);
					break;
			}
		}
		public void LoadValue(long value) { _gen.Emit(OpCodes.Ldc_I8, value); }
		public void LoadValue(float value) { _gen.Emit(OpCodes.Ldc_R4, value); }
		public void LoadValue(double value) { _gen.Emit(OpCodes.Ldc_R8, value); }
		public void LoadValue(decimal value)
		{
			int[] v = decimal.GetBits((decimal)value);
			LocalBuilder lcl = _gen.DeclareLocal(typeof(int[]));
			NewArr(typeof(int), 3);
			StoreElement(lcl, 0, v[0]);
			StoreElement(lcl, 1, v[1]);
			StoreElement(lcl, 2, v[2]);
			LoadLocal(lcl.LocalIndex);
		}

		public void LoadValue(IValueRef value)
		{
			Contracts.Require.IsNotNull("value", value);
			
			value.LoadValue(this);
		}
		public void LoadValue(string value) { _gen.Emit(OpCodes.Ldstr, value); }
		public void LoadValue(object value)
		{
			if (value == null) _gen.Emit(OpCodes.Ldnull);
			else
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Boolean:
						LoadValue((bool)value);
						break;
					case TypeCode.Byte:
					case TypeCode.Char:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
						LoadValue(Convert.ToInt32(value));
						break;
					case TypeCode.DateTime:
						LoadValue((long)((DateTime)value).Ticks);
						NewObj(typeof(DateTime).GetConstructor(new Type[] { typeof(long) }));
						break;
					case TypeCode.Decimal:
						LoadValue(Convert.ToDecimal(value));
						break;
					case TypeCode.Double:
						LoadValue(Convert.ToDouble(value));
						break;
					case TypeCode.Empty:
						LoadNull();
						break;
					case TypeCode.Int64:
					case TypeCode.UInt64:
						LoadValue(Convert.ToInt64(value));
						break;
					case TypeCode.Single:
						LoadValue(Convert.ToSingle(value));
						break;
					case TypeCode.String:
						LoadValue(Convert.ToString(value));
						break;
					case TypeCode.Object:
					default:
						throw new InvalidOperationException();
				}
			}
		}
		public void LoadToken(Type t)
		{
			_gen.Emit(OpCodes.Ldtoken, t);
		}
		public void LoadType(Type t)
		{
			_gen.Emit(OpCodes.Ldtoken, t);
			_gen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), Type.EmptyTypes);
		}

		public void Return() { _gen.Emit(OpCodes.Ret); }

		public LocalBuilder DeclareLocal(Type localType)
		{
			return _gen.DeclareLocal(localType);
		}
		public LocalBuilder DeclareLocal(Type localType, bool pinned)
		{
			return _gen.DeclareLocal(localType, pinned);
		}

		public void NewObj(ConstructorInfo ctor)
		{
			_gen.Emit(OpCodes.Newobj, ctor);
		}
		private void NewArr(Type type, int elmCount)
		{
			Contracts.Require.IsNotNull("type", type);

			LoadValue(elmCount);
			_gen.Emit(OpCodes.Newarr, type);
		}

		public void Nop()
		{
			_gen.Emit(OpCodes.Nop);
		}

		public void StoreLocal(LocalBuilder lcl)
		{
			Contracts.Require.IsNotNull("lcl", lcl);

			StoreLocal(lcl.LocalIndex);
		}
		public void StoreLocalShortForm(int lcl)
		{
			_gen.Emit(OpCodes.Stloc_S, lcl);
		}
		public void StoreLocal(int lcl)
		{
			switch (lcl)
			{
				case 0: _gen.Emit(OpCodes.Stloc_0); break;
				case 1: _gen.Emit(OpCodes.Stloc_1); break;
				case 2: _gen.Emit(OpCodes.Stloc_2); break;
				case 3: _gen.Emit(OpCodes.Stloc_3); break;
				default: _gen.Emit(OpCodes.Stloc, lcl); break;
			}
		}
		public void StoreLocal_0() { _gen.Emit(OpCodes.Stloc_0); }
		public void StoreLocal_1() { _gen.Emit(OpCodes.Stloc_1); }
		public void StoreLocal_2() { _gen.Emit(OpCodes.Stloc_2); }
		public void StoreLocal_3() { _gen.Emit(OpCodes.Stloc_3); }
		public void StoreField(FieldInfo fld)
		{
			if (fld.IsStatic)
			{
				_gen.Emit(OpCodes.Stsfld, fld);
			}
			else
			{
				_gen.Emit(OpCodes.Stfld, fld);
			}
		}
		public void StoreValue(IValueRef value)
		{
			Contracts.Require.IsNotNull("value", value);

			value.StoreValue(this);
		}
		public void StoreElement(LocalBuilder lcl, int index, int value)
		{
			Contracts.Require.IsNotNull("lcl", lcl);
			
			LoadLocal(lcl.LocalIndex);
			LoadValue(index);
			LoadValue(value);
			_gen.Emit(OpCodes.Stelem_I4);
		}


		public Label DefineAndMarkLabel()
		{
			Label result = _gen.DefineLabel();
			_gen.MarkLabel(result);
			return result;
		}
		public Label DefineLabel()
		{
			return _gen.DefineLabel();
		}
		public void MarkLabel(Label lbl)
		{
			_gen.MarkLabel(lbl);
		}

		internal void BeginExceptionBlock()
		{
			_gen.BeginExceptionBlock();
		}
		internal void BeginFinallyBlock()
		{
			_gen.BeginFinallyBlock();
		}
		internal void EndExceptionBlock()
		{
			_gen.EndExceptionBlock();
		}

		public void Pop()
		{
			_gen.Emit(OpCodes.Pop);
		}

		public void Subtract()
		{
			_gen.Emit(OpCodes.Sub);
		}

		public void Switch(Label[] cases)
		{
			_gen.Emit(OpCodes.Switch, cases);
		}

		public void LoadProperty(PropertyInfo p, bool nonPublic)
		{
			Contracts.Require.IsNotNull("p", p);

			MethodInfo m = p.GetGetMethod(nonPublic);
			if (m == null) throw new InvalidOperationException(
				String.Format("Get method inaccessible for property: {0}", p.Name));
			this.Call(m);
		}
		public void StoreProperty(PropertyInfo p, bool nonPublic)
		{
			Contracts.Require.IsNotNull("p", p);

			MethodInfo m = p.GetSetMethod(nonPublic);
			if (m == null) throw new InvalidOperationException(
				String.Format("Set method inaccessible for property: {0}", p.Name));
			this.Call(m);
		}

		public void Throw(Type excType)
		{
			_gen.ThrowException(excType);
		}

		public void InitObject(Type typ)
		{
			Contracts.Require.IsNotNull("typ", typ);

			_gen.Emit(OpCodes.Initobj, typ);
		}

		public void UnboxAny(Type typ)
		{
			Contracts.Require.IsNotNull("typ", typ);

			_gen.Emit(OpCodes.Unbox_Any, typ);
		}

		public void LoadObjectRef()
		{
			_gen.Emit(OpCodes.Ldind_Ref);
		}
	}
	
}
