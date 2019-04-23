@Code
    ViewData("Title") = "ISO"
End Code

<style>
    .icon {
        font-size: 28px;
        color: #FCC33E;
    }
    .badge-custom {
        background-color: rgb(0,79,162);
        color: #ffffff;
    }
</style>

<div class="demo-container mb-2">
    <div class="row">
        <div class="col-11">
            <div id="treeview"></div>
            <div id="context-menu"></div>
        </div>
        @*<div class="col-6">
                <div id="product-details" class="hidden">
                    <div class="name"></div>
                    <div class="price"></div>
                </div>
            </div>*@
        <div class="col-1">
            <div class="btn-group" role="group">

                <div class="btn" id="btnExpand"></div>
                <div class="btn" id="btnCompress"></div>

            </div>
        </div>

    </div>
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
                <div class="row mb-2">
                    <div class="col-sm">
                        <div id="wgDate"></div>
                    </div>                                       
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="number" min="0" step="1" class="form-control" id="lbRevision" placeholder="Revision No." value="">
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

<!--Change Date-->
<div class="modal" id="mdChangeDate">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label hidden id="idChangeDate"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Start Date</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <div id="wgDateChange"></div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <input type="number" min="0" step="1" class="form-control" id="lbRevisionChange" placeholder="Revision No." value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnChangeDate" class="btn btn-success">Save</button>
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
        document.getElementById('wgDate').value = '';
        document.getElementById('wgDateChange').value = '';
        $("#mdNewFile").modal('hide');
        $("#mdNewFolder").modal('hide');
        $("#mdChangeDate").modal('hide');
    };

    var contextMenuItemsFolder = [
        { text: 'New File' },
        { text: 'New Folder' },
        { text: 'Rename' },
        { text: 'Delete' }
    ];
    var contextMenuItemsFile = [
        { text: 'Rename' },
        { text: 'Delete' },
        { text: 'Change Date' },
    ];
    var OptionsMenu = contextMenuItemsFolder;
    var idItem = '';
    var name = '';
    var s_date;
    var firstReLoad = true;

    $(function () {


        //var permission_status = '@Session("")';
        //console.log(permission_status);
        var st_date = $("#wgDate").dxDateBox({

        }).dxDateBox('instance');
        //if (st_date._options.text == '') {
        //    console.log('test');
        //}

        var st_date_change = $("#wgDateChange").dxDateBox({

        }).dxDateBox('instance');
        function ConvertId(str) {
            //var str = '1__43__ZpPHZC__47__PLK5VEuX5l3qJk37uCBHk3ubjzyyN6xTCw__61__';
            for (i = 32; i <= 127; i++) {
                var strRep = '__' + i.toString() + '__';
                str = str.replace(new RegExp(strRep, 'g'), String.fromCharCode(i));
            }
            return str;
        }

        function showDate() {
            //console.log(treeview._options.items);
            if (firstReLoad) {
                firstReLoad = false;
                var dataHead = $(".dx-treeview-item-content");
                $(".dx-treeview-item-content")[0].innerHTML = $(".dx-treeview-item-content")[0].innerHTML + '<span class="badge badge-custom mt-1" style="float:right;font-size:12px;font-weight: normal;">Revision No.</span><span class="badge badge-custom mr-3 mt-1" style="float:right;font-size:12px;font-weight: normal;">Effective date</span>'
            }
            
            var dataNode = $(".dx-treeview-node-is-leaf");
           
            //console.log(dataNode[2].dataset.itemId);
            //console.log(dataHead);
            for (var i = 0; i < dataNode.length; i++) {
                var str = dataNode[i].innerHTML;

                if (str.indexOf("badge") == -1) {
                    var positionStart = str.indexOf("<span>");
                    var positionEndStart = str.indexOf("</span>") + 7;
                    var subStr = str.substring(positionStart, positionEndStart);
                    //console.log(ConvertId(dataNode[i].dataset.itemId));
                    var data_filter = treeview._options.items.filter(function (x) { return x.id === ConvertId(dataNode[i].dataset.itemId); })
                    //var data_filter = treeview._options.items.filter(element => element.id == ConvertId(dataNode[i].dataset.itemId))
                    //console.log(data_filter);
                    if (data_filter[0].start_date !== undefined && data_filter[0].start_date != null && data_filter[0].revision != null) {
                        if (data_filter[0].revision < 10) {
                            data_filter[0].revision = '&nbsp;&nbsp;' + data_filter[0].revision
                        }
                        $(".dx-treeview-node-is-leaf")[i].innerHTML = str.replace(subStr, subStr + '<span class="badge badge-light mr-4 mt-1" style="float:right;font-size:12px;font-weight: normal;">' + data_filter[0].revision + '</span><span class="badge badge-light mr-5 mt-1" data-toggle="tooltip" title="Effective date" style="float:right;font-size:12px;font-weight: normal;">' + moment(parseJsonDate(data_filter[0].start_date)).format('DD.MM.YYYY') + '</span>');
                    } else {
                        //$(".dx-treeview-node-is-leaf")[i].innerHTML = str.replace(subStr, subStr + '<span class="badge badge-primary ml-1 mt-1" data-toggle="tooltip" title="วันที่ประกาศใช้งาน" style="float:right;">11.20.2018</span>');
                    }
                }
            }
        }
        //showDate();
        var treeview;
        var editTreeView, scrollable;
        getMenu();
        function getMenu() {
            firstReLoad = true;
            $.ajax({
                type: "POST",
                url: '../Home/GetMenuISO',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    //for (var i = 0; i < data.length; i++) {
                    //    if (data[i].parentDirId != '' && data[i].parentDirId !== undefined) {
                    //        console.log(data[i].parentDirId);
                    //        var name = data[i].name
                    //        data[i].name = name + '<div class="ml-auto">2019-01-02</div>';
                    //    }

                    //}
                    treeview = $("#treeview").dxTreeView({
                        items: data,
                        dataStructure: "plain",
                        parentIdExpr: "parentDirId",
                        keyExpr: "id",
                        displayExpr: "name",
                        searchEnabled: true,
                        searchEditorOptions: {
                            //placeholder: "Type search value here...",
                            width: '100%',
                            elementAttr: {
                                //class: "ml-auto"
                            }
                        },
                        onItemClick: function (e) {
                            var item = e.node.itemData;
                            console.log(e);
                            if (item.path) {
                                window.open(item.path, '_blank');
                            }
                            //var item = e.itemData;
                            //if (item.path) {
                            //    $("#product-details").removeClass("hidden");
                            //    $("#product-details > .price").text("test1");
                            //    $("#product-details > .name").text("test");
                            //} else {
                            //    $("#product-details").addClass("hidden");
                            //}
                        },
                        onItemContextMenu: function (e) {
                            var item = e.node.itemData;
                            if (item.id) {
                                name = item.name
                                s_date = item.start_date;
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
                        onItemExpanded: function (e) {
                            var item = e.itemData;
                            //console.log(e);
                            showDate();


                            //ExpandedAndCollapsed(item.id, 1);
                            //console.log($(".dx-treeview-node"));
                            //$(".dx-treeview-node")[2].innerHTML = '<div class="dx-item dx-treeview-item" aria-selected="false"><div class="dx-item-content dx-treeview-item-content"><img src="../img/pdf.png" class="dx-icon"><span>File3JP.pdf</span><span class="badge badge-primary ml-2 mt-1" style="float:right;">2019-01-05</span><span class="badge badge-success mt-1" style="float:right;">2019-01-02</span></div></div>'
                        },
                        onItemCollapsed: function (e) {
                            var item = e.itemData;
                            //console.log(item);
                            //ExpandedAndCollapsed(item.id, 0);
                        },
                        onContentReady: function (e) {
                            var $btnView = $('<div id="btnExpand">').dxButton({
                                icon: 'exportpdf', //or your custom icon
                                onClick: function () {

                                }
                            });
                            if (e.element.find('#btnExpand').length == 0) {
                                e.element
                                    .find('.dx-toolbar-after')
                                    .prepend($btnView);
                                //console.log(e.element);
                            };


                            var $btnUpdate = $('<div id="btnCompress" class="mr-2">').dxButton({
                                icon: 'upload',
                                onClick: function () {

                                }
                            });
                            if (e.element.find('#btnCompress').length == 0)
                                e.element
                                    .find('before')
                                    .prepend($btnUpdate);
                        },
                        expandAllEnabled: true,
                        //onContentReady: function (e) {
                        //    editTreeView = e.component;
                        //    scrollable = editTreeView.element().find(".dx-scrollable").dxScrollable("instance");
                        //}

                    }).dxTreeView("instance");
                    showDate();
                },
                error: function (error) {
                    alert(error);
                }

            });
        }

        //var currentNode = $("#treeview").find("[data-item-id=" + "osTnSpQafYD8nYgTY2NCb5nvFZo/PXGxHHTZdy+6+QU=" + "]");
        //console.log(currentNode);
        //console.log($(".dx-treeview-node-is-leaf"));
        //showDate();

        //var str = $(".dx-treeview-node-is-leaf")[0].innerHTML
        //var positionStart = str.indexOf("<span>");
        //var positionEndStart = str.indexOf("</span>");
        //var subStr = str.substring(positionStart, positionEndStart);
        //var res = str.replace(subStr, "<span>ttttt");
        //console.log(res);
        //$(".dx-treeview-node-is-leaf")[0].innerHTML = str.replace(subStr, "<span>ttttt");//'<div class="dx-item dx-treeview-item" aria-selected="false"><div class="dx-item-content dx-treeview-item-content"><img src="../img/pdf.png" class="dx-icon"><span>File1JP.pdf</span><span class="badge badge-primary ml-2 mt-1" style="float:right;">2019-01-05</span><span class="badge badge-success mt-1" style="float:right;">2019-01-02</span></div></div>'

        //$(".dx-treeview-node")[2].innerHTML = '<div class="dx-item dx-treeview-item" aria-selected="false"><div class="dx-item-content dx-treeview-item-content"><img src="../img/pdf.png" class="dx-icon"><span>File3JP.pdf</span><span class="badge badge-primary ml-2 mt-1" style="float:right;">2019-01-05</span><span class="badge badge-success mt-1" style="float:right;">2019-01-02</span></div></div>'

        $("#btnExpand").dxButton({
            onClick: function (e) {
                treeview.expandAll();
                //editTreeView.expandItem(editTreeView.element().find(".dx-treeview-item")[0])

                //var currentNode = $("#treeview").find("[data-item-id=" + "XUlbiBF8hOpKRdTbfa42eydIc1NuhSgHt9wQI+prB1Q=" + "]");

                //$("#treeview").find(".dx-treeview-node").removeClass("dx-state-focused");
                //currentNode.focus().addClass("dx-state-focused");
                //setTimeout(function () {
                //    scrollable.scrollToElement(currentNode);
                //}, 300);
                //treeview.selectItem("XUlbiBF8hOpKRdTbfa42eydIc1NuhSgHt9wQI+prB1Q=");
            },
            icon: "spindown",
            elementAttr: {
                title: "Expand"
            }
        });
        $("#btnCompress").dxButton({
            onClick: function (e) {
                treeview.collapseAll();
                treeview.expandItem("hPZz7R9z8eTjqnuYqwXtQncB2jUzfHwuXjInN8ClIeY=");
            },
            icon: "spinup",
            elementAttr: {
                title: "Compress"
            }
        });

        //function ExpandedAndCollapsed(id, expanded) {
        //    $.ajax({
        //        type: "POST",
        //        url: '../Home/ExpandedAndCollapsed',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        data: "{id:'" + id + "', expanded: '" + expanded + "'}",
        //        async: false,
        //        success: function (data) {
        //        }
        //    });
        //}

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
                        } else if (e.itemData.text == "Change Date") {
                            document.getElementById('idChangeDate').innerHTML = idItem;
                            document.getElementById('wgDateChange').value = s_date;
                            $("#mdChangeDate").modal();
                        }

                        //DevExpress.ui.notify("The \"" + e.itemData.text + "\" item was clicked id " + idItem, "success", 1500);
                    }
                }
            });
        }

        //Buttom In Modal

        function fnNewFolder() {
            //console.log("tr");
            var newName = document.getElementById('lbNewFolder').value;
            var id = document.getElementById('idNewFolder').innerHTML;

            if (newName != '') {
                $.ajax({
                    type: "POST",
                    url: "../Home/fnNewFolderISO",
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
                    url: "../Home/fnRenameISO",
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
            var revision = document.getElementById('lbRevision').value;
            //console.log(st_date._options.text);
            if (newName != '' && strPath != '' && st_date._options.text != '') {
                var formData = new FormData();
                formData.append("newName", newName);
                formData.append("strPath", strPath);
                formData.append("id", id);
                formData.append("start_date", st_date._options.text);
                formData.append("revision", revision);
                $.ajax({
                    type: "POST",
                    url: "../Home/fnNewFileISO",
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        document.getElementById('lbNewFile').value = '';
                        document.getElementById('lbFile').innerHTML = 'Choose file';
                        document.getElementById('lbRevision').value = 0;
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
                url: "../Home/fnDeleteISO",
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

        function fnChangeDate() {
            var id = document.getElementById('idChangeDate').innerHTML;
            var revision = document.getElementById('lbRevisionChange').value;
            $.ajax({
                type: "POST",
                url: "../Home/fnChangeDateISO",
                contentType: "application/json; charset=utf-8",
                data: "{ id:'" + id + "', start_date: '" + st_date_change._options.text + "', revision: '" + revision + "' }",
                dataType: "json",
                async: false,
                success: function (data) {
                    document.getElementById('wgDateChange').value = '';
                    document.getElementById('lbRevisionChange').value = 0;
                    $('#mdChangeDate').modal('hide');
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
            //console.log(fileName);
            document.getElementById("lbFile").innerHTML = fileName;
            document.getElementById("lbNewFile").value = document.getElementById('customFile').files[0].name;
            $("#lbNewFile").focus();
        });
        //New Folder
        $("#btnNewFolder").click(function () {
            //console.log("r");
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
        //Change Date
        $("#btnChangeDate").click(function () {
            fnChangeDate();
        });
        //End Buttom In

        function parseJsonDate(jsonDateString) {
            return new Date(parseInt(jsonDateString.replace('/Date(', '')));
        }
    });
</script>
