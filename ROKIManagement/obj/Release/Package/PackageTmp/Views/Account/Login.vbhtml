@Code
    ViewData("Title") = "Login"
End Code

<div class="card card-login mx-auto mt-5">
    <div class="card-header">Login</div>
    <div class="card-body">
        <input type="text" id="txtUsername" class="form-control mb-4" placeholder="Username" autofocus="autofocus">
        <input type="password" id="txtPassword" class="form-control mb-4" placeholder="Password">
        <button class="btn btn-primary btn-block" id="btnLogin">Login</button>
        <div class="text-danger text-center mt-3" id="lbError"></div>

        
        @*<div class="text-center">
            <a class="d-block small mt-3" href="register.html">Register an Account</a>
            <a class="d-block small" href="forgot-password.html">Forgot Password?</a>
        </div>*@
    </div>
</div>
<script>
    $("#btnLogin").click(function () {
        var strUsername = document.getElementById('txtUsername').value;
        var strPassword = document.getElementById('txtPassword').value;
        $.ajax({
            type: "POST",
            url: "../dms/Account/CheckLogin",
            contentType: "application/json; charset=utf-8",
            data: "{Username:'" + strUsername + "', Password:'" + strPassword + "'}",
            dataType: "json",
            success: function (data) {
                document.getElementById('txtUsername').value = '';
                document.getElementById('txtPassword').value = '';
                if (data != '') {
                    window.location.href = '../dms/Home/Index';
                } else {
                    document.getElementById('lbError').innerHTML = "Please check the information."
                }

            }, error: function (xhr, status, error) {
                $.ajax({
                    type: "POST",
                    url: "../Account/CheckLogin",
                    contentType: "application/json; charset=utf-8",
                    data: "{Username:'" + strUsername + "', Password:'" + strPassword + "'}",
                    dataType: "json",
                    success: function (data) {
                        document.getElementById('txtUsername').value = '';
                        document.getElementById('txtPassword').value = '';
                        if (data != '') {
                            window.location.href = '../Home/Index';
                        } else {
                            document.getElementById('lbError').innerHTML = "Please check the information."
                        }
                    }
                });
            }
        });
    });
</script>

