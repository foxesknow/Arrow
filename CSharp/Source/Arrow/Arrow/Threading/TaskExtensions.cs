﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ConfiguredTaskAwaitable ContinueOnAnyContext(this Task task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ConfiguredTaskAwaitable<T> ContinueOnAnyContext<T>(this Task<T> task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable ContinueOnAnyContext(this ValueTask task)
        {
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable<T> ConfiguredValueTaskAwaitable<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
