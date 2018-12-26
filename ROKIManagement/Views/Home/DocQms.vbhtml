@Code
    ViewData("Title") = "QMS"
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
    <li class="breadcrumb-item active">QMS</li>
</ol>

<!-- DataTables -->
<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-table"></i>
        Document QMS
        @If (Session("GroupId") = 1 Or Session("GroupId") = 8) Then @<button type="button" class="btn btn-success btn-sm float-sm-right" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Add" data-target="#docAdd"><i class="fas fa-plus-circle"></i></button> End If
        
    </div>

    <div class="card-body">
        <div class="table-responsive">
            <table class="table table-bordered" id="dataTable" cellspacing="0">
                <thead>
                    <tr>
                        <th>Document Name(JP)</th>
                        <th>Document No.(JP)</th>
                        <th>Document Name(TH)</th>
                        <th>Document No.(TH)</th>
                        <th>Update By</th>
                        <th>Update Last</th>
                        @if(Session("GroupId") = 1 Or Session("GroupId") = 8) Then @<th>Action</th> End if
                    </tr>
                </thead>
                <tbody id="dataRow">
                </tbody>
            </table>

            <!-- The Modal docAdd-->
            <div class="modal" id="docAdd">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">
                            <h4 class="modal-title">Document Add</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-header">
                                    JP
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <div class="custom-file">
                                                <input type="file" class="custom-file-input" id="customFileJP">
                                                <label class="custom-file-label" for="customFile" id="lbPathJP">Choose file</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameDocJP" placeholder="document name" value="" required>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="noDocJP" placeholder="document no." value="" required>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card">
                                <div class="card-header">
                                    TH
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <div class="custom-file">
                                                <input type="file" class="custom-file-input" id="customFileTH">
                                                <label class="custom-file-label" for="customFile" id="lbPathTH">Choose file</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameDocTH" placeholder="document name" value="" required>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="noDocTH" placeholder="document no." value="" required>
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
            <div class="modal" id="docEdit">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">
                            <h4 class="modal-title">Document Edit ID <label id="seq"></label></h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-header">
                                    JP
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <div class="custom-file">
                                                <input type="file" class="custom-file-input" id="customFileJPe">
                                                <label class="custom-file-label" for="customFile" id="lbPathJPe">Choose file</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameDocJPe" placeholder="document name" value="" required>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="noDocJPe" placeholder="document no." value="" required>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card">
                                <div class="card-header">
                                    TH
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <div class="custom-file">
                                                <input type="file" class="custom-file-input" id="customFileTHe">
                                                <label class="custom-file-label" for="customFile" id="lbPathTHe">Choose file</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="nameDocTHe" placeholder="document name" value="" required>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" style="background-color: #FCFBC8" id="noDocTHe" placeholder="document no." value="" required>
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
            <div class="modal" id="docDel">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header">                            
                            <h4 class="modal-title">Document Delete ID <label id="seqDel"></label></h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>

                        <!-- Modal body -->
                        <div class="modal-body">
                            <div class="card mb-2">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col ml-3">
                                            <input class="form-check-input" type="checkbox" value="" id="cbDelJP" checked>JP
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" id="nameDocJPd" placeholder="document name" value="" readonly>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" id="noDocJPd" placeholder="document no." value="" readonly>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col ml-3">
                                            <input class="form-check-input" type="checkbox" value="" id="cbDelTH" checked>TH
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-sm">
                                            <input type="text" class="form-control" id="nameDocTHd" placeholder="document name" value="" readonly>
                                        </div>
                                        <div class="col-sm">
                                            <input type="text" class="form-control" id="noDocTHd" placeholder="document no." value="" readonly>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </div>

                        <!-- Modal footer -->
                        <div class="modal-footer">
                            <div class="text-left text-danger">กรุณาเลือกเอกสารที่ต้องการลบ</div>
                            <p> </p>
                            <div class="row">
                                <div class="col"> </div>
                                <div class="col"> </div>
                            </div>
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
            var strPathJP = document.getElementById('customFileJP').files[0];
            var strNameJP = document.getElementById('nameDocJP').value;
            var strNoJP = document.getElementById('noDocJP').value;
            var strPathTH = document.getElementById('customFileTH').files[0];
            var strNameTH = document.getElementById('nameDocTH').value;
            var strNoTH = document.getElementById('noDocTH').value;

            if (strNameJP == '' && strNameTH == '') {
                showToast(0);
            } else {
                formData.append("pathJP", strPathJP);
                formData.append("nameJP", strNameJP);
                formData.append("noJP", strNoJP);
                formData.append("pathTH", strPathTH);
                formData.append("nameTH", strNameTH);
                formData.append("noTH", strNoTH);

                $.ajax({
                    type: "POST",
                    url: '../Home/UploadDocQMS',
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        document.getElementById('lbPathJP').innerHTML = 'Choose file';
                        document.getElementById('nameDocJP').value = '';
                        document.getElementById('noDocJP').value = '';
                        document.getElementById('lbPathTH').innerHTML = 'Choose file';
                        document.getElementById('nameDocTH').value = '';
                        document.getElementById('noDocTH').value = '';

                        getDoc();
                        showToast(2);
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            }


        });

        $("#btnEdit").click(function () {
            var formData = new FormData();
            var strPathJP = document.getElementById('customFileJPe').files[0];
            var strNameJP = document.getElementById('nameDocJPe').value;
            var strNoJP = document.getElementById('noDocJPe').value;
            var strPathTH = document.getElementById('customFileTHe').files[0];
            var strNameTH = document.getElementById('nameDocTHe').value;
            var strNoTH = document.getElementById('noDocTHe').value;
            var seq = document.getElementById('seq').innerHTML;
            console.log(seq);
            formData.append("pathJP", strPathJP);
            formData.append("nameJP", strNameJP);
            formData.append("noJP", strNoJP);
            formData.append("pathTH", strPathTH);
            formData.append("nameTH", strNameTH);
            formData.append("noTH", strNoTH);
            formData.append("seq", seq);

            $.ajax({
                type: "POST",
                url: '../Home/UpDateDocQMS',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('lbPathJPe').innerHTML = 'Choose file';
                    document.getElementById('nameDocJPe').value = '';
                    document.getElementById('noDocJPe').value = '';
                    document.getElementById('lbPathTHe').innerHTML = 'Choose file';
                    document.getElementById('nameDocTHe').value = '';
                    document.getElementById('noDocTHe').value = '';
                    document.getElementById('seq').innerHTML = '';
                    getDoc();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $("#btnDel").click(function () {
            var formData = new FormData();
            var cbDelJP = document.getElementById('cbDelJP').checked;
            var cbDelTH = document.getElementById('cbDelTH').checked;
            var seq = document.getElementById('seqDel').innerHTML;
            console.log(seq + '=>' + cbDelJP + '=>' + cbDelTH);
            formData.append("cbDelJP", cbDelJP);
            formData.append("cbDelTH", cbDelTH);
            formData.append("seq", seq);

            $.ajax({
                type: "POST",
                url: '../Home/DelDocQMS',
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                    document.getElementById('cbDelJP').checked = true;
                    document.getElementById('cbDelTH').checked = true;
                    document.getElementById('seqDel').innerHTML = '';
                    getDoc();
                    showToast(2);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });
    });

    getDoc();

    $('#customFileTH').on('change', function () {
        var fileName = $(this).val();
        document.getElementById("lbPathTH").innerHTML = fileName;
        document.getElementById("nameDocTH").value = document.getElementById('customFileTH').files[0].name.substring(0, document.getElementById('customFileTH').files[0].name.indexOf('.'));
    })

    $('#customFileJP').on('change', function () {
        var fileName = $(this).val();
        document.getElementById("lbPathJP").innerHTML = fileName;
        document.getElementById("nameDocJP").value = document.getElementById('customFileJP').files[0].name.substring(0, document.getElementById('customFileJP').files[0].name.indexOf('.'));
    })

    $('#customFileTHe').on('change', function () {
        var fileName = $(this).val();
        document.getElementById("lbPathTHe").innerHTML = fileName;
        document.getElementById("nameDocTHe").value = document.getElementById('customFileTHe').files[0].name.substring(0, document.getElementById('customFileTHe').files[0].name.indexOf('.'));
    })

    $('#customFileJPe').on('change', function () {
        var fileName = $(this).val();
        document.getElementById("lbPathJPe").innerHTML = fileName;
        document.getElementById("nameDocJPe").value = document.getElementById('customFileJPe').files[0].name.substring(0, document.getElementById('customFileJPe').files[0].name.indexOf('.'));
    })

    $('#docEdit').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seq").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Home/GetDocWithSeq",
            contentType: "application/json; charset=utf-8",
            data: "{Kind:'QMS', Seq:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, obj) {
                    var strNameJP = "";
                    var strNameTH = "";
                    var strPathJP = "";
                    var strPathTH = "";
                    var strNoJP = "";
                    var strNoTH = "";
                    var seq = "";
                    $.each(obj, function (key, value) {
                        if (key == "nameFileJP") {
                            strNameJP = value;
                            document.getElementById("nameDocJPe").value = value;
                        } else if (key == "nameFileTH") {
                            strNameTH = value;
                            document.getElementById("nameDocTHe").value = value;
                        } else if (key == "pathFileJP") {
                            strPathJP = value;
                        } else if (key == "pathFileTH") {
                            strPathTH = value;
                        } else if (key == "seq") {
                            seq = value;
                        } else if (key == "docNoJP") {
                            strNoJP = value;
                            document.getElementById("noDocJPe").value = value;
                        } else if (key == "docNoTH") {
                            strNoTH = value;
                            document.getElementById("noDocTHe").value = value;
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });

    });

    $('#docDel').on('show.bs.modal', function (e) {
        var bookId = $(e.relatedTarget).data('book-id');
        document.getElementById("seqDel").innerHTML = bookId;
        $.ajax({
            type: "POST",
            url: "../Home/GetDocWithSeq",
            contentType: "application/json; charset=utf-8",
            data: "{Kind:'QMS', Seq:'" + bookId + "'}",
            dataType: "json",
            success: function (data) {
                $.each(data, function (idx, obj) {
                    var strNameJP = "";
                    var strNameTH = "";
                    var strNoJP = "";
                    var strNoTH = "";
                    var seq = "";
                    $.each(obj, function (key, value) {
                        if (key == "nameFileJP") {
                            strNameJP = value;
                            document.getElementById("nameDocJPd").value = value;
                        } else if (key == "nameFileTH") {
                            strNameTH = value;
                            document.getElementById("nameDocTHd").value = value;
                        } else if (key == "seq") {
                            seq = value;
                        } else if (key == "docNoJP") {
                            strNoJP = value;
                            document.getElementById("noDocJPd").value = value;
                        } else if (key == "docNoTH") {
                            strNoTH = value;
                            document.getElementById("noDocTHd").value = value;
                        }
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    });

    function getDoc() {
        $.ajax({
            type: "POST",
            url: "../Home/GetDoc",
            contentType: "application/json; charset=utf-8",
            data: "{Kind:'QMS'}",
            dataType: "json",
            success: function (data) {
                var tblSomething = '';

                $.each(data, function (idx, obj) {
                    tblSomething += '<tr>';
                    var strNameJP = "";
                    var strNameTH = "";
                    var strPathJP = "";
                    var strPathTH = "";
                    var seq = "";
                    $.each(obj, function (key, value) {
                        //console.log(key);
                        if (key == "nameFileJP") {
                            strNameJP = value;
                            tblSomething += "<td><a href='" + strPathJP + "' target='_blank'>" + strNameJP + "</a></td>";
                        } else if (key == "nameFileTH") {
                            strNameTH = value;
                            tblSomething += "<td><a href='" + strPathTH + "' target='_blank'>" + strNameTH + "</a></td>";
                        } else if (key == "pathFileJP") {
                            strPathJP = value;
                        } else if (key == "pathFileTH") {
                            strPathTH = value;
                        } else if (key == "seq") {
                            seq = value;
                        } else {
                            tblSomething += "<td>" + value + "</td>";
                        }

                    });
                    var sessiongroupid = @Session("GroupId")
                    console.log(sessiongroupid);
                    if(sessiongroupid == 1 || sessiongroupid == 8){
                        tblSomething += '<td class="text-center"><div class="btn-group"><button type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Edit" data-target="#docEdit" data-book-id=' + seq + '><i class="fas fa-edit"></i></button><button type="button" class="btn btn-danger btn-sm" data-tooltip="tooltip" data-placement="right" title="Delete" data-toggle="modal" data-target="#docDel" data-book-id=' + seq + '><i class="fas fa-trash-alt"></i></button></div></td>'
                    }else{
                        //tblSomething += '<td><div class="btn-group"><button disabled type="button" class="btn btn-warning btn-sm" data-toggle="modal" data-tooltip="tooltip" data-placement="right" title="Edit" data-target="#docEdit" data-book-id=' + seq + '><i class="fas fa-edit"></i></button><button disabled type="button" class="btn btn-danger btn-sm" data-tooltip="tooltip" data-placement="right" title="Delete" data-toggle="modal" data-target="#docDel" data-book-id=' + seq + '><i class="fas fa-trash-alt"></i></button></div></td>'
                    }
                    
                    tblSomething += '</tr>';

                });
                $('#dataRow').html(tblSomething);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    }

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

