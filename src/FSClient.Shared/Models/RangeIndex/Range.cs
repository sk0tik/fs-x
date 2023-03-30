﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET6_0 || NETSTANDARD2_1
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Range))]
#else
namespace System
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>Represent a range has start and end indexes.</summary>
    /// <remarks>
    /// Range is used by the C# compiler to support the range syntax.
    /// <code>
    /// int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
    /// int[] subArray1 = someArray[0..2]; // { 1, 2 }
    /// int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
    /// </code>
    /// </remarks>
#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals
    [DebuggerNonUserCode]
    public readonly struct Range : IEquatable<Range>
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>Represent the inclusive start index of the Range.</summary>
        public Index Start { get; }

        /// <summary>Represent the exclusive end index of the Range.</summary>
        public Index End { get; }

        /// <summary>Construct a Range object using the start and end indexes.</summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Indicates whether the current Range object is equal to another object of the same type.</summary>
        /// <param name="value">An object to compare with this object</param>
#pragma warning disable CS8765 // Допустимость значений NULL для типа параметра не соответствует переопределенному элементу (возможно, из-за атрибутов допустимости значений NULL).
        public override bool Equals(object value)
#pragma warning restore CS8765 // Допустимость значений NULL для типа параметра не соответствует переопределенному элементу (возможно, из-за атрибутов допустимости значений NULL).
        {
            return value is Range r
                && r.Start.Equals(Start)
                && r.End.Equals(End);
        }

        /// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Range other)
        {
            return other.Start.Equals(Start) && other.End.Equals(End);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            return (Start, End).GetHashCode();
        }

        /// <summary>Converts the value of the current Range object to its equivalent string representation.</summary>
        public override string ToString()
        {
            // 2 for "..", then for each index 1 for '^' and 10 for longest possible uint
            var sb = new StringBuilder(2 + (2 * 11));

            if (Start.IsFromEnd)
            {
                sb.Append('^');
            }
            sb.Append(Start.Value);
            sb.Append('.');
            sb.Append('.');

            if (End.IsFromEnd)
            {
                sb.Append('^');
            }
            sb.Append(End.Value);

            return sb.ToString();
        }

        /// <summary>Create a Range object starting from start index to the end of the collection.</summary>
        public static Range StartAt(Index start)
        {
            return new Range(start, Index.End);
        }

        /// <summary>Create a Range object starting from first element in the collection to the end Index.</summary>
        public static Range EndAt(Index end)
        {
            return new Range(Index.Start, end);
        }

        /// <summary>Create a Range object starting from first element to the end.</summary>
        public static Range All => new Range(Index.Start, Index.End);

        /// <summary>Calculate the start offset and length of range object using a collection length.</summary>
        /// <param name="length">The length of the collection that the range will be used with. length has to be a positive value.</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter against negative values.
        /// It is expected Range will be used with collections which always have non negative length/count.
        /// We validate the range is inside the length scope though.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            int start;
            var startIndex = Start;
            if (startIndex.IsFromEnd)
            {
                start = length - startIndex.Value;
            }
            else
            {
                start = startIndex.Value;
            }

            int end;
            var endIndex = End;
            if (endIndex.IsFromEnd)
            {
                end = length - endIndex.Value;
            }
            else
            {
                end = endIndex.Value;
            }

            if ((uint)end > (uint)length || (uint)start > (uint)end)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            return (start, end - start);
        }
    }
}
#endif
