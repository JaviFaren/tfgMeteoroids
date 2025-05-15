<?php
require_once 'db_config.php';

header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

$identifier = $_POST['identifier'];
$password = $_POST['password'];

$stmt = $conn->prepare("SELECT id, name, email, password FROM users WHERE email = ? OR name = ?");
$stmt->bind_param("ss", $identifier, $identifier);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    $stmt->bind_result($id, $name, $email, $hashedPassword);
    $stmt->fetch();
    
    if (password_verify($password, $hashedPassword)) {
        $token = bin2hex(random_bytes(32)); // Generar un token seguro de 64 caracteres 
        
    	$update = $conn->prepare("UPDATE users SET session_token = ? WHERE id = ?"); // Guardar el token en la base de datos
    	$update->bind_param("si", $token, $id);
    	$update->execute();
    	$update->close();    
                        
        echo json_encode([
            "success" => true,
            "message" => "Login exitoso",
            "user" => [
                "id" => $id,
                "name" => $name,
                "email" => $email,
                "session_token" => $token,
            ]
        ]);
    } else {
        echo json_encode(["success" => false, "message" => "Contraseña incorrecta", "code" => "wrong_password"]);
    }
} else {
    echo json_encode(["success" => false, "message" => "Usuario no encontrado", "code" => "user_not_found"]);
}

$stmt->close();
$conn->close();
?>
