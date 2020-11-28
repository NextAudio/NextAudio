using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NextAudio.Utils
{
    /// <summary>
    /// Provides content utilities.
    /// </summary>
    public static class ContentUtils
    {
        /// <summary>Throws a <see cref="ArgumentNullException" /> if <paramref name="value" /> is <c>null</c>.</summary>
        /// <param name="value">The value to be checked for <c>null</c>.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <c>null</c>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(this T? value, string paramName)
            where T : class
        {
            if (value.IsNull())
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Check if <paramref name="value"/> is <c>null</c>.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
        /// <returns><c>true</c> if <paramref name="value"/> is <c>null</c></returns>
        public static bool IsNull<T>(this T? value)
            where T : class
        {
            return value == null;
        }

        /// <summary>
        /// Check if <paramref name="value"/> is not <c>null</c>.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <typeparam name="T">The type of <paramref name="value" />.</typeparam>
        /// <returns><c>true</c> if <paramref name="value"/> is not <c>null</c></returns>
        public static bool IsNotNull<T>(this T? value)
            where T : class
        {
            return value != null;
        }
    }
}