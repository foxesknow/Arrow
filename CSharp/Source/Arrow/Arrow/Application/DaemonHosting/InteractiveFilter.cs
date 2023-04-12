﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.DaemonHosting
{
    public sealed class InteractiveFilter
    {
        /// <summary>
        /// The function that filters the data
        /// </summary>
        /// <param name="pipelinePart"></param>
        /// <returns>The sequence of data generated by the command</returns>
        public delegate IEnumerable<object?> HandlerFunction(PipelinePart pipelinePart, IEnumerable<object?> input);

        public InteractiveFilter(string name, string description, HandlerFunction handler) : this(name, null, description, handler)
        {
        }

        public InteractiveFilter(string name, string? syntax, string description, HandlerFunction handler)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));

            if(description is null) throw new ArgumentNullException(nameof(description));
            if(string.IsNullOrWhiteSpace(description)) throw new ArgumentException("description is empty", nameof(description));

            if(handler is null) throw new ArgumentNullException(nameof(handler));

            this.Name = name;
            this.Syntax = syntax;
            this.Description = description;
            this.Handler = handler;
        }

        /// <summary>
        /// The name of the filter
        /// </summary>
        public string Name{get;}

        /// <summary>
        /// What the filter does
        /// </summary>
        public string Description{get;}

        /// <summary>
        /// And additional syntax for the filter
        /// </summary>
        public string? Syntax{get;}

        /// <summary>
        /// The handler for the filter
        /// </summary>
        public HandlerFunction Handler{get;}

        /// <summary>
        /// A human readable description of the filter
        /// </summary>
        /// <returns></returns>
        public string DisplayName()
        {
            var value = this.Name;
            if(this.Syntax is not null)
            {
                value += " " + this.Syntax;
            }

            return value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name}\t{this.Description}";
        }
    }
}