﻿/* ****************************************************************************
 *
 * Copyright (c) Fredrik Holmström
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Extensions;

namespace IronJS.Compiler.Ast
{
    public class FuncNode : Node
    {
        public FuncNode Parent { get; protected set; }
        public IdentifierNode Name { get; protected set; }
        public Type ClosureType { get; set; }
        public List<IdentifierNode> Parameters { get; protected set; }
        public HashSet<IjsVarInfo> ClosesOver { get; protected set; }
        public INode Body { get; protected set; }
        public Dictionary<string, IjsVarInfo> Locals { get; protected set; }
        public HashSet<INode> Returns { get; protected set; }
        public LabelTarget ReturnLabel { get; protected set; }

        public bool IsNamed { get { return !IsLambda; } }
        public bool IsLambda { get { return Name == null; } }

        public bool IsBranched { get; set; }
        public bool IsSimple { get { return !IsBranched; } }

        public bool IsGlobalScope { get; protected set; }
        public bool IsNotGlobalScope { get { return !IsGlobalScope; } }

        public FuncNode(IdentifierNode name, List<IdentifierNode> parameters, INode body, ITree node)
            : base(NodeType.Func, node)
        {
            Body = body;
            Name = name;
            Parameters = parameters;

            Locals = new Dictionary<string, IjsVarInfo>();
            Returns = new HashSet<INode>();
            ClosesOver = new HashSet<IjsVarInfo>();

            if (IsNamed)
                Name.IsDefinition = true;
        }

        public override Type ExprType
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public Type ReturnType
        {
            get
            {
                if (IsSimple)
                    return Returns.EvalType();

                return IjsTypes.Dynamic;
            }
        }

        public FuncNode Analyze()
        {
            return (FuncNode) Analyze(this);
        }

        public override INode Analyze(FuncNode func)
        {
            if(func != this)
                Parent = func;

            if (IsNamed)
            {
                Name.Analyze(func);
                Name.VarInfo.AssignedFrom.Add(this);
            }

            foreach (var param in Parameters)
                param.Analyze(this);

            Body = Body.Analyze(this);

            return this;
        }

        public IjsVarInfo CreateLocal(string name)
        {
            if (HasLocal(name))
                throw new ArgumentException("A variable named '" + name + "' already exists");

            return Locals[name] = new IjsVarInfo(name);
        }

        public bool HasLocal(string name)
        {
            return Locals.ContainsKey(name);
        }

        public IjsVarInfo GetLocal(string name)
        {
            return Locals[name];
        }

        public IjsVarInfo GetNonLocal(string name)
        {
            IjsVarInfo varInfo;

            var parentFunctions = new HashSet<FuncNode>();
            var parent = Parent;

            while(true)
            {
                if(parent.HasLocal(name))
                {
                    if (parent.IsGlobalScope)
                    {
                        return parent.GetLocal(name);
                    }
                    else
                    {
                        varInfo = parent.GetLocal(name);
                        varInfo.IsClosedOver = true;

                        foreach (var subParent in parentFunctions)
                            subParent.ClosesOver.Add(varInfo);

                        ClosesOver.Add(varInfo);

                        return varInfo;
                    }
                }

                if (parent.Parent == null)
                    break;

                parentFunctions.Add(parent);
                parent = parent.Parent;
            }

            varInfo = parent.CreateLocal(name);
            varInfo.IsGlobal = true;

            return varInfo;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1) * 2);
            var indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr 
                + "(" + NodeType 
                + (" " + Name + " ").TrimEnd() 
                + " " + ReturnType.ShortName()
            );

            if (ClosesOver.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Closure");

                foreach (var id in ClosesOver)
                    writer.AppendLine(indentStr3 + "(" + id.Name + " " + id.ExprType.ShortName() + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            if(Parameters.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Parameters");

                foreach (var id in Parameters)
                    id.Print(writer, indent + 2);

                writer.AppendLine(indentStr2 + ")");
            }

            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }

        public static FuncNode CreateGlobalScope(List<INode> body)
        {
            return new FuncNode(
                null, 
                new List<IdentifierNode>(), 
                new BlockNode(body, null), 
                null
            ) { 
                IsGlobalScope = true, 
                ClosureType = typeof(object)
            };
        }
    }
}