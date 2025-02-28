﻿// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Internals;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Markup;

namespace Microsoft.CSharp.Activities;

[DebuggerStepThrough]
[ContentProperty("ExpressionText")]
public class CSharpValue<TResult> : CodeActivity<TResult>, ITextExpression
{
    private CompiledExpressionInvoker _invoker;

    public CSharpValue() => UseOldFastPath = true;

    public CSharpValue(string expressionText) : this() => ExpressionText = expressionText;

    public string ExpressionText { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Language => "C#";

    public Expression GetExpressionTree() => IsMetadataCached ? _invoker.GetExpressionTree() : throw FxTrace.Exception.AsError(new InvalidOperationException(SR.ActivityIsUncached));

    protected override void CacheMetadata(CodeActivityMetadata metadata)
    {
        _invoker = new CompiledExpressionInvoker(this, false, metadata);
       CsExpressionValidator.Instance.TryValidate<TResult>(this, metadata, ExpressionText);
    }

    protected override TResult Execute(CodeActivityContext context) => (TResult) _invoker.InvokeExpression(context);
}