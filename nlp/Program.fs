// Copyright (C) 2010 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson 2010-10-04

open Cureos.Numerics

// Set the number of variables and the variable bounds
let n = 4
let x_L = [| 1.0; 1.0; 1.0; 1.0 |]
let x_U = [| 5.0; 5.0; 5.0; 5.0 |]

// Set the number of constraints and set the values of the constraint bounds
let m = 2
let g_L = [| 25.0; 40.0 |]
let g_U = [| IpoptProblem.PositiveInfinity; 40.0 |]

// Number of nonzeros in the Jacobian of the constraints
let nele_jac = 8

// Number of nonzeros in the Hessian of the Lagrangian (lower or upper triangual part only)
let nele_hess = 10

// Objective function invoked delegate
let eval_f = new EvaluateObjectiveDelegate(fun n x new_x obj_value ->
    do
        obj_value <-  x.[0] * x.[3] * (x.[0] + x.[1] + x.[2]) + x.[2]
    true
)

// Constraint functions invoked delegate
let eval_g = new EvaluateConstraintsDelegate(fun n x new_x m g ->
    do
        g.[0] <- x.[0] * x.[1] * x.[2] * x.[3]
        g.[1] <- x.[0] * x.[0] + x.[1] * x.[1] + x.[2] * x.[2] + x.[3] * x.[3]
    true
)

// Objective function gradient invoked delegate
let eval_grad_f = new EvaluateObjectiveGradientDelegate(fun n x  new_x  grad_f ->
    do
        grad_f.[0] <- x.[0] * x.[3] + x.[3] * (x.[0] + x.[1] + x.[2])
        grad_f.[1] <- x.[0] * x.[3]
        grad_f.[2] <- x.[0] * x.[3] + 1.0
        grad_f.[3] <- x.[0] * (x.[0] + x.[1] + x.[2])
    true
)

// Jacobian invoked delegate
let eval_jac_g = new EvaluateJacobianDelegate(fun n x new_x m nele_jac iRow jCol values ->
        match values with
        | null ->
            do
                iRow.[0] <- 0
                jCol.[0] <- 0
                iRow.[1] <- 0
                jCol.[1] <- 1
                iRow.[2] <- 0
                jCol.[2] <- 2
                iRow.[3] <- 0
                jCol.[3] <- 3
                iRow.[4] <- 1
                jCol.[4] <- 0
                iRow.[5] <- 1
                jCol.[5] <- 1
                iRow.[6] <- 1
                jCol.[6] <- 2
                iRow.[7] <- 1
                jCol.[7] <- 3
            true
        | _ ->
            do
                values.[0] <- x.[1] * x.[2] * x.[3] // 0,0
                values.[1] <- x.[0] * x.[2] * x.[3] // 0,1
                values.[2] <- x.[0] * x.[1] * x.[3] // 0,2
                values.[3] <- x.[0] * x.[1] * x.[2] // 0,3

                values.[4] <- 2.0 * x.[0]           // 1,0
                values.[5] <- 2.0 * x.[1]           // 1,1
                values.[6] <- 2.0 * x.[2]           // 1,2
                values.[7] <- 2.0 * x.[3]           // 1,3
            true
)

// Hessian invoked delegate
let eval_h = new EvaluateHessianDelegate(fun n x new_x obj_factor m lambda new_lambda nele_hess iRow jCol values ->
    match values with
    | null ->
        do
            // Set the Hessian structure. This is a symmetric matrix, fill the lower left triangle only.
            // The hessian for this problem is actually dense.
            let mutable idx = 0
            for row = 0 to 3 do
                for col = 0 to row do
                    iRow.[idx] <- row
                    jCol.[idx] <- col
                    idx <- idx + 1
        true
    | _ ->
        do
            // return the values. This is a symmetric matrix, fill the lower left triangle only
            //           objective                                        first constraint                second constraint
            values.[0] <- obj_factor * (2.0 * x.[3])                                                    + lambda.[1] * 2.0  // 0,0

            values.[1] <- obj_factor * (x.[3])                          + lambda.[0] * (x.[2] * x.[3])                      // 1,0
            values.[2] <-                                                                                 lambda.[1] * 2.0  // 1,1

            values.[3] <- obj_factor * (x.[3])                          + lambda.[0] * (x.[1] * x.[3])                      // 2,0
            values.[4] <-                                                 lambda.[0] * (x.[0] * x.[3])                      // 2,1
            values.[5] <-                                                                                 lambda.[1] * 2.0  // 2,2

            values.[6] <- obj_factor * (2.0 * x.[0] + x.[1] + x.[2])    + lambda.[0] * (x.[1] * x.[2])                      // 3,0
            values.[7] <- obj_factor * (x.[0])                          + lambda.[0] * (x.[0] * x.[2])                      // 3,1
            values.[8] <- obj_factor * (x.[0])                          + lambda.[0] * (x.[0] * x.[1])                      // 3,2
            values.[9] <-                                                                                 lambda.[1] * 2.0  // 3,3
        true
)

// Allocate space for the initial point and set the values
let x = [| 1.0; 5.0; 5.0; 1.0 |]
let mutable obj = 0.0

// Initialize the optimization problem
let problem = new IpoptProblem(n, x_L, x_U, m, g_L, g_U, nele_jac, nele_hess, eval_f, eval_g, eval_grad_f, eval_jac_g, eval_h)

// Set some options.  Note the following ones are only examples, they might not be suitable for your problem.
problem.AddOption("tol", 1.0e-7) |> ignore
problem.AddOption("mu_strategy", "adaptive") |> ignore
problem.AddOption("output_file", "hs071.txt") |> ignore

// Solve the problem and output results
let status = problem.SolveProblem(x, &obj, null, null, null, null)

printfn ""
printfn "Optimization status: %O" status
printfn "x* = %A" x
