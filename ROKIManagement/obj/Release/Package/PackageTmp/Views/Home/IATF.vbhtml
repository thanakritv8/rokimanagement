@Code
    ViewData("Title") = "IATF"
End Code

<style>
.icon {
    font-size: 28px;
    color: #FCC33E;
}
</style>

<div class="demo-container mb-2">
    <div id="treeview"></div>
    <div id="context-menu"></div> 
</div>

<!--Rename-->
<div class="modal" id="mdRename">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label hidden id="idRename"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="text" class="form-control" id="lbRename" placeholder="document name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnRename" class="btn btn-success">Rename</button>
                <button type="button" id="btnClose" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>
<!--New Folder-->
<div class="modal" id="mdNewFolder">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label hidden id="idNewFolder"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="text" class="form-control" id="lbNewFolder" placeholder="document name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnNewFolder" class="btn btn-success">New Folder</button>
                <button type="button" onclick="clearModal();" class="btn btn-danger">Close</button>
            </div>

        </div>
    </div>
</div>
<!--Delete-->
<div class="modal" id="mdDelete">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label hidden id="idDelete"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="text" class="form-control" id="lbDelete" placeholder="document name" disabled value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnDelete" class="btn btn-danger">Delete</button>
                <button type="button" id="btnClose" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>
<!--New File-->
<div class="modal" id="mdNewFile">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label hidden id="idNewFile"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="custom-file">
                            <input type="file" class="custom-file-input" id="customFile">
                            <label class="custom-file-label" for="customFile" id="lbFile">Choose file</label>
                        </div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="text" class="form-control" id="lbNewFile" placeholder="File name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnNewFile" class="btn btn-success">New File</button>
                <button type="button" onclick="clearModal();" class="btn btn-danger">Close</button>
            </div>

        </div>
    </div>
