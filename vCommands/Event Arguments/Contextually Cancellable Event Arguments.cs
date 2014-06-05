using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace vCommands.EventArguments
{
    using Parsing.Expressions;
    
    /// <summary>
    /// Provides data for an event which can be cancelled under on a context and for a specified reason.
    /// </summary>
    public class ContextuallyCancellableEventArgs
        : CancelEventArgs
    {
        /// <summary>
        /// Gets the context under which the event is raised.
        /// </summary>
        public EvaluationContext Context { get; internal set; }

        /// <summary>
        /// Gets or sets a string representing the reason for cancelling the event.
        /// </summary>
        public String CancelReason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.EventArguments.ContextuallyCancellableEventArgs"/> class with the specified invocation context, cancel flag and cancel reason.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancel">True to cancel; otherwise false.</param>
        /// <param name="cancelReason">A text representing a human-readable reason for cancelling the event.</param>
        public ContextuallyCancellableEventArgs(EvaluationContext context, bool cancel, string cancelReason)
            : base(cancel)
        {
            this.Context = context;
            this.CancelReason = cancelReason;
        }
    }
}
