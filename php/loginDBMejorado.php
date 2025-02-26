<?php
    $con = mysqli_connect('fdb1028.awardspace.net', '4591675_tfgdb','tfg_Placeholder741' , '4591675_tfgdb');

    $user = $_POST["usuario"];
    $password = $_POST["password"];
    $continuar = 0;
    
	$sql2 = "SELECT ID_user,Nickname,Password FROM usuario WHERE Nickname = '$user';";
	$result = mysqli_query($con, $sql2);

    if(mysqli_connect_errno()){
        echo "Connection failed" . $mysql_connect_error();
        exit();
    }
    $total = mysqli_num_rows($result);
	
	if($total != 0){
    	$continuar = 1;
    }
	else{
     	$sql2 = "SELECT ID_user,Nickname,Password FROM usuario WHERE email = '$email';";
        $result = mysqli_query($con, $sql2);

        if(mysqli_connect_errno()){
            echo "Connection failed" . $mysql_connect_error();
            exit();
        }

        $total = mysqli_num_rows($result);
	
        if($total != 0){
            $continuar = 2
        }
        else{
            $continuar = 3
        }
    }

    if ($continuar == 1 || $continuar == 2){

        $sql = "SELECT ID_user,Nickname,Password FROM usuario WHERE Password = '$password' and ";
        if($continuar == 1){
            $sql = $sql + "Nickname = '$user';";
        }
        else{
            $sql = $sql + "email = '$email';";
        }


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
                $result = mysqli_query($con, $sql);
                if(mysqli_connect_errno()){
                    echo "Connection failed" . $mysql_connect_error();
                    exit();
                }
                $total = mysqli_num_rows($result);

                if($total == 0){
                    echo "contraseña incorrecta";
                }
                
            }
    }
    else{
        echo "usuario o correo incorrecto"
    }
?>