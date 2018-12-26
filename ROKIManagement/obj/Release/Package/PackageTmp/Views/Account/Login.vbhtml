@Code
    ViewData("Title") = "Login"
End Code

<div class="card card-login mx-auto mt-5">
    <div class="card-header">Login</div>
    <div class="card-body">
        
            <div class="form-group">
                <div class="form-label-group">
                    <input type="text" id="txtUsername" class="form-control" placeholder="Username"autofocus="autofocus">
                    <label for="txtUsername">Username</label>
                </div>
            </div>
            <div class="form-group">
                <div class="form-label-group">
                    <input type="password" id="txtPassword" class="form-control" placeholder="Password">
                    <label for="txtPassword">Password</label>
                </div>
            </div>
            <div class="form-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" value="remember-me">
                        Remember Password
                    </label>
                </div>
            </div>
            <button class="btn btn-primary btn-block" id="btnLogin">Login</button>
            <div class="text-danger text-center mt-3" id="lbError"></div>

        
        <div class="text-center">
            <a class="d-block small mt-3" href="register.html">Register an Account</a>
            <a class="d-block small" href="forgot-password.html">Forgot Password?</a>
        </div>
    </div>
</div>
<script>
    $("#btnLogin").click(function () {
        var strUsername = document.getElementById('txtUsername').value;
        var strPassword = document.getElementById('txtPassword').value;
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
    });
</script>