</div>
<script>
    function clearModal() {
        document.getElementById('lbFile').innerHTML = 'Choose file';
        document.getElementById('lbNewFile').value = '';
        document.getElementById('lbNewFolder').value = '';
        $("#mdNewFile").modal('hide');
        $("#mdNewFolder").modal('hide');
    };

    var contextMenuItemsFolder = [
        { text: 'New File' },
        { text: 'New Folder' },
        { text: 'Rename' },
        { text: 'Delete' }
    ];
    var contextMenuItemsFile = [
        { text: 'Rename' },
        { text: 'Delete' }
    ];
    var OptionsMenu = contextMenuItemsFolder;
    var idItem = '';
    var name = '';

    $(function () {
        getMenu();
        function getMenu() {
            $.ajax({
                type: "POST",
                url: '../Home/GetMenuIATF',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    var treeview = $("#treeview").dxTreeView({
                        items: data,
                        dataStructure: "plain",
                        parentIdExpr: "parentDirId",
                        keyExpr: "id",
                        displayExpr: "name",
                        searchEnabled: true,
                        onItemClick: function (e) {
                            var item = e.itemData;
                            console.log(e);
                            if (item.path) {
                                window.open(item.path, '_blank');
                            }
                        },
                        onItemContextMenu: function (e) {
                            var item = e.itemData;
                            if (item.id) {
                                name = item.name
                                idItem = item.id;
                                if(item.type == 1)
                                {
                                    OptionsMenu = contextMenuItemsFile;
                                } else {
                                    OptionsMenu = contextMenuItemsFolder;
                                }
                                getContextMenu();
                            }
                        },
                    }).dxTreeView("instance");
                },
                error: function (error) {
                    alert(error);
                }
            });
        }

        getContextMenu();
        function getContextMenu() {
            $("#context-menu").dxContextMenu({
                dataSource: OptionsMenu,
                width: 200,
                target: "#treeview",
                onItemClick: function (e) {
                    if (!e.itemData.items) {
                        if (e.itemData.text == "Rename") {
                            document.getElementById('idRename').innerHTML = idItem;
                            document.getElementById('lbRename').value = name;
                            $("#mdRename").modal();
                            $("#lbRename").focus();
                        } else if (e.itemData.text == "New File") {
                            document.getElementById('idNewFile').innerHTML = idItem;
                            $("#mdNewFile").modal();
                            $("#lbNewFile").focus();
                        } else if (e.itemData.text == "New Folder") {
                            document.getElementById('idNewFolder').innerHTML = idItem;
                            $("#mdNewFolder").modal();
                            $("#lbNewFolder").focus();
                        } else if (e.itemData.text == "Delete") {
                            document.getElementById('idDelete').innerHTML = idItem;
                            document.getElementById('lbDelete').value = name;
                            $("#mdDelete").modal();
                        }

                        //DevExpress.ui.notify("The \"" + e.itemData.text + "\" item was clicked id " + idItem, "success", 1500);
                    }
                }
            });
        }

        //Buttom In Modal

        function fnNewFolder() {
            var newName = document.getElementById('lbNewFolder').value;
            var id = document.getElementById('idNewFolder').innerHTML;
            if (newName != '') {
                $.ajax({
                    type: "POST",
                    url: "../Home/fnNewFolder",
                    contentType: "application/json; charset=utf-8",
                    data: "{NewName:'" + newName + "', id:'" + id + "'}",
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        document.getElementById('lbNewFolder').value = '';
                        $('#mdNewFolder').modal('hide');
                        getMenu();
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            } else {
                $('#mdNewFolder').modal('hide');
                alert("กรุณากรอกข้อมูลให้ครบ");
            }
        }
        function fnRename() {
            var newName = document.getElementById('lbRename').value;
            var id = document.getElementById('idRename').innerHTML;
            if (newName != "") {
                $.ajax({
                    type: "POST",
                    url: "../Home/fnRename",
                    contentType: "application/json; charset=utf-8",
                    data: "{newName:'" + newName + "', id:'" + id + "'}",
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        document.getElementById('lbRename').value = '';
                        $('#mdRename').modal('hide');
                        getMenu();
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            } else {
                $('#mdNewFolder').modal('hide');
                alert("กรุณากรอกข้อมูลให้ครบ");
            }
        }
        function fnNewFile() {
            var newName = document.getElementById('lbNewFile').value;
            var strPath = document.getElementById('customFile').files[0];
            var id = document.getElementById('idNewFile').innerHTML;
            console.log(strPath);
            if (newName != '' && strPath != '') {
                var formData = new FormData();
                formData.append("newName", newName);
                formData.append("strPath", strPath);
                formData.append("id", id);
                $.ajax({
                    type: "POST",
                    url: "../Home/fnNewFile",
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        document.getElementById('lbNewFile').value = '';
                        document.getElementById('lbFile').innerHTML = 'Choose file';
                        $('#mdNewFile').modal('hide');
                        getMenu();
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            } else {
                $('#mdNewFile').modal('hide');
                alert("กรุณากรอกข้อมูลให้ครบ");
            }
        }
        function fnDelete() {
            var id = document.getElementById('idDelete').innerHTML;
            $.ajax({
                type: "POST",
                url: "../Home/fnDelete",
                contentType: "application/json; charset=utf-8",
                data: "{ idDel:'" + id + "' }",
                dataType: "json",
                async: false,
                success: function (data) {
                    document.getElementById('lbDelete').value = '';
                    $('#mdDelete').modal('hide');
                    getMenu();
                },
                error: function (error) {
                    alert(error);
                }
            });
        }

        $("#lbNewFolder").keypress(function (e) {
            if (event.which == 13) {
                fnNewFolder();
            }
        });
        $("#lbRename").keypress(function (e) {
            if (event.which == 13) {
                fnRename();
            }
        });
        
        $("#lbNewFile").keypress(function (e) {
            if (event.which == 13) {
                fnNewFile();
            }
        });

        $('#customFile').on('change', function () {
            var fileName = $(this).val();
            console.log(fileName);
            document.getElementById("lbFile").innerHTML = fileName;
            document.getElementById("lbNewFile").value = document.getElementById('customFile').files[0].name;
            $("#lbNewFile").focus();
        });
        //New Folder
        $("#btnNewFolder").click(function () {
            fnNewFolder();
        });
        //Rename
        $("#btnRename").click(function () {
            fnRename();
        });
        //New File
        $("#btnNewFile").click(function () {
            fnNewFile();
        });
        //Delete
        $("#btnDelete").click(function () {
            fnDelete();
        });
        //End Buttom In Modal
    });
    

    
</script>
