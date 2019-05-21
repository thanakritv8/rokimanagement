@Code
    ViewData("Title") = "QMS"
End Code
<style>
    
    #imglogo {
        opacity: 0.2;
        filter: alpha(opacity=50); /* For IE8 and earlier */
    }
</style>
@*<div class="bg-success">
    <img id="imglogo" src="~/img/index.png" alt="Responsive image">
</div>*@
<script>
    $(".d1").next().toggle();
    $(".d1").click(function (e) {
        e.stopPropagation();
        $(".d1").next().toggle();
    });
    $(".d2").next().toggle();
    $(".d2").click(function (e) {
        e.stopPropagation();
        $(".d2").next().toggle();
    });
    $(".d3").next().toggle();
    $(".d3").click(function (e) {
        e.stopPropagation();
        $(".d3").next().toggle();
    });

</script>

