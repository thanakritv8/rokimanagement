@Code
    ViewData("Title") = "Account"
End Code
<style>
    .toast-top-center {
        top: 50%;
        margin: 0 auto;
        margin-left: -150px;
    }

    #dataTable {
        width: 100%;
    }
</style>
<!-- Breadcrumbs-->
<ol class="breadcrumb">
    <li class="breadcrumb-item">
        <a href="#">Main</a>
    </li>
    <li class="breadcrumb-item active">Account</li>
</ol>

<!-- DataTables -->
<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-table"></i>
        Config Account
        <button type="button" class="btn btn-success btn-sm float-sm-right" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Add" data-target="#accountAdd"><i class="fas fa-plus-circle"></i></button>
    </div>

    <div class="card-body">
        <div class="table-responsive">
            <table class="table table-bordered" id="dataTable" cellspacing="0">
                <thead>
                    <tr>
                        <th>Firstname</th>
                        <th>Lastname</th>
                        <th>Username</th>
                        <th>Department</th>
                        <th>Email</th>
                        <th>IsActive</th>
                        <th>Group</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody id="dataRow"></tbody>
            </table>

            <!-- The Modal docAdd-->
            <div class="modal" id="accountAdd">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">
                            <h4 class="modal-title">Account Add</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="firstnamea" placeholder="firstname" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="lastnamea" placeholder="lastname" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="usernamea" placeholder="username" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="password" class="form-control" style="background-color: #FCFBC8" id="passworda" placeholder="password" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="departmenta" placeholder="department" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="emaila" placeholder="email" value="" required>
                                        </div>
                                    </div> 
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <select class="form-control form-control-sm" id="nameGroupa" style="background-color: #FCFBC8"></select>
                                        </div>
                                    </div>                                   
                                </div>
                            </div>
                        </div>

                        <!-- Modal footer -->
                        <div class="modal-footer">
                            <button type="button" id="btnSave" class="btn btn-success" data-dismiss="modal">Save</button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>

                    </div>
                </div>
            </div>
            <!--End Modal docAdd-->
            <!--Modal Edit-->
            <div class="modal" id="accountEdit">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">
                            <h4 class="modal-title">Account Edit ID <label id="seq"></label></h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="firstnamee" placeholder="firstname" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="lastnamee" placeholder="lastname" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="usernamee" placeholder="username" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="password" class="form-control" style="background-color: #FCFBC8" id="passworde" placeholder="password" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="departmente" placeholder="department" value="" required>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="emaile" placeholder="email" value="" required>
                                        </div>
                                    </div>  
                                    <div class="row mb-2">
                                        <div class="col">
                                            <select class="form-control form-control-sm" id="nameGroupe" style="background-color: #FCFBC8"></select>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col">
                                            <select class="form-control form-control-sm" id="isactive" style="background-color: #FCFBC8"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Modal footer -->
                        <div class="modal-footer">
                            <button type="button" id="btnEdit" class="btn btn-success" data-dismiss="modal">OK</button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>

                    </div>
                </div>
            </div>
            <!--End Modal Edit-->
            <!--Modal Del-->
            <div class="modal" id="accountDel">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">
                            <h4 class="modal-title">Account Delete ID <label id="seqDel"></label></h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" id="usernamed" placeholder="group name" value="" readonly>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Modal footer -->
                        <div class="modal-footer">
                            <button type="button" id="btnDel" class="btn btn-success" data-dismiss="modal">Delete</button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>

                    </div>
                </div>
            </div>
            <!--End Modal Del-->
        </div>
    </div>
    <div class="card-footer small text-muted">
        <div class="row">
            <div class="col">
                Updated yesterday at 11:59 PM
            </div>
        </div>
    </div>
