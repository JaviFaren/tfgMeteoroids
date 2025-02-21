<?php
    $con = mysqli_connect('fdb1028.awardspace.net', '4591675_tfgdb','tfg_Placeholder741' , '4591675_tfgdb');

    $user = $_POST["usuario"];
    $password = $_POST["password"];

    $sql = "SELECT ID_user,Nickname,Password FROM usuario WHERE Nickname = '$user' and Password = '$password';";
    $result = mysqli_query($con, $sql);

    if(mysqli_connect_errno()){
        echo "Connection failed" . $mysql_connect_error();
        exit();
    }
    $total = mysqli_num_rows($result);

    if($total == 1){
        
        echo "exito";
    }
    else{
        $sql = "SELECT ID_user,Nickname FROM usuario WHERE Nickname = '$user';";
        echo "usuario o contraseña erronea";
    }
?>