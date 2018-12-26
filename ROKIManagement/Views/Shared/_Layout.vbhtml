<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - ROKI Management</title>
    <!-- Bootstrap core CSS-->
    <link href="~/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom fonts for this template-->
    <link href="~/vendor/fontawesome-free/css/all.min.css" rel="stylesheet" type="text/css">

    <!-- Page level plugin CSS-->
    <link href="~/vendor/datatables/dataTables.bootstrap4.css" rel="stylesheet">
    @*<script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>*@
    <script src="~/vendor/jquery/jquery.min.js"></script>

    <link href="//cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/css/toastr.min.css" rel="stylesheet" />
    <link href="~/Content/Toast.css" rel="stylesheet" />
    
    <!-- Custom styles for this template-->
    <link href="~/Content/sb-admin.css" rel="stylesheet">

</head>
<body id="page-top">
    <nav class="navbar navbar-expand navbar-dark bg-dark static-top">
        <a class="navbar-brand mr-1" href="~/Home/Index">ROKI Document Management</a>
        <button class="btn btn-link btn-sm text-white order-1 order-sm-0" id="sidebarToggle" href="#">
            <i class="fas fa-bars"></i>
        </button>
        <ul class="navbar-nav ml-auto">
            <li class="nav-item">
                <a class="navbar-brand" href="~/Home/Index"><img src="~/img/roki-th-02.png" width="170" height="50" alt="Responsive image" /></a>
            </li>
        </ul>
        
    </nav>        
    <div id="wrapper">
        @If Session("StatusLogin") = "OK" Then
            @<ul Class="sidebar navbar-nav">
                <li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i Class="fas fa-fw fa-folder"></i>
                        <span> Document</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                        <a Class="dropdown-item" href="../Home/DocQms">QMS</a>
                    </div>
                </li>
                @if Session("GroupId") = 1 Then
                 @<li Class="nav-item dropdown">
                     <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                         <i Class="fas fa-user-circle"></i>
                         <span> Permission</span>
                     </a>
                     <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                         <a Class="dropdown-item" href="../Account/Group">Group</a>
                         <a Class="dropdown-item" href="../Account/Account">Account</a>
                         <a Class="dropdown-item" href="../Account/Application">Application</a>
                     </div>
                     
                 </li>
                End If
                <li Class="nav-item">
                    <a Class="nav-link" href="../Account/Logout">
                        <i Class="fas fa-sign-out-alt"></i>
                        <span> Logout</span>
                    </a>
                </li>
            </ul>
        End If
        <!-- Sidebar -->
        @*<ul Class="sidebar navbar-nav">
            <li Class="nav-item dropdown">
                <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    <i Class="fas fa-fw fa-folder"></i>
                    <span> Document</span>
                </a>
                <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                    <a Class="dropdown-item" href="../Home/DocQms">QMS</a>
                </div>
            </li>
        </ul>*@

        <div id = "content-wrapper" >

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

    <!-- Logout Modal-->
    <div class="modal fade" id="logoutModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Ready to Leave?</h5>
                    <button class="close" type="button" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body">Select "Logout" below if you are ready to end your current session.</div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" type="button" data-dismiss="modal">Cancel</button>
                    <a class="btn btn-primary" href="login.html">Logout</a>
                </div>
            </div>
        </div>
    </div>

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
    
</body>
</html>