</div>
<script>
    $(document).ready(function () {
        $('[data-tooltip="tooltip"]').tooltip();
        $("#btnSave").click(function () {
            var formData = new FormData();
            var firstname = document.getElementById('firstnamea').value;
            var lastname = document.getElementById('lastnamea').value;
            var username = document.getElementById('usernamea').value;
            var password = document.getElementById('passworda').value;
            var department = document.getElementById('departmenta').value;
            var email = document.getElementById('emaila').value;
            var groupid = document.getElementById('nameGroupa').value;

            if (groupid != 0 && username != '' && password != '') {
                formData.append("firstname", firstname);
                formData.append("lastname", lastname);
                formData.append("username", username);
                formData.append("password", password);
                formData.append("department", department);
                formData.append("email", email);
                formData.append("groupid", groupid);
                $.ajax({
                    type: "POST",
                    url: '../Account/UploadAccount',
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        document.getElementById('firstnamea').value = '';
                        document.getElementById('lastnamea').value = '';
                        document.getElementById('usernamea').value = '';
                        document.getElementById('passworda').value = '';
                        document.getElementById('departmenta').value = '';
                        document.getElementById('emaila').value = '';
                        document.getElementById('nameGroupa').value = '0';
                        getAccount();
                        showToast(2);
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            } else {
                showToast(0);
            }
        });

        $("#btnEdit").click(function () {
            var formData = new FormData();
            var firstname = document.getElementById('firstnamee').value;
            var lastname = document.getElementById('lastnamee').value;
            var username = document.getElementById('usernamee').value;
            var password = document.getElementById('passworde').value;
            var department = document.getElementById('departmente').value;
            var email = document.getElementById('emaile').value;
            var groupid = document.getElementById('nameGroupe').value;
            var isactive = document.getElementById('isactive').value;
            var seq = document.getElementById('seq').innerHTML;

            formData.append("firstname", firstname);
            formData.append("lastname", lastname);
            formData.append("username", username);
            formData.append("password", password);
            formData.append("department", department);
            formData.append("email", email);
            formData.append("isactive", isactive);
            formData.append("groupid", groupid);
            formData.append("userid", seq);

            $.ajax({
                type: "POST",
                url: '../Account/UpDateAccount',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('firstnamee').value = '';
                    document.getElementById('lastnamee').value = '';
                    document.getElementById('usernamee').value = '';
                    document.getElementById('passworde').value = '';
                    document.getElementById('departmente').value = '';
                    document.getElementById('emaile').value = '';                    
                    document.getElementById('seq').innerHTML = '';
                    getAccount();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $("#btnDel").click(function () {
            var formData = new FormData();
            var seq = document.getElementById('seqDel').innerHTML;
            formData.append("UserId", seq);

            $.ajax({
                type: "POST",
                url: '../Account/DelAccount',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('seqDel').innerHTML = '';
                    getAccount();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });
    });

    getAccount();

    $('#accountEdit').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seq").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Account/GetAccountWithUserId",
            contentType: "application/json; charset=utf-8",
            data: "{UserId:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                var tblSomething = '';
                $.each(data, function (idx, obj) {

                    $.each(obj, function (key, value) {
                        if (key == "Firstname") {
                            document.getElementById("firstnamee").value = value;
                        } else if (key == "Lastname") {
                            document.getElementById("lastnamee").value = value;
                        } else if (key == "Username") {
                            document.getElementById("usernamee").value = value;
                        } else if (key == "Password") {
                            document.getElementById("passworde").value = value;
                        } else if (key == "Department") {
                            document.getElementById("departmente").value = value;
                        } else if (key == "email") {
                            document.getElementById("emaile").value = value;
                        } else if (key == "IsActive") {
                            if (value == true) {
                                $('#isactive').html("<option value='1'>Enable</option><option value='0'>Disable</option>");
                            } else {
                                $('#isactive').html("<option value='0'>Disable</option><option value='1'>Enable</option>");
                            }
                        } else if (key == "GroupId") {
                            $.ajax({
                                type: "POST",
                                url: "../Account/GetGroup",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (datatemp) {
                                    var tblSomething = '';
                                    $.each(datatemp, function (idxtemp, objtemp) {
                                        var groupid = 0;
                                        $.each(objtemp, function (keytemp, valuetemp) {
                                            if (keytemp == "nameGroup") {
                                                if (value == groupid) {
                                                    tblSomething += "<option value='" + groupid + "' selected>" + valuetemp + "</option>";
                                                } else {
                                                    tblSomething += "<option value='" + groupid + "'>" + valuetemp + "</option>";
                                                }
                                                
                                            } else if (key == "GroupId") {
                                                groupid = valuetemp;
                                            }
                                        });
                                    });
                                    $('#nameGroupe').html(tblSomething);
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    alert('Hey, something went wrong because: ' + errorThrown);
                                }
                            });
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });

    });


    $('#accountAdd').on('show.bs.modal', function (e) {
        $.ajax({
            type: "POST",
            url: "../Account/GetGroup",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var tblSomething = "<option value='0'>Select Group</option>";
                $.each(data, function (idx, obj) {
                    var groupid = 0;
                    $.each(obj, function (key, value) {
                        if (key == "nameGroup") {
                            tblSomething += "<option value='" + groupid + "'>" + value + "</option>";
                        } else if (key == "GroupId") {
                            groupid = value;
                        }
                    });
                });
                $('#nameGroupa').html(tblSomething);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    });

    $('#accountDel').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seqDel").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Account/GetAccountWithUserId",
            contentType: "application/json; charset=utf-8",
            data: "{ UserId:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, obj) {
                    $.each(obj, function (key, value) {
                        if (key == "Username") {
                            document.getElementById("usernamed").value = value;
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    });

    function getAccount() {
        $.ajax({
            type: "POST",
            url: "../Account/GetAccount",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var tblSomething = '';
                $.each(data, function (idx, obj) {
                    tblSomething += '<tr>';
                    var UserId = 0;
                    $.each(obj, function (key, value) {
                        if (key == "UserId") {
                            UserId = value;
                        } else if (key == "IsActive") {
                            if (value == true) {
                                tblSomething += "<td class='text-center'><i class='fas fa-user text-success'></i></td>";
                            } else {
                                tblSomething += "<td class='text-center'><i class='fas fa-user-slash text-danger'></i></td>";
                            }
                            
                        }else{
                            tblSomething += "<td>" + value + "</td>";
                        }
                    });

                    tblSomething += '<td class=\'text-center\'><div class="btn-group"><button type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Edit" data-target="#accountEdit" data-book-id=' + UserId + '><i class="fas fa-edit"></i></button><button type="button" class="btn btn-danger btn-sm" data-tooltip="tooltip" data-placement="right" title="Delete" data-toggle="modal" data-target="#accountDel" data-book-id=' + UserId + '><i class="fas fa-trash-alt"></i></button></div></td>'
                    tblSomething += '</tr>';

                });
                $('#dataRow').html(tblSomething);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    }


    //Toast

    function Toast(type, css, msg) {
        this.type = type;
        this.css = css;
        this.msg = msg;
    }

    var toasts = [
        new Toast('error', 'toast-top-center', 'Please check the filling.'),
        new Toast('warning', 'toast-top-center', 'warning'),
        new Toast('success', 'toast-top-center', 'success')
    ];

    //toastr.options.extendedTimeOut = 0; //1000;


    var i = 0;

    function showToast(indexMsg) {
        toastr.options.timeOut = 3000;
        toastr.options.fadeOut = 250;
        toastr.options.fadeIn = 250;
        var t = toasts[indexMsg];
        toastr.options.positionClass = t.css;
        toastr[t.type](t.msg);
    }

</script>

