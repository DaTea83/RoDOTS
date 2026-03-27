using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

// Taken from: https://github.com/Unity-Technologies/EntityComponentSystemSamples
namespace EugeneC.Utilities {

    public static partial class UtilityCollection {

        /// <summary>
        ///     A binary search helper for blob arrays.
        ///     It finds the segment in a sorted cumulative array (like arc lengths) where a given value belongs, returning the
        ///     index of the lower segment endpoint.
        /// </summary>
        public static int LowerBound<T>(ref this BlobArray<T> array, T value)
            where T : struct, IComparable<T> {
            return array.LowerBound(value, new NativeSortExtension.DefaultComparer<T>());
        }

        /// <summary>
        ///     Overload extension that allows to use a custom comparer
        /// </summary>
        /// <remarks>
        ///     For what is IComparer check out:
        ///     https://learn.microsoft.com/en-us/dotnet/api/system.collections.icomparer?view=net-9.0
        ///     IComparable : https://learn.microsoft.com/en-us/dotnet/api/system.icomparable?view=net-9.0
        /// </remarks>
        public static int LowerBound<T, TComparer>(ref this BlobArray<T> array, T value, TComparer comparer)
            where T : struct
            where TComparer : IComparer<T> {
            var leftBound = 0;
            var rightBound = array.Length;

            while (rightBound > leftBound) {
                var median = (leftBound + rightBound) / 2;
                var compare = comparer.Compare(array[median], value);

                //median = value
                if (compare == 0)
                    return leftBound;

                // median < value
                if (compare < 0)
                    leftBound = median + 1;
                // median > value
                else
                    rightBound = median;
            }

            return leftBound - 1;
        }

    }

}