@Code
    ViewData("Title") = "IATF"
End Code
<style>
    .text-folder {
        color: rgb(255,240,147);
    }
    #items{
        list-style:none;
        margin:0px;
        margin-top:4px;
        padding-left:10px;
        padding-right:10px;
        padding-bottom:3px;
        font-size:17px;
        color: #333333;
    }
    hr { width: 85%; 
        background-color:#E4E4E4;
        border-color:#E4E4E4;
        color:#E4E4E4;
    }
    #cntnr, #cntnrFile{
        display:none;
        position:fixed;
        border:1px solid #B2B2B2;
        width:170px;      background:#F9F9F9;
        box-shadow: 3px 3px 2px #E9E9E9;
        border-radius:4px;

    }
    #cntnr #items li, #cntnrFile #items li{
  
        padding: 3px;
        padding-left:10px;

    }



    #items :hover{
        color: white;
        background:#284570;
        border-radius:2px;
    }
    .tag-container
    {
        display: inline-block;
        clear: both;
        width: 100%;
        margin-top: 20px;
    }
    .doc-item
    {
        margin: 5px;
        padding-right: 0.3em;
        padding-left: 0.3em;
        padding-top: 0.1em;
        padding-bottom: 0.1em;
        background-color: white;
        border: 1px solid black;
        cursor: default;
    }
    .item-trash
    {
        font-size: 48px;
        float: right;
    }
    .item-trash-dropover
    {
        color: red;
    }
</style>
<!-- Breadcrumbs-->
<ol class="breadcrumb">
    <li class="breadcrumb-item">
        <a href="#">Main</a>
    </li>
    <li class="breadcrumb-item active">IATF</li>
</ol>
<div class="row mb-3">
    <div class="col">
        <div class="card">
            @*<div class="card-header">
                Location : <label id="idPath"></label>
            </div>*@
            <div class="card-body context-menu-one">
                <ul id="root" class="nodefile"></ul>
                
            </div>
        </div>
        @*<div class="card mb-3 mt-3">
            
            <div class="card-body">
                
            </div>
        </div>*@
    </div>
    
</div>
<div id='cntnr'>
    <ul id='items'>
        <li>New File</li>
        <li>New Folder</li>
        <li>Rename</li>
        <li>Delete</li>
    </ul>
</div>
<div id='cntnrFile'>
    <ul id='items'>
        <li>Open new tab</li>
        <li>Rename</li>
        <li>Delete</li>
    </ul>
</div>
<div class="modal" id="mdRename">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label id="idPath"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">                    
                    <div class="col-sm"> 
                        <label id="hiddenfile" hidden></label>
                        <label id="hiddenpath" hidden></label>
                                               
                        <input type="text" class="form-control" id="lbRename" placeholder="document name" value="">
                    </div>
                </div>                
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnRename" class="btn btn-success">Rename</button>
                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>
<div class="modal" id="mdNewFolder">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label id="idPath"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <label id="hiddenfile" hidden></label>
                        <label id="hiddenpath" hidden></label>

                        <input type="text" class="form-control" id="lbNewFolder" placeholder="document name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnNewFolder" class="btn btn-success">New Folder</button>
                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>
<div class="modal" id="mdDelete">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label id="idPath"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="col-sm">Name</div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <label id="hiddenfile" hidden></label>
                        <label id="hiddenpath" hidden></label>

                        <input type="text" class="form-control" id="lbDelete" placeholder="document name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnDelete" class="btn btn-danger">Delete</button>
                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>
<div class="modal" id="mdNewFile">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <!-- Modal body -->
            <div class="modal-header">
                <label id="idPath"></label>
            </div>
            <div class="modal-body">
                <div class="row mb-2">
                    <div class="col-sm">
                        <div class="custom-file">
                            <input type="file" class="custom-file-input" id="customFile">
                            <label class="custom-file-label" for="customFile" id="lbPath">Choose file</label>
                        </div>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-sm">
                        <label id="hiddenfile" hidden></label>
                        <label id="hiddenpath" hidden></label>
                        <input type="text" class="form-control" id="lbNewFile" placeholder="File name" value="">
                    </div>
                </div>
            </div>

            <!-- Modal footer -->
            <div class="modal-footer">
                <button type="button" id="btnNewFile" class="btn btn-success">New File</button>
                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
            </div>

        </div>
    </div>
</div>

