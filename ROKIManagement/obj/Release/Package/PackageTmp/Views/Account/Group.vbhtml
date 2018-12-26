@Code
    ViewData("Title") = "Group"
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
    <li class="breadcrumb-item active">Group</li>
</ol>

<!-- DataTables -->
<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-table"></i>
        Config Group
        <button type="button" class="btn btn-success btn-sm float-sm-right" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Add" data-target="#groupAdd"><i class="fas fa-plus-circle"></i></button>
    </div>

    <div class="card-body">
        <div class="table-responsive">
            <table class="table table-bordered" id="dataTable" cellspacing="0">
                 <thead>
                     <tr>
                         <th>Group Name</th>
                         <th>Create Date</th>
                         <th>Create By</th>
                         <th>Update Date</th>
                         <th>Update By</th>
                         <th>Remark</th>
                         <th>Action</th>
                     </tr>
                 </thead>
                 <tbody id="dataRow"></tbody>
             </table>
        </div>
        
        <!-- The Modal docAdd-->
        <div class="modal" id="groupAdd">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Group Add</h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="card mb-2">
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-sm">
                                        <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameGroupa" placeholder="group name" value="" required>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-sm">
                                        <textarea class="form-control" style="background-color: #FCFBC8" id="remarka" rows="3" placeholder="remark"></textarea>
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
        <div class="modal" id="groupEdit">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Group Edit ID <label id="seq"></label></h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="card mb-2">
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-sm">
                                        <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameGroupe" placeholder="group name" value="" required>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-sm">
                                        <textarea class="form-control" style="background-color: #FCFBC8" id="remarke" rows="3" placeholder="remark"></textarea>
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
        <div class="modal" id="groupDel">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">

                    <!-- Modal Header -->
                    <div class="modal-header">
                        <h4 class="modal-title">Group Delete ID <label id="seqDel"></label></h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>

                    <!-- Modal body -->
                    <div class="modal-body">
                        <div class="card mb-2">
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-sm">
                                        <input type="text" class="form-control" id="nameGroupd" placeholder="group name" value="" readonly>
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
            var namegroup = document.getElementById('nameGroupa').value;
            var remark = document.getElementById('remarka').value;
            formData.append("nameGroup", namegroup);
            formData.append("remark", remark);
            $.ajax({
                type: "POST",
                url: '../Account/UploadGroup',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('nameGroupa').value = '';
                    document.getElementById('remarka').value = '';
                    getGroup();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $("#btnEdit").click(function () {
            var formData = new FormData();
            var nameGroup = document.getElementById('nameGroupe').value;
            var remark = document.getElementById('remarke').value;
            var seq = document.getElementById('seq').innerHTML;
            console.log(seq);
            formData.append("GroupId", seq);
            formData.append("nameGroup", nameGroup);
            formData.append("remark", remark);
           
            $.ajax({
                type: "POST",
                url: '../Account/UpDateGroup',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('nameGroupe').value = '';
                    document.getElementById('remarke').value = '';
                    document.getElementById('seq').innerHTML = '';
                    getGroup();
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
            formData.append("GroupId", seq);

            $.ajax({
                type: "POST",
                url: '../Account/DelGroup',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('seqDel').innerHTML = '';
                    getGroup();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });
    });

    getGroup();

    $('#groupEdit').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seq").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Account/GetGroupWithGroupId",
            contentType: "application/json; charset=utf-8",
            data: "{GroupId:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, obj) {
                    
                    $.each(obj, function (key, value) {
                        if (key == "nameGroup") {
                            document.getElementById("nameGroupe").value = value;
                        } else if (key == "remark") {
                            document.getElementById("remarke").value = value;
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });

    });

    $('#groupDel').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seqDel").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Account/GetGroupWithGroupId",
            contentType: "application/json; charset=utf-8",
            data: "{ GroupId:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, obj) {
                    $.each(obj, function (key, value) {
                        if (key == "nameGroup") {
                            document.getElementById("nameGroupd").value = value;
                        } 
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    });

    function getGroup() {
        $.ajax({
            type: "POST",
            url: "../Account/GetGroup",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var tblSomething = '';
                $.each(data, function (idx, obj) {
                    tblSomething += '<tr>';
                    var GroupId = 0;
                    $.each(obj, function (key, value) {
                        if (key == "nameGroup") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "createDate") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "createBy") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "updateDate") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "updateBy") {
                            var updateBy = '';
                            if (value != 0) {
                                updateBy = value
                            }
                            tblSomething += "<td>" + updateBy + "</td>";
                        } else if (key == "remark") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "GroupId") {
                            GroupId = value;
                        }
                    });

                    tblSomething += '<td class="text-center"><div class="btn-group"><button type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Edit" data-target="#groupEdit" data-book-id=' + GroupId + '><i class="fas fa-edit"></i></button><button type="button" class="btn btn-danger btn-sm" data-tooltip="tooltip" data-placement="right" title="Delete" data-toggle="modal" data-target="#groupDel" data-book-id=' + GroupId + '><i class="fas fa-trash-alt"></i></button></div></td>'
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

