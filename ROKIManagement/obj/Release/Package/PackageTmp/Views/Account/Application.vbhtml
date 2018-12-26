@Code
    ViewData("Title") = "Application"
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
    <li class="breadcrumb-item active">Application</li>
</ol>

<div class="card mb-3">
    <div class="card-header">
        <i class="fas fa-layer-group"></i>
        Select Group
    </div>

    <div class="card-body">
        <select onchange="GroupChange()" class="form-control" id="nameGroup">
            
        </select>
    </div>
</div>


<!-- DataTables -->
<div class="card mb-3">
    <div class="card-header">
        <i class="far fa-check-square"></i>
        Checked Application
    </div>

    <div class="card-body">
        <div class="table-responsive">
            
            <table class="table table-bordered" id="dataTable" cellspacing="0">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th>Application</th>
                        <th>Checked</th>
                    </tr>
                </thead>
                <tbody id="dataRow"></tbody>
            </table>
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
    getApplication(1);
    getGroup();
    

    function GroupChange() {
        var groupid = document.getElementById("nameGroup").value;
        getApplication(groupid);
    }

    function getGroup() {
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
                
                $('#nameGroup').html(tblSomething);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Hey, something went wrong because: ' + errorThrown);
            }
        });
    }

    function checkAll(ele) {
        var checkboxes = document.getElementsByTagName('input');
        
        if (ele.checked) {
            var appid = ele.value;
            var groupid = document.getElementById('nameGroup').value
            $.ajax({
                type: "POST",
                url: "../Account/UploadApplication",
                contentType: "application/json; charset=utf-8",
                data: "{AppId:'" + appid + "', GroupId:'" + groupid + "'}",
                dataType: "json",
                success: function (data) {
                    
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Hey, something went wrong because: ' + errorThrown);
                }
            });
        } else {
            var appid = ele.value;
            var groupid = document.getElementById('nameGroup').value
            $.ajax({
                type: "POST",
                url: "../Account/DeleteApplication",
                contentType: "application/json; charset=utf-8",
                data: "{AppId:'" + appid + "', GroupId:'" + groupid + "'}",
                dataType: "json",
                success: function (data) {

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Hey, something went wrong because: ' + errorThrown);
                }
            });
        }
    }

    function getApplication(groupid) {
        //$.ajax({
        //    type: "POST",
        //    url: "../Account/GetApplication",
        //    contentType: "application/json; charset=utf-8",
        //    data: "{GroupId:'" + groupid + "'}",
        //    dataType: "json",
        //    success: function (data) {
        //        var tblSomething = '';
        //        $.each(data, function (idx, obj) {
        //            tblSomething += '<tr>';
        //            $.each(obj, function (key, value) {
        //                tblSomething += "<td>12</td>";
        //            });
        //            tblSomething += '</tr>';
        //        });
        //        console.log(tblSomething);
        //        $('#dataRow').html(tblSomething);
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        alert('Hey, something went wrong because: ' + errorThrown);
        //    }
        //});
        $.ajax({
            type: "POST",
            url: "../Account/GetApplication",
            contentType: "application/json; charset=utf-8",
            data: "{GroupId:'" + groupid + "'}",
            dataType: "json",
            success: function (data) {

                var i = 1;
                var tblSomething = '';
                $.each(data, function (idx, obj) {
                    tblSomething += '<tr>';
                    var AppId = 0;
                    var ck = 0;
                    tblSomething += "<td>" + i + "</td>";
                    $.each(obj, function (key, value) {
                        if (key == "nameApp") {
                            tblSomething += "<td>" + value + "</td>";
                        } else if (key == "AppId") {
                            AppId = value;
                        } else if (key == "ck") {
                            ck = value;
                        }
                    });
                    if (ck > 0) {

                        //tblSomething += "<td>12</td>";
                        tblSomething += "<td><div class='form-check'><input checked class='form-check-input' type='checkbox' value='" + AppId + "' onchange='checkAll(this)' name='chk[]'> </div> </td>";
                    } else {
                        //tblSomething += "<td>12</td>";
                        tblSomething += "<td><div class='form-check'><input class='form-check-input' type='checkbox' value='" + AppId + "' onchange='checkAll(this)' name='chk[]'> </div> </td>";
                    }

                    tblSomething += '</tr>';
                    i++;
                });
                //console.log(tblSomething);
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