<script type="text/javascript">

    
    var listArr;
    jQuery(function ($) {
        var treeview = $("#root").shieldTreeView({
            dragDrop: true,
            events: {
                select: function (e) {                    
                    listArr = this.getPath(e.element);
                },
               
                drop: function (e) {
                    var valid = e.valid;
                    if (valid) {
                        if (e.sourceNode) {
                            // dropping a treeview node - move it
                            this.append(e.sourceNode, e.targetNode);
                            console.log(treeview.getItem(e.sourceNode));
                            console.log(treeview.getItem(e.targetNode));
                        }
                        
                        // disable the animation
                        e.skipAnimation = true;
                    }
                }
            },
        }).swidget();

        $(document).bind("contextmenu", function (e) {
            e.preventDefault();
            console.log(treeview.getDataSourceIndex(listArr));
            //console.log(e.pageX + "," + e.pageY);
            var href = treeview.getItem(listArr).href;
            //console.log(href);
            //var numCut = name.indexOf('<a');
            if (typeof href !== "undefined") {
                $("#cntnrFile").css("left", e.pageX);
                $("#cntnrFile").css("top", e.pageY);               
                $("#cntnr").hide();
                $("#cntnrFile").fadeIn(200, startFocusOut());
            } else {
                $("#cntnr").css("left", e.pageX);
                $("#cntnr").css("top", e.pageY);
                $("#cntnrFile").hide();
                $("#cntnr").fadeIn(200, startFocusOut());
            }
        });

        function startFocusOut() {
            $(document).on("click", function () {
                $("#cntnr").hide();
                $("#cntnrFile").hide();
                $(document).off("click");
            });
        }

        $("#items > li").click(function () {
            var typeClick = $(this).text();
            var name = treeview.getItem(listArr).text;
            var numCut = name.indexOf('<a');
            if (numCut > 0) {
                name = name.substring(0, numCut);
            }
            var path = '';
            var arr = [];
            for (var i = 0; i < listArr.length - 1; i++) {
                
                arr[i] = listArr[i];
                path += treeview.getItem(arr).text + "/";
            }
            
            //console.log(path);
            if (typeClick == "Rename") {
                if (path == '') {
                    document.getElementById('idPath').innerHTML = "Path : Root";
                } else {
                    document.getElementById('idPath').innerHTML = "Path : " + path;
                }
                document.getElementById('hiddenpath').innerHTML = path;
                document.getElementById('hiddenfile').innerHTML = name;
                document.getElementById('lbRename').value = name;
                $("#mdRename").modal();
            } else if (typeClick == "New Folder") {
                if (path == '') {
                    document.getElementById('idPath').innerHTML = "Path : Root";
                } else {
                    document.getElementById('idPath').innerHTML = "Path : " + path;
                }
                document.getElementById('hiddenpath').innerHTML = path;
                document.getElementById('hiddenfile').innerHTML = name;
                $("#mdNewFolder").modal();

            } else if (typeClick == "Delete") {
                if (path == '') {
                    document.getElementById('idPath').innerHTML = "Path : Root";
                } else {
                    document.getElementById('idPath').innerHTML = "Path : " + path;
                }
                document.getElementById('hiddenpath').innerHTML = path;
                document.getElementById('hiddenfile').innerHTML = name;
                document.getElementById('lbDelete').value = name;
                $("#mdDelete").modal();
            } else if (typeClick == "New File") {
                if (path == '') {
                    document.getElementById('idPath').innerHTML = "Path : Root";
                } else {
                    document.getElementById('idPath').innerHTML = "Path : " + path;
                }
                document.getElementById('hiddenpath').innerHTML = path;
                document.getElementById('hiddenfile').innerHTML = name;
                $("#mdNewFile").modal();
            } else if (typeClick == "Open new tab") {
                var href = treeview.getItem(listArr).href
                window.open(href, '_blank');
                console.log(href);
            }
        });

        $("#btnRename").click(function () {
            var name = document.getElementById('hiddenfile').innerHTML;
            var path = document.getElementById('hiddenpath').innerHTML;
            var newName = document.getElementById('lbRename').value;

            $.ajax({
                type: "POST",
                url: "../Home/RenameItem",
                contentType: "application/json; charset=utf-8",
                data: "{name:'" + name + "', path:'" + path + "', newName:'" + newName + "', id:'" + treeview.getItem(listArr).cls + "'}",
                dataType: "json",
                async: false,
                success: function (data) {
                    document.getElementById('lbRename').value = '';
                    document.getElementById('hiddenfile').innerHTML = '';
                    document.getElementById('hiddenpath').innerHTML = '';
                    $('#mdRename').modal('hide');
                    getMenu();
                    treeview.refresh();
                    treeview.expanded(true, listArr)
                    console.log(listArr);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $("#btnNewFolder").click(function () {
            var name = document.getElementById('hiddenfile').innerHTML;
            var path = document.getElementById('hiddenpath').innerHTML;
            var newName = document.getElementById('lbNewFolder').value;
            console.log(treeview.getItem(listArr).cls);
            $.ajax({
                type: "POST",
                url: "../Home/fnAddFolder",
                contentType: "application/json; charset=utf-8",
                data: "{Name:'" + name + "', Path:'" + path + "', NewName:'" + newName + "', id:'" + treeview.getItem(listArr).cls + "'}",
                dataType: "json",
                async: false,
                success: function (data) {
                    document.getElementById('lbNewFolder').value = '';
                    document.getElementById('hiddenfile').innerHTML = '';
                    document.getElementById('hiddenpath').innerHTML = '';
                    $('#mdNewFolder').modal('hide');
                    getMenu();
                    treeview.refresh();
                    treeview.expanded(true, listArr)
                    console.log(listArr);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $("#btnDelete").click(function () {
            var name = document.getElementById('hiddenfile').innerHTML;
            var path = document.getElementById('hiddenpath').innerHTML;

            $.ajax({
                type: "POST",
                url: "../Home/fnDeleteItem",
                contentType: "application/json; charset=utf-8",
                data: "{Name:'" + name + "', Path:'" + path + "'}",
                dataType: "json",
                async: false,
                success: function (data) {
                    document.getElementById('lbDelete').value = '';
                    document.getElementById('hiddenfile').innerHTML = '';
                    document.getElementById('hiddenpath').innerHTML = '';
                    $('#mdDelete').modal('hide');
                    //treeview.remove(listArr);
                    //treeview.destroy();
                    getMenu();

                    //treeview.load([0])
                    treeview.refresh();
                    treeview.expanded(true, listArr)
                    //treeview.selected(listArr);
                    console.log(listArr);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });

        $('#customFile').on('change', function () {
            var fileName = $(this).val();
            document.getElementById("lbPath").innerHTML = fileName;
            document.getElementById("lbNewFile").value = document.getElementById('customFile').files[0].name;
        });

        $("#btnNewFile").click(function () {
            var name = document.getElementById('hiddenfile').innerHTML;
            var path = document.getElementById('hiddenpath').innerHTML;
            var newName = document.getElementById('lbNewFile').value;
            var strPath = document.getElementById('customFile').files[0];
            var formData = new FormData();
            formData.append("name", name);
            formData.append("path", path);
            formData.append("newName", newName);
            formData.append("strPath", strPath);

            $.ajax({
                type: "POST",
                url: "../Home/fnAddFile",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {
                    document.getElementById('lbNewFile').value = '';
                    document.getElementById('hiddenfile').innerHTML = '';
                    document.getElementById('hiddenpath').innerHTML = '';
                    $('#mdNewFile').modal('hide');
                    //treeview.remove(listArr);
                    //treeview.destroy();
                    getMenu();

                    //treeview.load([0])
                    treeview.refresh();
                    treeview.expanded(true, listArr)
                    //treeview.selected(listArr);
                    console.log(listArr);
                },
                error: function (error) {
                    alert(error);
                }
            });
        });
    });

    getMenu();

    function getMenu(){
        $.ajax({
            type: "POST",
            url: '../Home/GetMenu',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                console.log(data);
                populateUL($("#root"), data);
            },
            error: function (error) {
                alert(error);
            }
        });
    }

    function populateUL($ul, data) {
        $ul.empty();
        var hash = {
            "": { $ul: $ul }
        };
        data.forEach(function (o) {
            var $ul = $("<ul>");
            if (o.type == 1) {
                hash[o.id] = {
                    $ul: $ul,
                    $li: $("<li data-icon-cls='fas fa-file text-muted' data-href='" + o.path + "' data-class='" + o.id + "'>").text(o.name).append($ul),
                    target: "_blank"
                    //$li: $("<li data-icon-cls='fas fa-file text-muted' href='" + o.path + "'>").text(o.name).append(
                    //        $('<data>').attr({
                    //            'href': o.path,
                    //        }).append($ul)
                    //     ),
                };
                
            } else {
                hash[o.id] = {
                    $ul: $ul,
                    $li: $("<li data-icon-cls='fa fa-folder text-folder' data-class='" + o.id + "'>").text(o.name).append($ul),
                };
            }
        });
        // Append each LI element to the correct parent UL element
        data.forEach(function (o) {
            hash[o.parentDirId].$ul.append(hash[o.id].$li);
        });
        //console.log($ul);
        
    }
</script>



