﻿<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - ROKI DMS</title>

    <!--Include CSS-->
    <link href="~/vendor/bootstrap/css/bootstrap.css" rel="stylesheet">
    <link href="~/vendor/fontawesome-free/css/all.min.css" rel="stylesheet" type="text/css">
    <link href="~/vendor/datatables/dataTables.bootstrap4.css" rel="stylesheet">

    <link href="//cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css" rel="stylesheet" />
    @*<script type="text/javascript" src="//www.shieldui.com/shared/components/latest/js/shieldui-all.min.js"></script>*@
    <link href="~/Content/Toast.css" rel="stylesheet" />

    <!--Include Java-->
    <script src="~/vendor/jquery/jquery.min.js"></script>
    
    <!-- Custom styles for this template-->
    <link href="~/Content/sb-admin.css" rel="stylesheet">
    <style>
        .bg-custom {
            background-color: #1a52c6;
        }
        #h1:hover {
            background-color: rgb(204, 204, 255);
        }
        
    </style>
</head>

<body id="page-top" style="font-size:14px">
    <nav class="navbar navbar-expand navbar-dark bg-light static-top">
        <a class="navbar-brand mr-1 text-muted" href="~/Home/Index">Document Management System</a>
        <button class="btn btn-link btn-sm text-white order-1 order-sm-0" id="sidebarToggle" href="#">
            <i class="fas fa-bars" style="color:rgb(16,91,172)"></i>
        </button>
        <ul class="navbar-nav ml-auto">
            <li class="nav-item">
                <a class="navbar-brand" href="~/Home/Index"><img src="~/img/roki-th-02.png" width="170" height="50" alt="Responsive image" /></a>
            </li>
        </ul>
        
    </nav>        
    <div id="wrapper">
        @If Session("StatusLogin") = "OK" Then
            @<ul Class="sidebar navbar-nav" style="background-color:rgb(0,79,162)">
                <li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i Class="fas fa-fw fa-folder"></i>
                        <span class="text-light"> Document</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                        @If Session("QMS") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="../Home/DocQms">QMS</a> End If
                        @If Session("ISO") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="#">ISO14001-2015</a> End If
                        @If Session("IATF") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="../Home/DocIatf">IATF16949-2016</a> End If
                    </div>
                </li>
                @If Session("Admin") = 1 Then
                    @<li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i class="fas fa-users-cog"></i>
                        <span Class="text-light"> Permission</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                        <a Class="dropdown-item" id="h1" href="../Account/Group">Group</a>
                        <a Class="dropdown-item" id="h1" href="../Account/Account">Account</a>
                        @*<a Class="dropdown-item" id="h1" href="../Account/Application">Application</a>*@
                    </div>
                </li>
                End If
                <li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i class="fas fa-user-circle"></i>
                        <span Class="text-light"> Profile</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                        <a Class="dropdown-item" id="h1" href="../Account/Setting">Setting</a>
                    </div>
                </li>
                <li Class="nav-item">
                    <a Class="nav-link" href="../Account/Logout">
                        <i Class="fas fa-sign-out-alt"></i>
                        <span Class="text-light"> Logout</span>
                    </a>
                </li>
            </ul>
        End If
        <!-- Sidebar -->

        <div id = "content-wrapper" class="mt-0" >
            <div class="container-fluid">
                @RenderBody()
            </div>
            <!-- /.container-fluid -->
            <!-- Sticky Footer -->
            @If Session("StatusLogin") = "OK" Then
            @<footer Class="sticky-footer">
                <div Class="container my-auto">
                    <div Class="copyright text-center my-auto">
                        <span> Copyright © Your Website 2018</span>
                    </div>
                </div>
            </footer>
            End If
            

        </div>
        <!-- /.content-wrapper -->

    </div>
    <!-- /#wrapper -->
    <!-- Scroll to Top Button-->
    <a class="scroll-to-top rounded" href="#page-top">
        <i class="fas fa-angle-up"></i>
    </a>

    <!-- Bootstrap core JavaScript-->

    <script src="~/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
                                                                                                                                                
    <!-- Core plugin JavaScript-->
    <script src="~/vendor/jquery-easing/jquery.easing.min.js"></script>
                                                                                                                                                
    <!-- Page level plugin JavaScript-->
    @*<script src="../vendor/chart.js/Chart.min.js"></script>*@
    <script src="~/vendor/datatables/jquery.dataTables.js"></script>
    <script src="~/vendor/datatables/dataTables.bootstrap4.js"></script>

    <!-- Custom scripts for all pages-->
    <script src="~/js/sb-admin.min.js"></script>

    <!-- Demo scripts for this page-->
    <script src="~/js/demo/datatables-demo.js"></script>
    @*<script src="../js/demo/chart-area-demo.js"></script>*@

    <script src="//cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/js/toastr.min.js"></script>
    <link rel="stylesheet" type="text/css" href="http://www.shieldui.com/shared/components/latest/css/light-bootstrap/all.min.css" />
    <script type="text/javascript" src="http://www.shieldui.com/shared/components/latest/js/shieldui-all.min.js"></script>
</body>
</html>
