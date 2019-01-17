@Code
    ViewData("Title") = "IATFFile"
End Code
@Model IEnumerable<FileTreeViewModel>
    <ul class="jqueryFileTree">
        @For Each item In Model
            If item.IsDirectory Then
                @<li Class="directory collapsed">
                    <a href = "#" rel="@item.PathAltSeparator()">@item.Name</a>
                </li>
            Else
                @<li class="file ext_@item.Ext">
                    <a href="#" rel="@item.PathAltSeparator()">@item.Name</a>
                </li>
            End If
        Next
    </ul>

