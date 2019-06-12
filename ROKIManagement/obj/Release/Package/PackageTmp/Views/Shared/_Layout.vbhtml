<!DOCTYPE html>
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

    <link rel="stylesheet" type="text/css" href="https://cdn3.devexpress.com/jslib/18.2.4/css/dx.common.css" />
    <link rel="dx-theme" data-theme="generic.light" href="https://cdn3.devexpress.com/jslib/18.2.4/css/dx.light.css" />
    <script src="https://cdn3.devexpress.com/jslib/18.2.4/js/dx.all.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.13.0/moment.min.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.4/cldr.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.4/cldr/event.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.4/cldr/supplemental.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/cldrjs/0.4.4/cldr/unresolved.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/globalize/1.1.1/globalize.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/globalize/1.1.1/globalize/message.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/globalize/1.1.1/globalize/number.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/globalize/1.1.1/globalize/currency.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/globalize/1.1.1/globalize/date.min.js"></script>


    <style>
        .bg-custom {
            background-color: #1a52c6;
        }
        #h1:hover, #pagesDropdown11:hover {
            background-color: rgb(204, 204, 255);
        }
        .text-custom{
            color:rgb(0, 79, 162);
        }
        /*#level-tree {border-style: none;}*/
        
    </style>
</head>

<body id="page-top" style="font-size:14px">

    <div class="modal" id="mdResetPassword">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <H2>Change Password</H2>
                </div>                
                <div class="modal-body">                    
                    <div class="row mb-2">
                        <div class="col-sm">
                            <input type="password" class="form-control" id="lbPassword" placeholder="New Password" value="">
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-sm">
                            <input type="password" class="form-control" id="lbPasswordConfirm" placeholder="Confirm New Password" value="">
                        </div>
                    </div>
                </div>

                <!-- Modal footer -->
                <div class="modal-footer">
                    <button type="button" id="btnSave" class="btn btn-success">Save</button>
                    <button type="button" onclick="clearModalMain();" class="btn btn-danger">Close</button>
                </div>

            </div>
        </div>
    </div>

    <nav class="navbar navbar-expand navbar-dark bg-light static-top">        
        <a class="navbar-brand" href="~/Home/Index"><img src="~/img/roki-th-03.png" width="280" height="60" alt="Responsive image" /></a>
        @*<div class="mx-auto"><a class="navbar-brand" href="index.html" style="color:rgb(118,120,123);">Document Management System</a></div>*@
        
        @If IsDBNull(Session("UserId")) = False AndAlso Session("UserId") <> 0 Then
        @<a Class="nav-link text-custom ml-auto" href="#">
            <span> <i Class="fas fa-user-circle fa-fw"></i> @Session("Name")</span>
        </a>
        @<a Class="nav-link text-custom" href="~/Account/Setting">
            <span> <i Class="fas fa-user-cog"></i> Setting</span>
        </a>
        @<a Class="nav-link text-custom" href="~/Account/Logout">
            <span> <i Class="fas fa-sign-out-alt"></i> Logout</span>
        </a>
        End If
        
    </nav>        
    <div id = "wrapper" >
        @If Session("StatusLogin") = "OK" Then
            @<ul Class="sidebar navbar-nav" style="background-color:rgb(0,79,162)">
            @If Session("QMS") <> 0 Or Session("ISO") <> 0 Or Session("IATF") <> 0 Or Session("Admin") = 1 Then
                @<li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle d1" href="#" id="pagesDropdown1" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                        <i Class="fas fa-fw fa-folder"></i>
                        <span Class="text-light"> Document</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown1">
                        @Code 
