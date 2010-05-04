﻿open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open System

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = 
  (Runtime.Environment.Create 
    Compiler.Core.compileFile 
    Compiler.Core.compileAst2)

let compiled = env.CompileFile "Testing.js"

let timeCompile = Utils.time(compiled).TotalMilliseconds
let time = Utils.time(compiled).TotalMilliseconds