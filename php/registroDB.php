<?php
    $con = mysqli_connect('fdb1028.awardspace.net', '4591675_tfgdb','tfg_Placeholder741' , '4591675_tfgdb');

    $user = $_POST["usuario"];
    $password = $_POST["password"];
    $email = $_POST["correo"];

    $sql = "SELECT ID_user,Nickname,Password,email FROM usuario WHERE Nickname = '$user';";
    $result = mysqli_query($con, $sql);

    if(mysqli_connect_errno()){
        echo "Connection failed" . $mysql_connect_error();
        exit();
    }
    $total = mysqli_num_rows($result);

    if($total == 0){
        $sql = "SELECT ID_user,Nickname,Password,email FROM usuario WHERE email = '$email';";
        $result = mysqli_query($con, $sql);

        if(mysqli_connect_errno()){
            echo "Connection failed" . $mysql_connect_error();
            exit();
        }
        $total = mysqli_num_rows($result);

        if($total == 0){
            $sql = "INSERT INTO `usuario` (`Nickname`, `Password`, `email`) VALUES ('$user', '$password', '$email');";

            $result = mysqli_query($con, $sql);

            if(mysqli_connect_errno()){
                echo "Connection failed" . $mysql_connect_error();
                exit();
            }
            echo "$result";
        }
        else{
            echo "ErrorCorreo";
        }
    }
    else{
        echo "ErrorUsuario";
    }
?>