'Dim arrId() As String = Session("AppId").ToString.Split(",")
'For i As Integer = 0 To 

                        End Code
                        @If Session("QMS") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item QMS" id="h1" href="~/Home/DocQms">QMS</a> End If
                        @If Session("ISO") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Home/ISO">ISO 14001-2015</a> End If
                        @If Session("IATF") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Home/IATF">IATF 16949-2016</a> End If
                        @If Session("SQM_ROKI") <> 0 Or Session("SQM_CUSTOMER") <> 0 Or Session("Admin") = 1 Then @<a class="dropdown-toggle dropdown-item sqm" href="#" id="pagesDropdown11" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">SQM</a>End if
                            <ul class="nav nav-third-level">
                            <li>
                                <div Class="dropdown-menu mt-2" id="level-tree" aria-labelledby="pagesDropdown11">
                                    @If Session("SQM_ROKI") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Home/SQMROKI">ROKI</a>End If
                                    @If Session("SQM_CUSTOMER") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Home/SQMCUSTOMER">CUSTOMER</a>End If
                                    @*<a Class="dropdown-item" id="h1" href="#">HRST</a>
                                    <a Class="dropdown-item" id="h1" href="#">HTAS</a>
                                    <a Class="dropdown-item" id="h1" href="#">KMT</a>
                                    <a Class="dropdown-item" id="h1" href="#">MAZDA</a>
                                    <a Class="dropdown-item" id="h1" href="#">META</a>
                                    <a Class="dropdown-item" id="h1" href="#">THM</a>
                                    <a Class="dropdown-item" id="h1" href="#">TYM</a>
                                    <a Class="dropdown-item" id="h1" href="#">HRAP</a>
                                    <a Class="dropdown-item" id="h1" href="#">IMCT</a>
                                    <a Class="dropdown-item" id="h1" href="#">NMT</a>
                                    <a Class="dropdown-item" id="h1" href="#">RJP</a>
                                    <a Class="dropdown-item" id="h1" href="#">TMT</a>
                                    <a Class="dropdown-item" id="h1" href="#">TSM</a>
                                    <a Class="dropdown-item" id="h1" href="#">AAT</a>
                                    <a Class="dropdown-item" id="h1" href="#">MMTH</a>*@
                                </div>
                            </li>
                        </ul>
                        @*<li class="nav-item dropdown">
                            <a Then Class="nav-link dropdown-toggle d11" href="#" id="pagesDropdown11" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">SQM Customer</a>
                            <div Class="dropdown-menu" aria-labelledby="pagesDropdown1">
                                <a Class="dropdown-item" id="h1" href="../Drawing/DUCATI">DUCATI</a>
                            </div>
                        </li>*@
                        
                    </div>
                </li>
                            End if
        @If Session("HATC") <> 0 Or Session("THM") <> 0 Or Session("TSM") <> 0 Or Session("AAT") <> 0 Or Session("Admin") = 1 Then
                 @<li Class="nav-item dropdown">
                     <a Class="nav-link dropdown-toggle d2" href="#" id="pagesDropdown2" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                         <i Class="fas fa-drafting-compass"></i>
                         <span Class="text-light"> Drawing</span>
                     </a>
                     <div Class="dropdown-menu" aria-labelledby="pagesDropdown2">
                         @If Session("DUCATI") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/DUCATI">DUCATI</a> End If
                         @If Session("HATC") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/HATC">HATC</a> End If
                         @If Session("HRST") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/HRST">HRST</a> End If
                         @If Session("HTAS") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/HTAS">HTAS</a> End If
                         @If Session("KMT") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/KMT">KMT</a> End If
                         @If Session("MAZDA") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/MAZDA">MAZDA</a> End If
                         @If Session("META") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/META">META</a> End If
                         @If Session("THM") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/THM">THM</a> End If
                         @If Session("TYM") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/TYM">TYM</a> End If
                         @If Session("HRAP") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/HRAP">HRAP</a> End If
                         @If Session("IMCT") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/IMCT">IMCT</a> End If
                         @If Session("NMT") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/NMT">NMT</a> End If
                         @If Session("RJP") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/RJP">RJP</a> End If
                         @If Session("TMT") <> 0 Or Session("Admin") = 1 Then @<a Then Class="dropdown-item" id="h1" href="~/Drawing/TMT">TMT</a> End If
                         @If Session("TSM") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/TSM">TSM</a> End If
                         @If Session("AAT") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/AAT">AAT</a> End If
                         @If Session("MMTH") <> 0 Or Session("Admin") = 1 Then @<a Class="dropdown-item" id="h1" href="~/Drawing/MMTH">MMTH</a> End If
                     </div>
                 </li>
                 end if

                @If Session("Admin") = 1 Then
                    @<li Class="nav-item dropdown">
                        <a Class="nav-link dropdown-toggle" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                            <i class="fas fa-users-cog"></i>
                            <span Class="text-light"> Permission</span>
                        </a>
                        <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                            <a Class="dropdown-item" id="h1" href="~/Account/Group">Group</a>
                            <a Class="dropdown-item" id="h1" href="~/Account/Account">Account</a>
                            @*<a Class="dropdown-item" id="h1" href="~/Account/Application">Application</a>*@
                        </div>
                    </li>
                End If
                @*@If Session("Admin") <> 1 Then
                @<li Class="nav-item dropdown">
                    <a Class="nav-link dropdown-toggle d3" href="#" id="pagesDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                        <i Class="fas fa-user-circle"></i>
                        <span Class="text-light"> Profile</span>
                    </a>
                    <div Class="dropdown-menu" aria-labelledby="pagesDropdown">
                        <a Class="dropdown-item" id="h1" href="../Account/Setting">Setting</a>
                    </div>
                </li>
                End If
                <li Class="nav-item">
                    <a Class="nav-link" href="../Account/Logout">
                        <i Class="fas fa-sign-out-alt"></i>
                        <span Class="text-light"> Logout</span>
                    </a>
                </li>*@
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
                        <span class="text-custom"> Copyright © ROKI (THAILAND) Co.,Ltd. All rights reserved.</span>
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
    @*<link rel="stylesheet" type="text/css" href="http://www.shieldui.com/shared/components/latest/css/light-bootstrap/all.min.css" />
    <script type="text/javascript" src="http://www.shieldui.com/shared/components/latest/js/shieldui-all.min.js"></script>*@
</body>
</html>
<script>
    var permission_status = '@Session("StatusLogin")';
    var first_status = '@Session("FirstLogin")';
    console.log('p=>' + permission_status);
    console.log('f=>' + first_status);
    if (permission_status == 'OK' && first_status == 0) {
        $("#mdResetPassword").modal('show');
    }

    function clearModalMain() {

        document.getElementById('lbPassword').value = '';

        $("#mdResetPassword").modal('hide');

    };
    $(function () {        
        $("#btnSave").click(function () {
            var password = document.getElementById('lbPassword').value;
            var password_confirm = document.getElementById('lbPasswordConfirm').value;
            var userId = @Session("UserId");
            if (password == password_confirm) {
                if(password == '' || password_confirm == ''){
                    DevExpress.ui.notify("กรุณากรอกรหัสผ่าน", "error", 1000); 
                }else{
                    $.ajax({
                        type: "POST",
                        url: "../Account/UpDatePassword",
                        contentType: "application/json; charset=utf-8",
                        data: "{ Password:'" + password + "', userId: "+ userId + " }",
                        dataType: "json",
                        async: false,
                        success: function (data) { 
                            DevExpress.ui.notify("เปลี่ยนรหัสผ่านเรียบร้อย", "success", 1000);                        
                            $('#mdResetPassword').modal('hide');                    
                        },
                        error: function (error) {
                            alert(error);
                        }
                    });
                }
            } else {
                DevExpress.ui.notify("รหัสผ่านไม่ตรงกัน", "error", 1000); 
            }
        });
    });
</script>



