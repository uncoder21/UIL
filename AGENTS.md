# Development Plan

This repository aims to build a compiler with the following features and subfeatures. Contributors should use this document as high-level guidance for future work.

## 1. Symbol Resolution
- **Scoping Rules**: Resolve names based on local, class, base class, interface, and global scopes.
- **Accessibility**: Consider visibility (public/private/protected/internal).
- **Extension Methods**: Include statically imported methods based on `using` directives.
- **Name Qualification**: Support dotted names like `Namespace.Class.Method`.

## 2. Generics Support
- **Generic Type Parameters**: Support `T`, `TKey`, etc. in class/method declarations.
- **Inference**: Infer types at call site when not explicitly provided.
- **Constraints**: Enforce type constraints like `where T : class, new()`.

## 3. Overload Resolution
- **Arity Matching**: Match based on number of parameters.
- **Type Compatibility**: Score overloads by how well argument types match.
- **Generic Matching**: Filter overloads by generic type compatibility.
- **Ambiguity Diagnostics**: Report ambiguous calls with suggestions.

## 4. Constant Evaluation
- **Constant Folding**: Simplify arithmetic expressions. ✅ Implemented for `+`, `-`, `*`, `/` with division-by-zero diagnostics.
- **Compile-Time Expressions**: Support `typeof`, `default`, `sizeof`, `nameof`, etc.
- **Conditional Expressions**: Evaluate ternary or boolean expressions when inputs are constant.

## 5. Nullability Analysis
- **Flow-Sensitive Tracking**: Track null state across control flow.
- **Nullable Types & Annotations**: Support `T?`, null checks, and diagnostics.
- **Pure Method Propagation**: Infer nullability from function results.
- **Unnecessary Check Elimination**: Detect and remove redundant null checks/coalescing.

## 6. Control Flow Analysis (CFA)
- **Flow Graph Construction**: Build CFG for each method.
- **Dominators and Reachability**: Use CFG for optimization passes.
- **Closure Capture Analysis**: Detect captured variables in lambdas.

## 7. SSA Form (Static Single Assignment)
- **Φ-functions**: Introduce φ-nodes at control joins.
- **Variable Versioning**: Ensure each assignment has a unique version.
- **SSA Graph**: Represent data flow explicitly for each variable.

## 8. Optimizations
- **Dead Code Elimination (DCE)**: Remove code with no effect.
- **Sparse Conditional Constant Propagation (SCCP)**: Propagate known constants.
- **Global Value Numbering (GVN)**: Remove redundant computations.
- **Loop-Invariant Code Motion (LICM)**: Move constant calculations outside loops.
- **Copy/Strength Reduction**: Simplify expressions and weaken expensive operations.
- **Layout Scheduling**: Reorder blocks to reduce branching and improve prediction.
- **Register Pressure Estimation**: Analyze temporary use to guide layout.

## 9. Intermediate Language (IL) Generation
- **Abstract IL Instructions**: Stack-based, minimal instruction set.
- **Labeling and Jumps**: Block-based layout and jump resolution.
- **Deferred Address Computation**: Labels and jumps patched after layout.

## 10. Metadata & Debug Info
- **Fixed-Size Metadata**: Type, method, and field metadata stored compactly.
- **Variable Pool**: Store strings and names in a separate section.
- **Source Spans**: Track file/line/column for each statement/expression.
- **Debug Symbols**: Enable mapping between IL and source code.

## 11. Instrumentation
- **Instrumentation Sink**: Collect data at predefined points in each pass.
- **Validation Hooks**: Assert correctness after transformations.
- **Performance Counters**: Optional collection of transformation statistics.

## 12. Code Layout Planning
- **Block Ordering**: Order IL blocks for fallthrough and prediction.
- **Label Resolution**: Calculate jump distances after layout.
- **Short/Long Jump Rewriting**: Pick optimal encoding post layout.

## 13. (Optional) Native Code Backend
- **Register Allocation**: Convert stack ops to register ops.
- **Calling Convention**: Emit prologue/epilogue and argument passing.
- **ABI Integration**: For interop or bootable formats.

