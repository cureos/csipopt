' Copyright (C) 2010 Anders Gustafsson and others. All Rights Reserved.
' This code is published under the Eclipse Public License.
'
' Author:  Anders Gustafsson 2010-05-28
' Converted from C# using the converter at 
' http://www.developerfusion.com/tools/convert/csharp-to-vb/

Imports System
Imports System.IO
Imports Cureos.Numerics

Namespace hs071_cs
    Public Class Program
        Public Shared Sub Main(ByVal args As String())

            ' create the IpoptProblem 
            Dim p As New HS071()

            ' allocate space for the initial point and set the values 
            Dim x As Double() = {1.0, 5.0, 5.0, 1.0}

            Dim obj As Double
            Dim status As IpoptReturnCode

            Using problem As New Ipopt(p.n, p.x_L, p.x_U, p.m, p.g_L, p.g_U, _
             p.nele_jac, p.nele_hess, AddressOf p.eval_f, AddressOf p.eval_g, _
             AddressOf p.eval_grad_f, AddressOf p.eval_jac_g, AddressOf p.eval_h)
                ' Set some options.  Note the following ones are only examples,
                ' they might not be suitable for your problem. 
                problem.AddOption("tol", 0.0000001)
                problem.AddOption("mu_strategy", "adaptive")
                problem.AddOption("output_file", "hs071.txt")

                ' solve the problem 
                status = problem.SolveProblem(x, obj, Nothing, Nothing, Nothing, Nothing)
            End Using

            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status)

            For i As Integer = 0 To 3
                Console.WriteLine("x[{0}]={1}", i, x(i))
            Next
        End Sub
    End Class

    Public Class HS071
        Public n As Integer
        Public m As Integer
        Public nele_jac As Integer
        Public nele_hess As Integer
        Public x_L As Double()
        Public x_U As Double()
        Public g_L As Double()
        Public g_U As Double()

        Public Sub New()
            ' set the number of variables and allocate space for the bounds 
            ' set the values for the variable bounds 
            n = 4
            x_L = {1.0, 1.0, 1.0, 1.0}
            x_U = {5.0, 5.0, 5.0, 5.0}

            ' set the number of constraints and allocate space for the bounds 
            ' set the values of the constraint bounds 
            m = 2
            g_L = {25, 40}
            g_U = {Ipopt.PositiveInfinity, 40}

            ' Number of nonzeros in the Jacobian of the constraints 
            nele_jac = 8

            ' Number of nonzeros in the Hessian of the Lagrangian (lower or upper triangual part only) 
            nele_hess = 10
        End Sub

        Public Function eval_f(ByVal n As Integer, ByVal x As Double(), ByVal new_x As Boolean, ByRef obj_value As Double) As Boolean
            obj_value = x(0) * x(3) * (x(0) + x(1) + x(2)) + x(2)

            Return True
        End Function

        Public Function eval_grad_f(ByVal n As Integer, ByVal x As Double(), ByVal new_x As Boolean, ByVal grad_f As Double()) As Boolean
            grad_f(0) = x(0) * x(3) + x(3) * (x(0) + x(1) + x(2))
            grad_f(1) = x(0) * x(3)
            grad_f(2) = x(0) * x(3) + 1
            grad_f(3) = x(0) * (x(0) + x(1) + x(2))

            Return True
        End Function

        Public Function eval_g(ByVal n As Integer, ByVal x As Double(), ByVal new_x As Boolean, ByVal m As Integer, ByVal g As Double()) As Boolean
            g(0) = x(0) * x(1) * x(2) * x(3)
            g(1) = x(0) * x(0) + x(1) * x(1) + x(2) * x(2) + x(3) * x(3)

            Return True
        End Function

        Public Function eval_jac_g(ByVal n As Integer, ByVal x As Double(), ByVal new_x As Boolean, ByVal m As Integer, ByVal nele_jac As Integer, ByVal iRow As Integer(), _
         ByVal jCol As Integer(), ByVal values As Double()) As Boolean
            If values Is Nothing Then
                ' set the structure of the jacobian 
                ' this particular jacobian is dense 
                iRow(0) = 0
                jCol(0) = 0
                iRow(1) = 0
                jCol(1) = 1
                iRow(2) = 0
                jCol(2) = 2
                iRow(3) = 0
                jCol(3) = 3
                iRow(4) = 1
                jCol(4) = 0
                iRow(5) = 1
                jCol(5) = 1
                iRow(6) = 1
                jCol(6) = 2
                iRow(7) = 1
                jCol(7) = 3
            Else
                ' return the values of the jacobian of the constraints 
                values(0) = x(1) * x(2) * x(3)  ' 0,0 
                values(1) = x(0) * x(2) * x(3)  ' 0,1 
                values(2) = x(0) * x(1) * x(3)  ' 0,2 
                values(3) = x(0) * x(1) * x(2)  ' 0,3 

                values(4) = 2 * x(0)            ' 1,0 
                values(5) = 2 * x(1)            ' 1,1 
                values(6) = 2 * x(2)            ' 1,2 
                values(7) = 2 * x(3)            ' 1,3 
            End If

            Return True
        End Function

        Public Function eval_h(ByVal n As Integer, ByVal x As Double(), ByVal new_x As Boolean, ByVal obj_factor As Double, ByVal m As Integer, ByVal lambda As Double(), _
         ByVal new_lambda As Boolean, ByVal nele_hess As Integer, ByVal iRow As Integer(), ByVal jCol As Integer(), ByVal values As Double()) As Boolean
            If values Is Nothing Then
                ' set the Hessian structure. This is a symmetric matrix, fill the lower left triangle only. 
                ' the hessian for this problem is actually dense 
                Dim idx As Integer = 0
                ' nonzero element counter 
                For row As Integer = 0 To 3
                    For col As Integer = 0 To row
                        iRow(idx) = row
                        jCol(idx) = col
                        idx += 1
                    Next

                Next
            Else
                ' return the values. This is a symmetric matrix, fill the lower left triangle only 
                ' fill the objective portion 
                values(0) = obj_factor * (2 * x(3))                 ' 0,0 

                values(1) = obj_factor * (x(3))                     ' 1,0 
                values(2) = 0                                       ' 1,1 

                values(3) = obj_factor * (x(3))                     ' 2,0 
                values(4) = 0                                       ' 2,1 
                values(5) = 0                                       ' 2,2 

                values(6) = obj_factor * (2 * x(0) + x(1) + x(2))   ' 3,0 
                values(7) = obj_factor * (x(0))                     ' 3,1 
                values(8) = obj_factor * (x(0))                     ' 3,2 
                values(9) = 0                                       ' 3,3 


                ' add the portion for the first constraint 
                values(1) += lambda(0) * (x(2) * x(3))              ' 1,0 

                values(3) += lambda(0) * (x(1) * x(3))              ' 2,0 
                values(4) += lambda(0) * (x(0) * x(3))              ' 2,1 

                values(6) += lambda(0) * (x(1) * x(2))              ' 3,0 
                values(7) += lambda(0) * (x(0) * x(2))              ' 3,1 
                values(8) += lambda(0) * (x(0) * x(1))              ' 3,2 

                ' add the portion for the second constraint 
                values(0) += lambda(1) * 2                          ' 0,0 
                values(2) += lambda(1) * 2                          ' 1,1 
                values(5) += lambda(1) * 2                          ' 2,2 
                values(9) += lambda(1) * 2                          ' 3,3 
            End If

            Return True
        End Function
    End Class
End Namespace
