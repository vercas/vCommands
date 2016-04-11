using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands
{
    /// <summary>
    /// Represents common status codes accross evaluation results.
    /// </summary>
    public enum CommonStatusCodes
        : int
    {
        /// <summary>
        /// Command completed successfully.
        /// </summary>
        Success = 0,

        /// <summary>
        /// A CLR exception has occurred.
        /// </summary>
        ClrException = -1,
        /// <summary>
        /// Invocation was cancelled by 
        /// </summary>
        InvocationCanceled = -2,

        /// <summary>
        /// Command does not support given toggler.
        /// </summary>
        TogglerNotSupported = -10,

        /// <summary>
        /// A series expression failed to evaluate all members.
        /// </summary>
        SeriesExpressionEvaluationFailure = -20,

        /// <summary>
        /// A conditional expression lacks a condition.
        /// </summary>
        ConditionalExpressionConditionMissing = -30,
        /// <summary>
        /// A conditional expression lacks a primary action.
        /// </summary>
        ConditionalExpressionPrimaryActionMissing = -31,

        /// <summary>
        /// Command expects a different amount of arguments.
        /// </summary>
        InvalidArgumentCount = 1,
        /// <summary>
        /// An argument was expected to succeed in evaluating.
        /// </summary>
        ArgumentEvaluationFailure = 2,
        /// <summary>
        /// An argument's value is outside of the expected range.
        /// </summary>
        ArgumentOutOfRange = 3,

        /// <summary>
        /// A command to invoke does not exist.
        /// </summary>
        CommandNotFound = 10,

        /// <summary>
        /// A desired variable could not be found.
        /// </summary>
        CvarNotFound = 20,
        /// <summary>
        /// A variable change was attempted with a null value expression.
        /// </summary>
        CvarValueNull = 21,
        /// <summary>
        /// Expression meant to be a value for a variable failed to evaluate.
        /// </summary>
        CvarValueEvaluationFailure = 22,
        /// <summary>
        /// Variable value expression is of invalid format.
        /// </summary>
        CvarValueFormatInvalid = 23,
        /// <summary>
        /// Variable value expression does not contain data of the required type.
        /// </summary>
        CvarValueDataLacking = 24,
        /// <summary>
        /// Type of variable does not support the specified change type.
        /// </summary>
        CvarChangeTypeNotSupported = 25,
        /// <summary>
        /// Cvar value cannot be read.
        /// </summary>
        CvarUnretrievable = 28,
        /// <summary>
        /// Cvar value cannot be changed.
        /// </summary>
        CvarUnchangeable = 29,

        /// <summary>
        /// Data of a specific type was expected.
        /// </summary>
        TypedDataNotFound = 30,
        /// <summary>
        /// Data of a specific type must be unique.
        /// </summary>
        TypedDataDuplicate = 31,

        /// <summary>
        /// An expression in an evaluation sequence returned non-zero.
        /// </summary>
        SequentialEvaluationFailure = 40,

        /// <summary>
        /// An expression acting as the body of a loop failed to evaluate.
        /// </summary>
        LoopExpressionFailure = 50,
        /// <summary>
        /// An expression acting as boundary to a loop evaluates to a non-positive value.
        /// </summary>
        LoopNegativeBound = 51,
        /// <summary>
        /// An expression acting as the increment amount to a loop evaluates to a value advancing in the wrong direction or is zero.
        /// </summary>
        LoopIncrementorInvalid = 52,

        /// <summary>
        /// A local variable with the specified name does not exist in the evaluation context.
        /// </summary>
        LocalVariableNotFound = 60,
        /// <summary>
        /// The evaluation context has no user arguments.
        /// </summary>
        UserArgumentsMissing = 61,
        /// <summary>
        /// The evaluation context does not contain an argument at the given index.
        /// </summary>
        UserArgumentNotFound = 62,
        /// <summary>
        /// User argument index is non-positive.
        /// </summary>
        UserArgumentIndexInvalid = 63,

        /// <summary>
        /// An alias or user command creation was attempted with a name that already exists.
        /// </summary>
        CommandAlreadyExists = 70,

        /// <summary>
        /// A mathematical logic relationship has a false value.
        /// </summary>
        MathematicalLogicFailure = 100,
    }
}
